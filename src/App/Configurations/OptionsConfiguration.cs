using App.Common.Infrastructure.Options;

namespace App.Configurations;

public static class OptionsConfiguration
{
	public static void SetUpOptions(this WebApplicationBuilder builder)
	{
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
