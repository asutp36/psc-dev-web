using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Models.Test
{
    public class RegionModel
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public List<WashModel> Washes { get; set; }
    }
}
