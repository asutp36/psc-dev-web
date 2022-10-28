using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Models.DTOs
{
    public class RegionDTO
    {
        public int IdRegion { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public IEnumerable<WashDTO> Washes { get; set; }
    }
}
