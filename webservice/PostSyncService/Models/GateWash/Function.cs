using System;
using System.Collections.Generic;

#nullable disable

namespace PostSyncService.Models.GateWash
{
    public partial class Function
    {
        public int Idfunction { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
    }
}
