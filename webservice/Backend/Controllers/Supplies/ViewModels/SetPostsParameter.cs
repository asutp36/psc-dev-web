using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class SetPostsParameter<T>
    {
        public string[] posts {get;set;}
        public T value { get; set; }
    }
}
