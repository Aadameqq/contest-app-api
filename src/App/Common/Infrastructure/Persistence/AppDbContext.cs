using App.Common.Domain;
using App.Common.Infrastructure.Options;
using App.Common.Logic.Ports;
using App.Features.Auth.Infrastructure;
using App.Features.Problems.Domain;
using App.Features.Problems.Infrastructure;
using App.Features.Tags.Domain;
using App.Features.Tags.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace App.Common.Infrastructure.Persistence;

public class AppDbContext(IOptions<DatabaseOptions> databaseConfig)
	: IdentityDbContext<IdentityUser>,
		UnitOfWork
{
	public DbSet<Tag> Tags { get; set; }
	public DbSet<Problem> Problems { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseNpgsql(databaseConfig.Value.ConnectionString);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfiguration(new RolesEntityTypeConfiguration());
		modelBuilder.ApplyConfiguration(new TagsEntityTypeConfiguration());
		modelBuilder.ApplyConfiguration(new ProblemsEntityTypeConfiguration());
	}

	public override Task<int> SaveChangesAsync(CancellationToken ct = default)
	{
		foreach (
			var entry in ChangeTracker
				.Entries()
				.Where(entry => entry.Entity.GetType().GetProperty("CreatedAt") != null)
		)
		{
			if (entry.State == EntityState.Added)
			{
				entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
			}
			if (entry.State == EntityState.Modified)
			{
				entry.Property("CreatedAt").IsModified = false;
			}
			entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
		}
		return base.SaveChangesAsync(ct);
	}
}
