using System;
using UserManagement.Database.Models;
using Microsoft.AspNetCore.Identity;
using UserManagement.JWTAuthentication;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.Database
{
    public static class DbInitializer
    {
        public static async void Initialize(UserManagementContext context, UserManager<ApplicationUser > userManager, RoleManager<IdentityRole> roleManager)
        {
            // Creates or updates the database schema.
            await context.Database.MigrateAsync();

            // Create Admin roll in the roleManager if it doesn't exist.
            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            // Create Staff roll if it doesn't exist.
            if (!await roleManager.RoleExistsAsync(UserRoles.Staff))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Staff));

            // Create User roll if it doesn't exist.
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            string adminEmail = "admin@usermanagement.com";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            // Checks to see if the admin account has been created and exit if it has.
            if (adminUser != null)
                return;

            // Create an admin user.
            ApplicationUser  admin = new()
            {
                Email = adminEmail,
                Firstname = "System",
                Lastname = "Administrator",
                SecurityStamp = Guid.NewGuid().ToString()
            };

            await userManager.CreateAsync(admin, "AdminPassword123!");

            // Add admin role.
            await userManager.AddToRoleAsync(admin, UserRoles.Admin);
        }
    }
}
