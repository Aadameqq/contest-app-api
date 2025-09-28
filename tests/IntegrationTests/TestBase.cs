using Core.Common.Application;
using Core.Common.Infrastructure.Persistence;
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
		Console.WriteLine("test");
		await Seed(ctx);
		await ctx.SaveChangesAsync();
	}

	public async Task DisposeAsync()
	{
		await factory.ResetDatabase();
	}

	protected Scoped GetScope()
	{
		return new Scoped(factory.Services.CreateScope());
	}

	protected class Scoped(IServiceScope scope) : IDisposable
	{
		public T Service => scope.ServiceProvider.GetRequiredService<T>();

		public void Dispose()
		{
			scope.Dispose();
		}
	}
}
