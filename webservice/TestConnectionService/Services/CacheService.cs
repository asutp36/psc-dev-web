using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestConnectionService.Controllers.Supplies;

namespace TestConnectionService.Services
{
    public class CacheService
    {
        private IMemoryCache _cache;
        private object locker;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
            locker = new object();
        }

        public string GetLastConnectionAsync(string post)
        {
            lock (locker)
            {
                string lastConnection = null;
                if (!_cache.TryGetValue(post, out lastConnection))
                {
                    HttpResponse response = HttpSender.SendGet("https://yandex.com/time/sync.json?geo=213");

                    _cache.Set(post, response.ResultMessage);
                    lastConnection = response.ResultMessage;
                }

                return lastConnection;
            }
        }

        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                lock (locker)
                {
                    HttpResponse response = HttpSender.SendGet("https://yandex.com/time/sync.json?geo=213");

                    _cache.Set("13-1", response.ResultMessage);
                }

                await Task.Delay(10000);
            }
        }
    }
}
