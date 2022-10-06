using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Models.DTOs
{
    public class NewAccountInfoDto : AccountInfoDto
    {
        public string Password { get; set; }
    }
}
