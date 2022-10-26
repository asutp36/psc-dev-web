using System;
using System.Collections.Generic;

#nullable disable

namespace AuthenticationService.Models.UserAuthenticationDb
{
    public partial class WashType
    {
        public WashType()
        {
            Washes = new HashSet<Wash>();
        }

        public int IdwashType { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Wash> Washes { get; set; }
    }
}
