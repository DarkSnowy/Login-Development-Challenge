using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UserManagement.Database;

namespace UserManagement.Database.Models
{
    public class ApplicationUser : IdentityUser, IBaseEntity
    {
        [MaxLength(36)]
        public override string Id { get; set; }

        public string Firstname { get; set; } = "";

        public string Middlename { get; set; } = "";

        public string Lastname { get; set; } = "";

        public override string Email
        {
            get { return base.Email; }
            set
            {
                base.Email = value;
                base.UserName = value;
            }
        }

        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }
    }
}
