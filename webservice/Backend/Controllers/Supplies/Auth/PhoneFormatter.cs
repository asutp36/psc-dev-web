using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.Auth
{
    public class PhoneFormatter
    {
        public string phoneInt { get; set; }
        public string phone { get; set; }

        public PhoneFormatter(string phone)
        {
            if (phone == null)
            {
                this.phoneInt = "null";
                this.phone = "'null'";
                return;
            }
            this.phoneInt = long.Parse(string.Join("", phone.Where(c => char.IsDigit(c)))).ToString();

            this.phone = String.Format("{0:+#-###-###-##-##}", this.phoneInt);
        }
    }
}
