using System;

namespace UserManagement
{
    public class User
    {
        public String Firstname  { get; set; }

        public String Middlename { get; set; }

        public String Lastname { get; set; }

        public String Email { get; set; }

        public HashCode Password { get; set; }

        public DateTime Modified { get; set; }

        public DateTime Created { get; set; }
    }
}
