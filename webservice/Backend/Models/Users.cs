using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class Users
    {
        public Users()
        {
            UsersAvailableWash = new HashSet<UsersAvailableWash>();
        }

        public int Iduser { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }

        public virtual ICollection<UsersAvailableWash> UsersAvailableWash { get; set; }
    }
}
