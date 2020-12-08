using System;
using System.Collections.Generic;

#nullable disable

namespace CardsMobileService.Models
{
    public partial class Card
    {
        public Card()
        {
            Operations = new HashSet<Operation>();
        }

        public int Idcard { get; set; }
        public int Idowner { get; set; }
        public string CardNum { get; set; }
        public int IdcardStatus { get; set; }
        public int IdcardType { get; set; }
        public int? Balance { get; set; }
        public int LocalizedBy { get; set; }
        public int LocalizedId { get; set; }

        public virtual CardStatus IdcardStatusNavigation { get; set; }
        public virtual CardType IdcardTypeNavigation { get; set; }
        public virtual Owner IdownerNavigation { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }
    }
}
