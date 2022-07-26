using LoyalityService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LoyalityService.Services
{
    public interface IPostRCCaller
    {
        Task<HttpResponseMessage> StartPostAsync(StartPostParameters parameters);
    }
}
