using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies.ViewModels
{
    public class WashParameter<T>
    {
        public string washCode { get; set; }
        public string washName { get; set; }
        public T value { get 
            {
                if (this.posts == null || this.posts.Count == 0)
                    return default(T);

                PostParameter<T> first = posts.ElementAt(0);

                List<PostParameter<T>> firstEqual = new List<PostParameter<T>>();
                List<PostParameter<T>> other = new List<PostParameter<T>>();

                
                foreach(PostParameter<T> p in this.posts)
                {
                    if (p.value.Equals(first.value))
                        firstEqual.Add(p);
                    else
                        other.Add(p);
                }

                if(other.Count == 0)
                {
                    this.posts = null;
                    return first.value;
                }

                if (firstEqual.Count >= other.Count)
                {
                    this.posts = other;
                    return first.value;
                }
                else
                {
                    this.posts = firstEqual;
                    return other.ElementAt(0).value;
                }
                    

            }
            set { } }
        public List<PostParameter<T>> posts { get; set; }
    }
}
