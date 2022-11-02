using AuthenticationService.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Models.DTOs
{
    public class AccountInfoDto
    {
        public int id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public long? Phone { get; set; }
        public RoleDTO Role { get; set; }
        public IEnumerable<WashInfo> Washes { get; set; }
    }
}
