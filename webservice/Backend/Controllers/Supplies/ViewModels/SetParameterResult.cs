using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class SetParameterResult
    {
        public string wash { get; set; }
        public List<SetParameterResultPost> posts { get; set; }
    }
}
