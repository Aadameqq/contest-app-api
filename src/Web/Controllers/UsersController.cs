using Core.Auth.Entities;
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
	RoleManager<IdentityRole> roleManager
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
			return BadRequest("User not found");
		}

		var roles = await userManager.GetRolesAsync(user);

		return new GetUserResponse(user.Id, user.Email, roles.ToList());
	}

	[HttpPatch("{id}/roles")]
	[CheckAuth(Role.Admin)]
	public async Task<IActionResult> ManageRoles(
		[FromRoute] string id,
		[FromBody] ManageRolesRequest req
	)
	{
		var user = await userManager.FindByIdAsync(id);

		if (user is null)
		{
			return BadRequest($"User with id: '{id}' not found");
		}

		var added = new List<string>();
		var removed = new List<string>();

		foreach (var change in req.RoleChanges)
		{
			var exists = await roleManager.RoleExistsAsync(change.RoleName);

			if (!exists)
			{
				return BadRequest($"Role with name: '{change.RoleName}' not found");
			}

			switch (change.Op)
			{
				case ManageRolesRequest.ManageRoleOperation.Add:
					added.Add(change.RoleName);
					break;
				case ManageRolesRequest.ManageRoleOperation.Remove:
					removed.Add(change.RoleName);
					break;
				default:
					return BadRequest($"Unknown operation: {change.Op}");
			}
		}

		var result = await userManager.AddToRolesAsync(user, added);

		if (result.Succeeded)
		{
			result = await userManager.RemoveFromRolesAsync(user, removed);
		}

		if (!result.Succeeded)
		{
			return BadRequest(result.Errors.First().Description);
		}

		return Ok();
	}
}
