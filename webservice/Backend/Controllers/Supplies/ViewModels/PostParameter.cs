using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class PostParameter<T> : IEquatable<PostParameter<T>>
    {
        public string postCode { get; set; }
        public T value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is PostParameter<T> && Equals(obj as PostParameter<T>);
        }

        public bool Equals(PostParameter<T> p)
        {
            return this.value.Equals(p.value);
        }
    }
}
