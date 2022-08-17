using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangoAPIService.Services
{
    public interface IMangoAPICaller
    {
        Task CallHangupAsync(string call_id);
    }
}
