using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class WashParameter<T>
    {
        public string code { get; set; }
        public List<PostParameter<T>> posts { get; set; } 
        public T value { get 
            {
                PostParameter<T> first = posts.ElementAt(0);

                List<PostParameter<T>> firstEqual = new List<PostParameter<T>>();
                List<PostParameter<T>> other = new List<PostParameter<T>>();

                foreach(PostParameter<T> p in this.posts)
                {
                    if (p.Equals(first))
                        firstEqual.Add(p);
                    else
                        other.Add(p);
                }

                if (firstEqual.Count >= other.Count)
                    return first.value;
                else
                    return other.ElementAt(0).value;

            }
            set { } }
    }
}
