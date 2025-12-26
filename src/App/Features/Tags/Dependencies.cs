using App.Features.Tags.Infrastructure;
using App.Features.Tags.Logic;
using App.Features.Tags.Logic.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace App.Features.Tags;

public static class Dependencies
{
	public static void SetUpTags(this IServiceCollection services)
	{
		services.AddScoped<TagsRepository, EfTagsRepository>();

		services.AddScoped<TagsService>();
	}
}
