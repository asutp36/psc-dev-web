using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Models
{
    public class NewAccountViewModel : AccountViewModel
    {
        public string Password { get; set; }
    }
}
