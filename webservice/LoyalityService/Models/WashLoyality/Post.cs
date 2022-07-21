﻿using System;
using System.Collections.Generic;

#nullable disable

namespace LoyalityService.Models.WashLoyality
{
    public partial class Post
    {
        public int Idpost { get; set; }
        public string Name { get; set; }
        public int Idwash { get; set; }
        public int Iddevice { get; set; }
        public string Qrcode { get; set; }

        public virtual Device IddeviceNavigation { get; set; }
        public virtual Wash IdwashNavigation { get; set; }
    }
}
