using System;
using System.Collections.Generic;

#nullable disable

namespace AuthenticationService.Models.UserAuthenticationDb
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public int Idrole { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
