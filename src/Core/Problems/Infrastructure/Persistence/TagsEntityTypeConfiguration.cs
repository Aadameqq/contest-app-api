using Core.Problems.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Problems.Infrastructure.Persistence;

public class TagsEntityTypeConfiguration : IEntityTypeConfiguration<Tag>
{
	public void Configure(EntityTypeBuilder<Tag> builder)
	{
		builder.HasKey(t => t.Id);
		builder.Property(t => t.Title).IsRequired();
		builder.Property(t => t.Slug).IsRequired();
		builder.HasIndex(t => t.Slug).IsUnique();
	}
}
