using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class SetWashParameter<T>
    {
        public string washCode { get; set; }
        public T value { get; set; }
    }
}
