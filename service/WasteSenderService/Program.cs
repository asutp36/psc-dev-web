using WasteSenderService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WasteSeder>();
    })
    .Build();

await host.RunAsync();
