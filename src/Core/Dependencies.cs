using Core.Common.Infrastructure.Mediator;
using Core.Common.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class Dependencies
{
	public static void SetUpCore(this IServiceCollection services)
	{
		services.AddMediatR(cfg =>
		{
			cfg.RegisterGenericHandlers = true;
			cfg.AddOpenBehavior(typeof(FlushDatabaseContextBehaviour<,>));
			cfg.RegisterServicesFromAssembly(typeof(Dependencies).Assembly);
		});
		services.AddDbContext<CommonContext>();
	}
}
