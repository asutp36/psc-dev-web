using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileIntegration.Controllers.Supplies
{
    public class PhoneFormatter
    {
        public long phoneInt { get; set; }
        public string phone { get; set; }

        public PhoneFormatter(string phone)
        {
            this.phoneInt = long.Parse(string.Join("", phone.Where(c => char.IsDigit(c))));

            this.phone = String.Format("{0:+#-###-###-##-##}", this.phoneInt);
        }
    }
}