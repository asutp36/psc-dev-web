using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class WashDiscountViewModel
    {
        public string Wash { get; set; }
        public List<PostDiscountViewModel> Posts { get; set; }
    }
}
