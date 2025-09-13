using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;

namespace Web.Configurations;

public static class StatusCodesConfiguration
{
	public static void SetUpStatusCodes(this WebApplicationBuilder builder)
	{
		var services = builder.Services;
		services
			.AddProblemDetails(options =>
			{
				options.IncludeExceptionDetails = (_, _) =>
					builder.Environment.IsDevelopment();
			})
			.AddProblemDetailsConventions();
	}

	public static void UseStatusCodes(this WebApplication app)
	{
		app.UseProblemDetails();
	}
}
