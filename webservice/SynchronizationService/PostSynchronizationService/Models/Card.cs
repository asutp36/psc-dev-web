using System;
using System.Collections.Generic;

#nullable disable

namespace PostSynchronizationService.Models
{
    public partial class Card
    {
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
    }
}
