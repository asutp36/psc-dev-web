using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangoAPIService.Services
{
    interface IMangoAPICaller
    {
        void CallHangup(string call_id);
    }
}
