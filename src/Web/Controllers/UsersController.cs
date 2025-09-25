using Core.Auth.Entities;
using Core.Common.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Web.Controllers.Requests;
using Web.Controllers.Responses;
using Web.Identity;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(
	UserManager<IdentityUser> userManager,
	RoleManager<IdentityRole> roleManager,
	AppDbContext ctx
) : ControllerBase
{
	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
	{
		var user = new IdentityUser { UserName = req.Email, Email = req.Email };
		var result = await userManager.CreateAsync(user, req.Password);

		if (result.Succeeded)
			return Created();

		var modelState = new ModelStateDictionary();
		foreach (var e in result.Errors)
		{
			foreach (var key in new[] { "Email", "Password" })
			{
				if (e.Code.Contains(key))
				{
					modelState.AddModelError(key, e.Description);
				}
			}
		}
		return ValidationProblem(modelState);
	}

	[HttpGet("@me")]
	[Authorize]
	public async Task<ActionResult<GetUserResponse>> GetCurrentUser()
	{
		var user = await userManager.GetUserAsync(User);

		if (user is null)
		{
			return Unauthorized();
		}

		var roles = await userManager.GetRolesAsync(user);

		return new GetUserResponse(user.Id, user.Email, roles.ToList());
	}

	[HttpGet("{id}")]
	[CheckAuth(Role.Admin, Role.Moderator)]
	public async Task<ActionResult<GetUserResponse>> GetUser([FromRoute] string id)
	{
		var user = await userManager.FindByIdAsync(id);

		if (user is null)
		{
			return NotFound("User not found");
		}

		var roles = await userManager.GetRolesAsync(user);

		return new GetUserResponse(user.Id, user.Email, roles.ToList());
	}

	[HttpPut("{id}/roles")]
	[CheckAuth(Role.Admin)]
	public async Task<IActionResult> ManageRoles(
		[FromRoute] string id,
		[FromBody] ManageRolesRequest req
	)
	{
		var user = await userManager.FindByIdAsync(id);

		if (user is null)
		{
			return NotFound("User not found");
		}

		var currentState = await userManager.GetRolesAsync(user);
		var newState = req.Roles;

		foreach (var role in newState)
		{
			var exists = await roleManager.RoleExistsAsync(role);

			if (!exists)
			{
				return BadRequest($"Role with name: '{role}' not found");
			}
		}

		var added = newState
			.Except(currentState, StringComparer.OrdinalIgnoreCase)
			.ToList();
		var removed = currentState
			.Except(newState, StringComparer.OrdinalIgnoreCase)
			.ToList();

		await using var tx = await ctx.Database.BeginTransactionAsync();

		var result = await userManager.AddToRolesAsync(user, added);

		if (result.Succeeded)
		{
			result = await userManager.RemoveFromRolesAsync(user, removed);
		}

		if (result.Succeeded)
		{
			await tx.CommitAsync();
			return Ok();
		}

		await tx.RollbackAsync();

		foreach (var e in result.Errors)
		{
			ModelState.AddModelError(e.Code, e.Description);
		}

		return ValidationProblem(ModelState);
	}
}
