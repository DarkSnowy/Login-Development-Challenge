using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using UserManagement.Database.Models;

namespace UserManagement.Database
{
    public partial class UserManagementContext : IdentityDbContext<ApplicationUser >
    {
        public UserManagementContext(DbContextOptions<UserManagementContext> options)
            : base(options)
        {

        }

        /// <summary>
        /// Created the database tables from the models when the models are being created.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Saves entity changes to the database and sets the created and modified dates.
        /// </summary>
        /// <returns>How many records were created or modified.</returns>
        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IBaseEntity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((IBaseEntity)entityEntry.Entity).Modified = DateTime.Now;

                // We only set the created date to now if it is the first time the record is being added to the database.
                if (entityEntry.State == EntityState.Added)
                {
                    ((IBaseEntity)entityEntry.Entity).Created = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }


    }
}