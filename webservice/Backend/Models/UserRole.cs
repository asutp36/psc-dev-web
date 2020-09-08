using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class UserRole
    {
        public int Iduser { get; set; }
        public int Idrole { get; set; }

        public virtual Roles IdroleNavigation { get; set; }
        public virtual Users IduserNavigation { get; set; }
    }
}
