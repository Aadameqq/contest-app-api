using App.Common.Domain;
using Microsoft.AspNetCore.Authorization;

namespace App.Common.Web.Identity;

public class CheckAuthAttribute : AuthorizeAttribute
{
	public CheckAuthAttribute(params Role[] roles)
	{
		Roles = string.Join(", ", roles.Select(role => role.ToString()));
	}
}
