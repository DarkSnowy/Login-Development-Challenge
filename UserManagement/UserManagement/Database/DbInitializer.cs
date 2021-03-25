using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UserManagement.Database.Models;
using Microsoft.AspNetCore.Identity;

namespace UserManagement.Database
{
    public static class DbInitializer
    {
        public static void Initialize(UserManagementContext context)
        {
            // Creates the schema and tables if they have not yet been created.
            context.Database.EnsureCreated();

            string adminEmail = "admin@usermanagement.com";

            // Checks to see if the admin account has been created and exits if it has.
            if (context.Users.Any(x => x.Email == adminEmail))
                return;

            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes("password", salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);

            User admin = new()
            {
                Firstname = "System",
                Middlename = null,
                Lastname = "Administrator",
                Email = adminEmail,
                AccountType = User.Account.Admin,
                Created = DateTime.Now,
                Permissions = new List<UserPermission>()
                {
                    new UserPermission()
                    {
                        Permission = UserPermission.PermissionType.FullAdmin
                    }
                }
            };

            string password = new PasswordHasher<User>().HashPassword(admin, "AdminPassword");
            admin.Password = password;

            context.Add(admin);
            context.SaveChanges();
        }
    }
}
