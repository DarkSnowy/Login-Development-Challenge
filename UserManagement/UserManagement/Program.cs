using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UserManagement.Database;
using Microsoft.AspNetCore.Identity;
using UserManagement.Database.Models;

namespace UserManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            CreateDb(host);
            host.Run();
        }

        /// <summary>
        /// Create the database if it doesn't already exist.
        /// </summary>
        /// <param name="host"></param>
        public static void CreateDb(IHost host)
        {
            // Get a scope so that we can access services.
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                // Get the database context service.
                UserManagementContext dbContext = services.GetRequiredService<UserManagementContext>();
                UserManager<ApplicationUser > userManager = services.GetRequiredService<UserManager<ApplicationUser >>();
                RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // Initialize the database.
                DbInitializer.Initialize(dbContext, userManager, roleManager);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating or initializing the database.");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
