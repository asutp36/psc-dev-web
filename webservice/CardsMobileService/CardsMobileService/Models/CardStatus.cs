using System;
using System.Collections.Generic;

#nullable disable

namespace CardsMobileService.Models
{
    public partial class CardStatus
    {
        public CardStatus()
        {
            Cards = new HashSet<Card>();
        }

        public int IdcardStatus { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Card> Cards { get; set; }
    }
}
