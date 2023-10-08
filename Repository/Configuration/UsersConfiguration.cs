using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configuration;

public class UsersConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {

        var hasher = new PasswordHasher<ApplicationUser>();

        builder.HasData(
            new ApplicationUser
            {
                Id = 1,
                UserName = "admin",
                FirstName="fabioAdmin",
                LastName="markuAdmin",
                NormalizedUserName = "ADMIN",
                Email = "fabiomarku@admin.com",
                NormalizedEmail = "FABIOMARKU@ADMIN.COM",
                PasswordHash = hasher.HashPassword(null, "password123"),
                SecurityStamp = Guid.NewGuid().ToString()
            },
            new ApplicationUser
            {
                Id = 2,
                UserName = "user",
                FirstName = "fabio",
                LastName = "marku",
                NormalizedUserName = "USER",
                Email = "fabiomarku@user.com",
                NormalizedEmail = "FABIOMARKU@USER.COM",
                PasswordHash = hasher.HashPassword(null, "password123"),
                SecurityStamp = Guid.NewGuid().ToString(),

            }
        );
    }
}
