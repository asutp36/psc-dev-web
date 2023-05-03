using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace MobileIntegration.Controllers.Supplies
{
    public class CacheSingleton
    {
        private static CacheSingleton Instance;
        private static object locker = new object();

        private readonly MemoryCache _cache;

        private CacheSingleton()
        {
            _cache = new MemoryCache("main cache");
        }

        public static CacheSingleton GetCache()
        {
            if (Instance == null)
            {
                lock (locker)
                {
                    if (Instance == null)
                    {
                        Instance = new CacheSingleton();
                    }
                }
            }

            return Instance;
        }

        public bool AddItem(string key, object value)
        {
            return _cache.Add(key, value, new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(7) });
        }
    }
}