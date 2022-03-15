﻿using System;
using System.Collections.Generic;

#nullable disable

namespace MobileAppWasteSender.Models.WashCompany
{
    public partial class DeviceType
    {
        public DeviceType()
        {
            Devices = new HashSet<Device>();
        }

        public int IddeviceType { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Device> Devices { get; set; }
    }
}