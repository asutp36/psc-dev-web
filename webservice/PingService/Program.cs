using PingService.Service;
using System;
using System.Threading.Tasks;

namespace PingService
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hello World!");

                MainAsync().Wait();
            }
            catch(Exception e)
            {
                Console.WriteLine("An exception occured: " + e.ToString());
            }
        }

        private static async Task MainAsync()
        {
            CacheService cacheService = new CacheService();

            await cacheService.UpdateCacheAsync();

            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(1000);

                Console.WriteLine(cacheService.GetLastPing());
            }

            Console.ReadKey();
        }
    }
}
