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
    private readonly IHashRecordRepository _hashRecordRepository;
    private readonly IModel _channel;
    private const int threadCount = 4;

    public DataProcessorService(
        ILogger<DataProcessorService> logger,
        IHashRecordRepository hashRecordRepository,
        IOptions<RabbitMqConfiguration> rabbitMqOptions)
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
    }

    public async Task StartProcessing()
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            await SaveHashRecord(message);
        };

        _channel.BasicConsume(queue: "hashQueue", autoAck: true, consumer: consumer);
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

            await _hashRecordRepository.AddAsync(hashRecord);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during writing");
        }
    }
}