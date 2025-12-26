using App.Common.Infrastructure.Persistence;
using App.Common.Logic;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

public abstract class TestBase<T>(TestWebApplicationFactory factory) : IAsyncLifetime
	where T : Service
{
	protected abstract Task Seed(AppDbContext ctx);

	public async Task InitializeAsync()
	{
		using var scope = factory.Services.CreateScope();
		var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		await Seed(ctx);
		await ctx.SaveChangesAsync();
	}

	public async Task DisposeAsync()
	{
		await factory.ResetDatabase();
	}

	protected Scoped UseScope()
	{
		return new Scoped(factory.Services.CreateScope());
	}

	protected class Scoped(IServiceScope scope) : IDisposable
	{
		public T Service => scope.ServiceProvider.GetRequiredService<T>();
		public AppDbContext Ctx =>
			scope.ServiceProvider.GetRequiredService<AppDbContext>();

		public void Dispose()
		{
			scope.Dispose();
		}
	}
}
