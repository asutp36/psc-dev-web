using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class User
    {
        public int Iduser { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long? PhoneInt { get; set; }
        public int Idrole { get; set; }

        public virtual Role IdroleNavigation { get; set; }
    }
}
