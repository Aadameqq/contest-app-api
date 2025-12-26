using App.Features.Tags.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Features.Tags.Infrastructure;

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
