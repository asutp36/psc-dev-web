using System;
using System.Collections.Generic;

#nullable disable

namespace CardsMobileService.Models
{
    public partial class OperationType
    {
        public OperationType()
        {
            Operations = new HashSet<Operation>();
        }

        public int IdoperationType { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Operation> Operations { get; set; }
    }
}
