using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PostBackgroundServices
{
    class MobileAppWasteSender : BackgroundService
    {
        private readonly ILogger<MobileAppWasteSender> _logger;

        public MobileAppWasteSender(ILogger<MobileAppWasteSender> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
