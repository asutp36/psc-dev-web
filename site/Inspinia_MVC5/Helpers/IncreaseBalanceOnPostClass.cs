using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Helpers
{
    public class IncreaseBalanceOnPostClass
    {
        public int IdPost {get; set;}
        public int Sum { get; set; }

        public IncreaseBalanceOnPostClass (int IdPost, int Sum)
        {
            this.IdPost = IdPost;
            this.Sum = Sum;
        }
    }
}