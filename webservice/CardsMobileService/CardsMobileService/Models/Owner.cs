using System;
using System.Collections.Generic;

#nullable disable

namespace CardsMobileService.Models
{
    public partial class Owner
    {
        public Owner()
        {
            Cards = new HashSet<Card>();
        }

        public int Idowner { get; set; }
        public string Phone { get; set; }
        public long? PhoneInt { get; set; }
        public int LocalizedBy { get; set; }
        public int LocalizedId { get; set; }

        public virtual ICollection<Card> Cards { get; set; }
    }
}
