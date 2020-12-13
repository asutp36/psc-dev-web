using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestConnectionService.Controllers.Supplies
{
    public class CacheService
    {
        private IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GetLastConnection(string post)
        {
            string lastConnection = null;
            if (!_cache.TryGetValue(post, out lastConnection))
            {
                UpdateCache();
                _cache.TryGetValue(post, out lastConnection);
            }

            return lastConnection;
        }

        public void UpdateCache()
        {

        }

    }
}
