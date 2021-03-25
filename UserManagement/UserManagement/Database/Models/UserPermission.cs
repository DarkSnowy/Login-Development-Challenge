
namespace UserManagement.Database.Models
{
    public class UserPermission
    {
        public enum PermissionType
        {
            ViewAllUsers, EditUsers, ViewHistory, FullAdmin
        }

        public int UserPermissionId { get; set; }

        public PermissionType Permission { get; set; }
    }
}