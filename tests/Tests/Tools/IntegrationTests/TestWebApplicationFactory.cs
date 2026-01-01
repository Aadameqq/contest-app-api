using System.Data.Common;
using App.Common.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace Tests.Tools.IntegrationTests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
	private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder()
		.WithImage("postgres:14")
		.WithDatabase("contest-app")
		.WithUsername("admin")
		.WithPassword("admin")
		.Build();

	private Respawner respawner = null!;
	private DbConnection dbConnection = null!;

	public async Task InitializeAsync()
	{
		await dbContainer.StartAsync();
		CreateClient();

		dbConnection = new NpgsqlConnection(dbContainer.GetConnectionString());
		await dbConnection.OpenAsync();

		using var scope = Services.CreateScope();
		var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		await ctx.Database.MigrateAsync();

		respawner = await Respawner.CreateAsync(
			dbConnection,
			new RespawnerOptions { DbAdapter = DbAdapter.Postgres }
		);
	}

	public async Task ResetDatabase()
	{
		await respawner.ResetAsync(dbConnection);
	}

	public new async Task DisposeAsync()
	{
		await dbConnection.DisposeAsync();
		await dbContainer.DisposeAsync();
		await base.DisposeAsync();
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseSetting(
			"Database:ConnectionString",
			dbContainer.GetConnectionString()
		);
	}
}
