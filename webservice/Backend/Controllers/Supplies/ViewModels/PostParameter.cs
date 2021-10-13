using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class PostParameter<T>
    {
        public string code { get; set; }
        public T value { get; set; }

        public bool Equals(PostParameter<T> p)
        {
            return this.value.Equals(p.value);
        }
    }
}
