using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class CardWash
    {
        public int Idcard { get; set; }
        public int Idwash { get; set; }

        public virtual Cards IdcardNavigation { get; set; }
        public virtual Wash IdwashNavigation { get; set; }
    }
}
