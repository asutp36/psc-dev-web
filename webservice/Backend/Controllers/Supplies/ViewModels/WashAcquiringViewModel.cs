using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class WashAcquiringViewModel
    {
        public string Wash { get; set; }
        public List<PostAcquiringViewModel> Posts { get; set; }
    }
}
