using System;
using System.Collections.Generic;

#nullable disable

namespace CardsMobileService.Models
{
    public partial class CardGroup
    {
        public int Idcard { get; set; }
        public int Idgroup { get; set; }

        public virtual Card IdcardNavigation { get; set; }
        public virtual Group IdgroupNavigation { get; set; }
    }
}
