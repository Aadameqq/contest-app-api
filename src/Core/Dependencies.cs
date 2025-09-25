using Core.Common.Application.Ports;
using Core.Common.Infrastructure.Persistence;
using Core.Problems.Application.Ports;
using Core.Problems.Application.Services.Tags;
using Core.Problems.Infrastructure;
using Core.Problems.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class Dependencies
{
	public static void SetUpCore(this IServiceCollection services)
	{
		services.AddDbContext<AppDbContext>();

		services.AddScoped<TagsRepository, EfTagsRepository>();
		services.AddSingleton<SlugGenerator, SlugGeneratorImpl>();

		services.AddScoped<UnitOfWork>(c => c.GetRequiredService<AppDbContext>());

		services.AddScoped<TagsService>();
	}
}
