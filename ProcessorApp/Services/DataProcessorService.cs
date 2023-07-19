using System.Text;
using DataPersistenceLayer.Entities;
using DataPersistenceLayer.Repositories;
using DataPersistenceLayer.Repositories.Abstractions;
using Microsoft.Extensions.Options;
using ProcessorApp.Services.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedLibrary.Configs;

namespace ProcessorApp.Services;

public class DataProcessorService : IDataProcessorService
{
    private readonly ILogger<DataProcessorService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHashRecordRepository _hashRecordRepository;
    private readonly IModel _channel;
    private const int threadCount = 4;

    public DataProcessorService(
        ILogger<DataProcessorService> logger,
        IHashRecordRepository hashRecordRepository,
        IOptions<RabbitMqConfiguration> rabbitMqOptions,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _hashRecordRepository = hashRecordRepository;

        var factory = new ConnectionFactory() { HostName = rabbitMqOptions.Value.HostName };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.QueueDeclare(
            queue: rabbitMqOptions.Value.QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        _serviceProvider = serviceProvider;
    }

    public async Task StartProcessing()
    {
        try
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                await SaveHashRecord(message);
            };

            _channel.BasicQos(0, 4, false);

            Parallel.For(0, threadCount, _ =>
            {
                _channel.BasicConsume(queue: "hashQueue", autoAck: false, consumer: consumer);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occured during lsitening to the RabbitMQ");
        }

    }

    private async Task SaveHashRecord(string message)
    {
        try
        {
            _logger.LogInformation($"Message({message}) accepted");
            var hashRecord = new HashRecord
            {
                Date = DateTime.UtcNow,
                Sha1 = message
            };

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var hashRecordRepository = scope.ServiceProvider.GetRequiredService<IHashRecordRepository>();

                await hashRecordRepository.AddAsync(hashRecord);
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during writing");
        }
    }
}