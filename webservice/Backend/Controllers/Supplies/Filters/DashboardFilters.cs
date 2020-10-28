using Backend.Controllers.Supplies.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies
{
    public class DashboardFilters
    {
        public List<RegionViewModel> regions { get; set; }
        public List<WashViewModel> washes { get; set; }
        public List<PostViewModel> posts { get; set; }
        public List<ChangerViewModel> changers { get; set; }
        public List<OperationTypeViewModel> operationTypes { get; set; }
    }
}
