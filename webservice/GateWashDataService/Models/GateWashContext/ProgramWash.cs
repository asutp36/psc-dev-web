using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class ProgramWash
    {
        public int Idprogram { get; set; }
        public int Idwash { get; set; }

        public virtual Program IdprogramNavigation { get; set; }
        public virtual Wash IdwashNavigation { get; set; }
    }
}
