using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangoAPIService.Models
{
    public class MangoAPIRequestParameters
    {
        public string vpbx_api_key { get; set; }
        public string sign { get; set; }
        public string json { get; set; }
    }
}
