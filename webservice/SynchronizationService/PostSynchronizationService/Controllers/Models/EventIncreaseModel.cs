using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostSynchronizationService.Controllers.Models
{
    public class EventIncreaseModel
    {
        public string device { get; set; } // по нему ищу IDPost
        public int idEventPost { get; set; }
        public DateTime dtime { get; set; }
        public string eventKindCode { get; set; }
        public int amount { get; set; }
        public int m10 { get; set; }
        public int b10 { get; set; }
        public int b50 { get; set; }
        public int b100 { get; set; }
        public int b200 { get; set; }
        public int balance { get; set; }
        public int idPostSession { get; set; }
    }
}
