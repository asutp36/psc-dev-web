using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class GroupViewModel
    { 
        public int idGroup { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public List<CardViewModel> cards { get; set; }
    }
}
