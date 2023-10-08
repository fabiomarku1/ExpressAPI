using Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Repository.Configuration;

public class UserRolesConfiguration : IEntityTypeConfiguration<ApplicationUserRoles>
{
    public void Configure(EntityTypeBuilder<ApplicationUserRoles> builder)
    {
        builder.HasData(
            new ApplicationUserRoles
            {
                UserId = 1,
                RoleId = 2, //admin
            },
            new ApplicationUserRoles
            {
                UserId = 2,
                RoleId = 1, //user
            }
        );
    }
}