using App.Identity;
using Core.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.Features.Auth.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController(RoleManager<IdentityRole> roleManager) : ControllerBase
{
	[CheckAuth(Role.Admin, Role.Moderator)]
	[HttpGet]
	public List<string> GetRoles()
	{
		return roleManager.Roles.Select(role => role.Name!).ToList();
	}
}
