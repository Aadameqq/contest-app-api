using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Web.OpenApi;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var hasAuthorize = context
			.MethodInfo.GetCustomAttributes(true)
			.OfType<AuthorizeAttribute>()
			.Any();

		if (!hasAuthorize)
			return;

		operation.Description = "🔐 Requires authentication";

		operation.Security ??= [];

		var scheme = new OpenApiSecurityScheme
		{
			Reference = new OpenApiReference
			{
				Type = ReferenceType.SecurityScheme,
				Id = "CookieAuth",
			},
		};

		operation.Security.Add(new OpenApiSecurityRequirement { [scheme] = [] });
	}
}
