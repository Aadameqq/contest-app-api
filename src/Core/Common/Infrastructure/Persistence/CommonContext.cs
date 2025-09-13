using Core.Auth.Entities;
using Core.Auth.Infrastructure.Persistence;
using Core.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Common.Infrastructure.Persistence;

public class CommonContext(IOptions<DatabaseOptions> databaseConfig) : DbContext
{
	public DbSet<Greeting> Greetings { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseNpgsql(databaseConfig.Value.ConnectionString);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new GreetingEntityTypeConfiguration());
	}
}
