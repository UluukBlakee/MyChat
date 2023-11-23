using Microsoft.AspNetCore.Identity;
using MyChat.Models;

namespace MyChat.Services
{
    public class AdminInitializer
    {
        public static async Task SeedAdminUser(RoleManager<IdentityRole<int>> roleManager, UserManager<User> userManager)
        {
            string adminEmail = "admin@admin.com";
            string adminPassword = "1qwe@QWE";
            var roles = new[] { "admin", "user" };
            foreach (var r in roles)
            {
                if (await roleManager.FindByNameAsync(r) is null)
                    await roleManager.CreateAsync(new IdentityRole<int>(r));
            }
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                User admin = new User { Email = adminEmail, UserName = adminEmail };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "admin");
            }
        }
    }
}
