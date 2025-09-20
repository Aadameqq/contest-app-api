using Core.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Auth.Infrastructure.Persistence;

public class RolesEntityTypeConfiguration : IEntityTypeConfiguration<IdentityRole>
{
	public void Configure(EntityTypeBuilder<IdentityRole> builder)
	{
		var roles = Enum.GetNames(typeof(Role))
			.Select(roleName => new IdentityRole
			{
				Id = roleName.ToLower(),
				Name = roleName,
				NormalizedName = roleName.ToUpper(),
			})
			.ToList();

		builder.HasData(roles);
	}
}
