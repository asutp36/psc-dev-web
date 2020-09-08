using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class RoleWash
    {
        public int Idrole { get; set; }
        public int Idwash { get; set; }

        public virtual Roles IdroleNavigation { get; set; }
        public virtual Wash IdwashNavigation { get; set; }
    }
}
