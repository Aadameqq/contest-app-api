using Core.Auth.Entities;
using Core.Auth.Infrastructure.Persistence;
using Core.Common.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Common.Infrastructure.Persistence;

public class CommonContext(IOptions<DatabaseOptions> databaseConfig)
	: IdentityDbContext<IdentityUser>
{
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseNpgsql(databaseConfig.Value.ConnectionString);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfiguration(new RolesEntityTypeConfiguration());
	}
}
