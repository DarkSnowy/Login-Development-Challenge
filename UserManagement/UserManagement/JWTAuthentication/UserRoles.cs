using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.JWTAuthentication
{
    public class UserRoles
    {
        public const string Admin = "Admin";
        public const string Staff = "Staff";
        public const string StaffOrAdmin = "Admin,Staff";
        public const string User = "User";
    }
}
