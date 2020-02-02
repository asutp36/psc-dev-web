using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inspinia_MVC5.Models;

namespace Inspinia_MVC5.Helpers
{
    public class InfoPost
    {
        public int Balance { get; set; }

        public string Function { get; set; }

        public int State { get; set; }

        public Post Post { get; set; }

        public InfoPost(int Balance, string Function, int State, Post Post)
        {
            this.Balance = Balance;
            this.Function = Function;
            this.State = State;
            this.Post = Post;
        }
    }
}