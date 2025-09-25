using Core.Common.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Web.Identity;

public static class IdentityConfiguration
{
	public static void SetUpAuth(this IServiceCollection services)
	{
		services.AddAuthorization();
		services
			.AddIdentityCore<IdentityUser>()
			.AddRoles<IdentityRole>()
			.AddSignInManager()
			.AddEntityFrameworkStores<AppDbContext>();

		services
			.AddAuthentication(IdentityConstants.ApplicationScheme)
			.AddIdentityCookies();

		services.Configure<IdentityOptions>(options =>
		{
			options.SignIn.RequireConfirmedEmail = false;
			options.SignIn.RequireConfirmedAccount = false;
			options.User.RequireUniqueEmail = true;
		});
	}

	public static void UseAuth(this WebApplication app)
	{
		app.UseAuthentication();
		app.UseAuthorization();
	}
}
