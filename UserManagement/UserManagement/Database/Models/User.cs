using System;
using System.Collections.Generic;

#nullable enable

namespace UserManagement.Database.Models
{
    public class User
    {
        public enum Account
        {
            User, Staff, Admin
        }

        public int UserID { get; set; }

        public string Firstname { get; set; }

        public string? Middlename { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public Account? AccountType { get; set; }

        public DateTime Modified { get; set; }

        public DateTime Created { get; set; }

        public ICollection<UserPermission> Permissions { get; set; } = new List<UserPermission>();
    }
}
