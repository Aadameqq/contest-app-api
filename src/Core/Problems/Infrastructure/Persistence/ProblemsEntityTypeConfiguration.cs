using Core.Problems.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Problems.Infrastructure.Persistence;

public class ProblemsEntityTypeConfiguration : IEntityTypeConfiguration<Problem>
{
	public void Configure(EntityTypeBuilder<Problem> builder)
	{
		builder.HasKey(t => t.Id);
		builder.Property(t => t.Title).IsRequired();
		builder.Property(t => t.Slug).IsRequired();
		builder.HasIndex(t => t.Slug).IsUnique();
		builder.Property(b => b.CreatedAt);
		builder.Property(b => b.UpdatedAt);
		builder.HasMany(problem => problem.Tags).WithMany();
	}
}
