using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.Common.Web.OpenApi;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var authorizeAttrs = context
			.MethodInfo.GetCustomAttributes(true)
			.OfType<AuthorizeAttribute>();

		if (!authorizeAttrs.Any())
			return;

		var attr = authorizeAttrs.First();

		operation.Description = "🔐 Requires authentication";

		if (attr.Roles is null)
		{
			operation.Description += $"<br>🪪 Requires no role";
		}
		else
		{
			operation.Description +=
				$"<br>🪪 Requires any of the following roles: {attr.Roles}";
		}

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
