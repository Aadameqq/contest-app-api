using App.Common.Infrastructure.Options;

namespace App.Common.Web;

public static class OptionsConfiguration
{
	public static void SetUpOptions(this WebApplicationBuilder builder)
	{
		var customAppsettings = Environment.GetEnvironmentVariable("CUSTOM_APPSETTINGS");

		if (!string.IsNullOrEmpty(customAppsettings))
		{
			builder.Configuration.AddJsonFile(
				$"appsettings.{customAppsettings}.json",
				optional: false,
				reloadOnChange: true
			);
		}

		builder.Configuration.AddEnvironmentVariables();
		builder.Configuration.AddUserSecrets<Program>();

		var services = builder.Services;
		AddOptions<DatabaseOptions>(services, "Database");
	}

	private static void AddOptions<T>(IServiceCollection services, string sectionName)
		where T : class
	{
		services
			.AddOptions<T>()
			.BindConfiguration(sectionName)
			.ValidateDataAnnotations()
			.ValidateOnStart();
	}
}
