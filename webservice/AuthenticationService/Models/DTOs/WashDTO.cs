using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Models.DTOs
{
    public class WashDTO
    {
        public int IdWash { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public WashTypeDTO Type { get; set; }
    }
}
