using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspinia_MVC5.Helpers
{
    public class ChangeFunctionOnPost
    {
        public string Post { get; set; }

        public string Function { get; set; }

        public string DTime { get; set; }

        public string Login { get; set; }

        public ChangeFunctionOnPost(string Post, string Function, string DTime, string Login)
        {
            this.Post = Post;
            this.Function = Function;
            this.DTime = DTime;
            this.Login = Login;
        }
    }
}