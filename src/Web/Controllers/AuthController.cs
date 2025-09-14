using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers.Requests;
using Web.Controllers.Responses;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(SignInManager<IdentityUser> signInManager) : ControllerBase
{
	[HttpGet]
	public ActionResult<GetAuthResponse> Get()
	{
		return new GetAuthResponse(signInManager.IsSignedIn(User));
	}

	[HttpPost]
	public async Task<IActionResult> LogIn([FromBody] LogInRequest req)
	{
		var result = await signInManager.PasswordSignInAsync(
			req.Email,
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
