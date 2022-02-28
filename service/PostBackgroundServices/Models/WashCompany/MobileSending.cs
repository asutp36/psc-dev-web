using System;
using System.Collections.Generic;

#nullable disable

namespace PostBackgroundServices.Models.WashCompany
{
    public partial class MobileSending
    {
        public int IdmobileSending { get; set; }
        public int Idcard { get; set; }
        public int Idpost { get; set; }
        public DateTime Dtime { get; set; }
        public int Amount { get; set; }
        public string StatusCode { get; set; }
        public string ResultMessage { get; set; }

        public virtual Post IdpostNavigation { get; set; }
    }
}
