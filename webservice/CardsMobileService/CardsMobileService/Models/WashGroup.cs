using System;
using System.Collections.Generic;

#nullable disable

namespace CardsMobileService.Models
{
    public partial class WashGroup
    {
        public int Idwash { get; set; }
        public int Idgroup { get; set; }

        public virtual Group IdgroupNavigation { get; set; }
        public virtual Wash IdwashNavigation { get; set; }
    }
}
