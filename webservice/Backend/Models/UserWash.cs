using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class UserWash
    {
        public int Iduser { get; set; }
        public int Idwash { get; set; }

        public virtual Users IduserNavigation { get; set; }
        public virtual Wash IdwashNavigation { get; set; }
    }
}
