using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Web.Controllers.Requests;
using Web.Controllers.Responses;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(UserManager<IdentityUser> userManager) : ControllerBase
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
			foreach (var key in (string[])["Email", "Password"])
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
	public async Task<ActionResult<GetCurrentUserResponse>> GetCurrentUser()
	{
		var user = await userManager.GetUserAsync(User);
		if (user is null)
		{
			return Unauthorized();
		}

		return new GetCurrentUserResponse(user.Id, user.Email);
	}
}
