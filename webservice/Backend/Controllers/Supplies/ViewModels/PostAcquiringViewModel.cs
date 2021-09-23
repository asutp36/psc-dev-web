using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class PostAcquiringViewModel
    {
        public string post { get; set; }
        public AcquiringModel acquiring { get; set; }
    }
}
