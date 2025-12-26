using App.Common.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Features.Auth.Infrastructure;

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
