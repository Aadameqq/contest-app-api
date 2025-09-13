using Core.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Auth.Infrastructure.Persistence;

public class GreetingEntityTypeConfiguration : IEntityTypeConfiguration<Greeting>
{
	public void Configure(EntityTypeBuilder<Greeting> builder) { }
}
