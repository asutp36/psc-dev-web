using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Models.DTOs
{
    public class UpdateAccountModel : NewAccountInfoDto
    {
        public int id { get; set; }
    }
}
