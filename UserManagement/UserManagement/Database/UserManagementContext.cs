using Microsoft.EntityFrameworkCore;
using UserManagement.Database.Models;

namespace UserManagement.Database
{
    public partial class UserManagementContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }

        public UserManagementContext(DbContextOptions<UserManagementContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<UserPermission>().ToTable("UserPermissions");
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
