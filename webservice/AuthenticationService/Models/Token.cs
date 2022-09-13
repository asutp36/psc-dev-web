using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Models
{
    public class Token
    {
        public string AccessToken { get; set; }
        public string Login { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
    }
}
