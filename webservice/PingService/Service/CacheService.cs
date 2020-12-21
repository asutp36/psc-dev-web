using Microsoft.Extensions.Caching.Memory;
using PingService.Supplies;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PingService.Service
{
    class CacheService
    {
        private IMemoryCache _cache;
        private object locker;

        public CacheService (IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task UpdateCacheAsync()
        {
            while (true)
            {
                lock (locker)
                {
                    HttpResponse response = HttpSender.SendGet("https://yandex.com/time/sync.json?geo=213");

                    _cache.Set("13-1", response.ResultMessage);
                }

                await Task.Delay(10000);
            }
        }

        public string GetLastPing()
        {
            lock (locker)
            {
                string lastConnection = null;
                if (!_cache.TryGetValue("13-1", out lastConnection))
                {
                    HttpResponse response = HttpSender.SendGet("https://yandex.com/time/sync.json?geo=213");

                    _cache.Set("13-1", response.ResultMessage);
                    lastConnection = response.ResultMessage;
                }

                return lastConnection;
            }
        }
    }
}
