using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Web.Controllers.Requests;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(UserManager<IdentityUser> userManager) : ControllerBase
{
	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
	{
		var user = new IdentityUser { UserName = req.UserName, Email = req.Email };
		var result = await userManager.CreateAsync(user, req.Password);

		if (!result.Succeeded)
		{
			var modelState = new ModelStateDictionary();
			foreach (var e in result.Errors)
				modelState.AddModelError(e.Code, e.Description);
			return ValidationProblem(modelState);
		}

		return Created();
	}
}
