using System;
using System.Collections.Generic;

namespace MobileIntegration_v2.Models
{
    public partial class CardTypes
    {
        public CardTypes()
        {
            Cards = new HashSet<Cards>();
        }

        public int IdcardType { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Cards> Cards { get; set; }
    }
}
