using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamIndia.TalentFlow.Domain.Entities;

namespace TeamIndia.TalentFlow.API.Helpers;

public static class SeedDataHelper
{
    public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = scope.ServiceProvider.GetService<IConfiguration>();

        string[] roleNames = { "Admin", "Mentor", "Intern" };

        foreach (var roleName in roleNames)
        {
            var exists = await roleManager.RoleExistsAsync(roleName);
            if (!exists)
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }
        }

        var adminEmail = configuration?["SeedAdmin:Email"];
        var adminPassword = configuration?["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = configuration?["SeedAdmin:FullName"] ?? "Admin",
            EmailConfirmed = true,
        };

        try
        {
            ApplicationUser? user = null;
            try
            {
                user = await userManager.FindByEmailAsync(admin.Email);
            }
            catch
            {
                try
                {
                    var lookup = admin.Email.ToLowerInvariant();
                    user = await userManager.Users.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == lookup);
                }
                catch
                {
                    user = null;
                }
            }

            if (user == null)
            {
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    user = await userManager.FindByEmailAsync(admin.Email);
                }
            }

            if (user != null)
            {
                var rolesForUser = await userManager.GetRolesAsync(user);
                if (rolesForUser == null || !rolesForUser.Contains("Admin"))
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
        catch
        {
        }
    }
}
