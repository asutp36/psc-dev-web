namespace WasteSenderService
{
    public class WasteSeder : BackgroundService
    {
        private readonly ILogger<WasteSeder> _logger;

        public WasteSeder(ILogger<WasteSeder> logger)
        {
            _logger = logger;
        }


        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            throw new Exception("test");
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, cancellationToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            base.StopAsync(cancellationToken);
        }
    }
}