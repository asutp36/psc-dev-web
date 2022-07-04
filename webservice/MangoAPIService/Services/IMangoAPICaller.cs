using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangoAPIService.Services
{
    public interface IMangoAPICaller
    {
        void CallHangupAsync(string call_id);
    }
}
