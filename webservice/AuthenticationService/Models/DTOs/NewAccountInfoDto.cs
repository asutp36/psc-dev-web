using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Models.DTOs
{
    public class NewAccountInfoDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public long? Phone { get; set; }
        public int IdRole { get; set; }
        public IEnumerable<string> Washes { get; set; }
    }
}
