using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Helpers
{
    public class IncreaseBalanceOnPostClass
    {
        public string Post {get; set;}
        public int Amount { get; set; }

        public string DTime { get; set; }

        public string Login { get; set; }

        public IncreaseBalanceOnPostClass (string Post, int Amount, string DTime, string Login)
        {
            this.Post = Post;
            this.Amount = Amount;
            this.DTime = DTime;
            this.Login = Login;
        }
    }
}