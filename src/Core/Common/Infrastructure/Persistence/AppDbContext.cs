using Core.Auth.Entities;
using Core.Auth.Infrastructure.Persistence;
using Core.Common.Application.Ports;
using Core.Common.Infrastructure.Options;
using Core.Problems.Entities;
using Core.Problems.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Common.Infrastructure.Persistence;

public class AppDbContext(IOptions<DatabaseOptions> databaseConfig)
	: IdentityDbContext<IdentityUser>,
		UnitOfWork
{
	public DbSet<Tag> Tags { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseNpgsql(databaseConfig.Value.ConnectionString);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfiguration(new RolesEntityTypeConfiguration());
		modelBuilder.ApplyConfiguration(new TagsEntityTypeConfiguration());
	}
}
