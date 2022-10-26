using System;
using System.Collections.Generic;

#nullable disable

namespace AuthenticationService.Models.UserAuthenticationDb
{
    public partial class Wash
    {
        public Wash()
        {
            UserWashes = new HashSet<UserWash>();
        }

        public int Idwash { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int IdwashType { get; set; }
        public string Address { get; set; }
        public int Idregion { get; set; }

        public virtual Region IdregionNavigation { get; set; }
        public virtual WashType IdwashTypeNavigation { get; set; }
        public virtual ICollection<UserWash> UserWashes { get; set; }
    }
}
