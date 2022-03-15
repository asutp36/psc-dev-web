using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class Company
    {
        public Company()
        {
            Regions = new HashSet<Region>();
        }

        public int Idcompany { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Region> Regions { get; set; }
    }
}
