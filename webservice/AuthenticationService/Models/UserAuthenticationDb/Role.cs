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
        public bool IsAdmin { get; set; }
        public int Eco { get; set; }
        public int GateWash { get; set; }
        public bool RefillGateWash { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
