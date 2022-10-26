using System;
using System.Collections.Generic;

#nullable disable

namespace AuthenticationService.Models.UserAuthenticationDb
{
    public partial class Region
    {
        public Region()
        {
            Washes = new HashSet<Wash>();
        }

        public int Idregion { get; set; }
        public short Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Wash> Washes { get; set; }
    }
}
