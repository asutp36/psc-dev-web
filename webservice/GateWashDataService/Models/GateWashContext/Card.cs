using System;
using System.Collections.Generic;

#nullable disable

namespace GateWashDataService.Models.GateWashContext
{
    public partial class Card
    {
        public Card()
        {
            Sessions = new HashSet<Session>();
        }

        public int Idcard { get; set; }
        public string CardNum { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
    }
}
