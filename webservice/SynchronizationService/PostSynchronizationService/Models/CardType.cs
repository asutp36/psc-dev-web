using System;
using System.Collections.Generic;

#nullable disable

namespace PostSynchronizationService.Models
{
    public partial class CardType
    {
        public CardType()
        {
            Cards = new HashSet<Card>();
        }

        public int IdcardType { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Card> Cards { get; set; }
    }
}
