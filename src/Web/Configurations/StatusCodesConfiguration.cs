using Core.Common.Application.Exceptions;
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

				options.MapToStatusCode<NoSuch>(StatusCodes.Status404NotFound);
				options.MapToStatusCode<InvalidArgument>(StatusCodes.Status400BadRequest);
			})
			.AddProblemDetailsConventions();
	}

	public static void UseStatusCodes(this WebApplication app)
	{
		app.UseProblemDetails();
	}
}
