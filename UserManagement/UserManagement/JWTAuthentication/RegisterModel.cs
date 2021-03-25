using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.JWTAuthentication
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Firstname is required")]
        public string Firstname { get; set; }

        public string Middlename { get; set; }

        [Required(ErrorMessage = "Lastname is required")]
        public string Lastname { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
