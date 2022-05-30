using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotNotificationService.Models
{
    public class UpdateResult
    {
        public bool ok { get; set; }
        public List<Update> result { get; set; }
    }
}
