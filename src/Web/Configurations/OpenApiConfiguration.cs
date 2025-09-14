using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Web.OpenApi;

namespace Web.Configurations;

public static class OpenApiConfiguration
{
	public static void SetUpOpenApi(this IServiceCollection services)
	{
		var info = new OpenApiInfo
		{
			Title = "Contest App API Definition",
			Description = "Api definition for development purposes",
			Version = "1.0",
		};

		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(options =>
		{
			var cookieScheme = new OpenApiSecurityScheme
			{
				Name = ".AspNetCore.Identity.Application",
				Type = SecuritySchemeType.ApiKey,
				In = ParameterLocation.Cookie,
			};
			options.AddSecurityDefinition("CookieAuth", cookieScheme);
			options.OperationFilter<AuthorizeCheckOperationFilter>();
			options.SwaggerDoc("docs", info);
		});
	}

	public static void UseOpenApi(this WebApplication app)
	{
		if (!app.Environment.IsDevelopment())
			return;

		app.MapSwagger("/openapi/{documentName}.json");
		app.MapScalarApiReference(
			"/docs",
			options =>
			{
				options
					.WithLayout(ScalarLayout.Classic)
					.WithDefaultOpenAllTags(false)
					.WithClientButton(false)
					.AddDocument("docs", "docs", "/openapi/docs.json")
					.WithDocumentDownloadType(DocumentDownloadType.None)
					.WithDefaultHttpClient(ScalarTarget.Node, ScalarClient.Fetch);
				options.EnabledClients = [ScalarClient.Fetch, ScalarClient.Curl];
				options
					.AddPreferredSecuritySchemes(["CookieAuth"])
					.AddApiKeyAuthentication("CookieAuth", (s) => { });
			}
		);
	}
}
