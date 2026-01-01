using App.Common.Infrastructure;
using App.Common.Infrastructure.Persistence;
using App.Common.Logic;
using App.Common.Logic.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace App.Common;

public static class Dependencies
{
	public static void SetUpCommon(this IServiceCollection services)
	{
		services.AddDbContext<AppDbContext>();

		services.AddScoped<SlugGenerator, SlugGenerator>();
		services.AddSingleton<Slugifier, BasicSlugifier>();
		services.AddScoped<UnitOfWork>(c => c.GetRequiredService<AppDbContext>());
	}
}
