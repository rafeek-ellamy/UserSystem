using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserSystem.Data.Entities;
using UserSystem.Data.Enums;

namespace UserSystem.Data
{
    public class UserSystemDbContext : IdentityDbContext<UserProfile, IdentityRole, string>
    {
        private readonly string _adminId = "c24f1e41-6ad7-4ff0-9abf-4925f0e82f79";

        public UserSystemDbContext(DbContextOptions<UserSystemDbContext> options) : base(options)
        {

        }

        public DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        private UserProfile GetAdminUser()
        {
            return new UserProfile
            {
                Id = _adminId,
                FirstName = "System",
                LastName = "SuperAdmin",
                Email = "admin@admin.com",
                UserTypeId = EUserType.Admin,
                UserName = "SuperAdmin",
                NormalizedUserName = "SUPERADMIN",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<string>().HashPassword(null, "P@ssw0rd")
            };
        }

        public async Task SeedAsync(UserSystemDbContext context)
        {
            if (!context.Users.Any(o => o.Id == _adminId))
            {
                await context.Users.AddAsync(GetAdminUser());
                await context.SaveChangesAsync();
            }

            if (!context.Roles.Any())
            {
                List<IdentityRole> roles =
                [
                    new()
                    {
                        Id = "b4f5f021-136f-49ad-afa4-df32e76049ac",
                        Name = "SuperAdmin",
                        NormalizedName = "SUPERADMIN",
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    },
                    new()
                    {
                        Id = "a917082a-132e-40e0-b4f7-65fa6f85fc8d",
                        Name = "Admin",
                        NormalizedName = "ADMIN",
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    },
                    new()
                    {
                        Id = "b1c584ef-0a53-4d15-bbdb-ecf9409697fa",
                        Name = "SystemUser",
                        NormalizedName = "SYSTEMUSER",
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    },
                ];

                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }

            if (!context.UserRoles.Any())
            {
                //add SuperAdmin role for the main admin
                await context.UserRoles.AddAsync(new IdentityUserRole<string>
                {
                    UserId = _adminId,
                    RoleId = "b4f5f021-136f-49ad-afa4-df32e76049ac",
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
