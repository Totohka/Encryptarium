using Encryptarium.Domain.Model.Entities;
using Microsoft.AspNetCore.Identity;

namespace Encryptarium.Configuration
{
    public static class DefaultUser
    {
        public static async Task SeedDefaultUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<eUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            string adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(adminRole));
            }

            string admin = "admin";
            var adminUser = await userManager.FindByNameAsync(admin);

            if (adminUser == null)
            {
                adminUser = new eUser
                {
                    UserName = admin,
                };

                var password = Environment.GetEnvironmentVariable("AdminPassword");
                var result = await userManager.CreateAsync(adminUser, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRole);
                }
            }
        }
    }
}
