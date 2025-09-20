using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers.Requests;
using Web.Controllers.Responses;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController(RoleManager<IdentityRole> roleManager) : ControllerBase
{
	[Authorize]
	[HttpGet]
	public List<string> GetRoles()
	{
		return roleManager.Roles.Select(role => role.Name!).ToList();
	}
}
