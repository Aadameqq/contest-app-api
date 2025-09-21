using Core.Auth.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Web.Identity;

public class CheckAuthAttribute : AuthorizeAttribute
{
	public CheckAuthAttribute(params Role[] roles)
	{
		Roles = string.Join(", ", roles.Select(role => role.ToString()));
	}
}
