using MangoAPIService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MangoAPIService.Services
{
    public interface ILoyalityServiceCaller
    {
        public void HandleNewCallAsync(CallCacheModel call);
    }
}
