using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class WashGroup
    {
        public int Idwash { get; set; }
        public int Idgroup { get; set; }

        public virtual Groups IdgroupNavigation { get; set; }
        public virtual Wash IdwashNavigation { get; set; }
    }
}
