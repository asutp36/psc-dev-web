using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Models
{
    public class AccountViewModel
    {
        public int id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public long? Phone { get; set; }
        public string Role { get; set; }
        public string RoleName { get; set; }
        public IEnumerable<string> Washes { get; set; }
    }
}
