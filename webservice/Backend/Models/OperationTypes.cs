using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class OperationTypes
    {
        public OperationTypes()
        {
            Operations = new HashSet<Operations>();
        }

        public int IdoperationType { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Operations> Operations { get; set; }
    }
}
