using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class CardGroup
    {
        public int Idcard { get; set; }
        public int Idgroup { get; set; }

        public virtual Cards IdcardNavigation { get; set; }
        public virtual Groups IdgroupNavigation { get; set; }
    }
}
