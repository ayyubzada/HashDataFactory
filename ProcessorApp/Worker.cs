using ProcessorApp.Services.Abstractions;

namespace ProcessorApp;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(
        ILogger<Worker> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dataProcessorService = scope.ServiceProvider.GetRequiredService<IDataProcessorService>();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await dataProcessorService.StartProcessing();
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
