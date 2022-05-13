using System;
using System.Collections.Generic;
using System.Text;

namespace MobileAppWasteSender.Models
{
    class MobileAppWasteModel
    {
        public string time_send { get; set; }
        public string hash { 
            get {
                return CryptHash.GetHashCode(this.time_send);
            } 
            set { } }
        public string wash_id { get; set; }
        public string operation_time { get; set; }
        public string card { get; set; }
        public int value { get; set; }
        public Guid guid { get; set; }
    }
}
