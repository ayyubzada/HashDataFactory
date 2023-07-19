using System.Text;
using DataPersistenceLayer.Entities;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IModel _channel;
    private const int threadCount = 4;

    public DataProcessorService(
        ILogger<DataProcessorService> logger,
        IUnitOfWork unitOfWork,
        IOptions<RabbitMqConfiguration> rabbitMqOptions)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;

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

            await ProcessMessage(message);
        };

        _channel.BasicConsume(queue: "hashQueue", autoAck: true, consumer: consumer);
    }

    private async Task ProcessMessage(string message)
    {
        var tasks = new List<Task>();

        for (int i = 0; i < threadCount; i++)
        {
            tasks.Add(Task.Run(() => SaveHashRecord(message)));
        }

        await Task.WhenAll(tasks);
    }

    private async Task SaveHashRecord(string message)
    {
        _logger.LogInformation($"Message({message}) accepted");
        var hashRecord = new HashRecord
        {
            Date = DateTime.UtcNow,
            Sha1 = message
        };

        var hashRecordRepository = _unitOfWork.GetRepository<IHashRecordRepository>();
        await hashRecordRepository.AddAsync(hashRecord);
        await _unitOfWork.SaveChangesAsync();
    }
}