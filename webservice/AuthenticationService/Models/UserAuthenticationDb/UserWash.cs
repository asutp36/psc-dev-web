using System;
using System.Collections.Generic;

#nullable disable

namespace AuthenticationService.Models.UserAuthenticationDb
{
    public partial class UserWash
    {
        public int Iduser { get; set; }
        public int Idwash { get; set; }

        public virtual User IduserNavigation { get; set; }
        public virtual Wash IdwashNavigation { get; set; }
    }
}
