using System;
using System.Collections.Generic;
using System.Text;

namespace MobileAppWasteSender.Models
{
    class UnstoppedSessionModel
    {
        public string CardNum { get; set; }
        public string Post { get; set; }
        public DateTime DTimeStart { get; set; }
        public string Guid { get; set; }

        public override string ToString()
        {
            return $"Карта: {this.CardNum}, пост: {this.Post}, мойка началась в {DTimeStart:HH:mm:ss dd.MM.yyyy}";
        }
    }
}
