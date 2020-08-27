using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class UsersAvailableWash
    {
        public int IduserAvailableWash { get; set; }
        public int Iduser { get; set; }
        public int Idwash { get; set; }
        public int Available { get; set; }

        public virtual Users IduserNavigation { get; set; }
        public virtual Wash IdwashNavigation { get; set; }
    }
}
