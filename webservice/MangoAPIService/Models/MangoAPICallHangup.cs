using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangoAPIService.Models
{
    public class MangoAPICallHangup
    {
        // Просто наш внутренний идентификатор:
        // используется для связи команды с её результатом
        public string command_id { get; set; }
        // ID звонка
        public string call_id { get; set; }
    }
}
