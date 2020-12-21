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
        private Dictionary<string, string> _cache;
        private object locker;

        public CacheService ()
        {
            _cache = new Dictionary<string, string>();
            locker = new object();
        }

        public async Task UpdateCacheAsync()
        {
            while (true)
            {
                lock (locker)
                {
                    HttpResponse response = HttpSender.SendGet("https://yandex.com/time/sync.json?geo=213");

                    _cache.Remove("13-1");
                    _cache.Add("13-1", response.ResultMessage);
                }

                await Task.Delay(3000);
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

                    //_cache.Set("13-1", response.ResultMessage);
                    _cache.Remove("13-1");
                    _cache.Add("13-1", response.ResultMessage);
                    lastConnection = response.ResultMessage;
                }

                return lastConnection;
            }
        }
    }
}
