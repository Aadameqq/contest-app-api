using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers.Requests;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
	SignInManager<IdentityUser> signInManager,
	UserManager<IdentityUser> userManager
) : ControllerBase
{
	[HttpPost]
	public async Task<IActionResult> LogIn([FromBody] LogInRequest req)
	{
		var user = await userManager.FindByEmailAsync(req.Email);
		if (user == null)
			return Unauthorized();

		var result = await signInManager.PasswordSignInAsync(
			user,
			req.Password,
			req.RememberMe,
			false
		);

		return result.Succeeded ? Ok() : Unauthorized();
	}

	[Authorize]
	[HttpDelete]
	public async Task<IActionResult> LogOut()
	{
		await signInManager.SignOutAsync();
		return Ok();
	}
}
