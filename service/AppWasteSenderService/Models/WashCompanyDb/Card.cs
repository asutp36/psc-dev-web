using System;
using System.Collections.Generic;

#nullable disable

namespace AppWasteSenderService.Models.WashCompanyDb
{
    public partial class Card
    {
        public Card()
        {
            MobileSendings = new HashSet<MobileSending>();
        }

        public int Idcard { get; set; }
        public int Idowner { get; set; }
        public string CardNum { get; set; }
        public int IdcardStatus { get; set; }
        public int IdcardType { get; set; }
        public int? Balance { get; set; }
        public int LocalizedBy { get; set; }
        public int LocalizedId { get; set; }

        public virtual ICollection<MobileSending> MobileSendings { get; set; }
    }
}
