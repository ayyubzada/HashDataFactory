using System.Text;
using DataGeneratorApi.Services.Abstractions;
using RabbitMQ.Client;
using System.Security.Cryptography;
using System;
using Microsoft.Extensions.Options;
using DataPersistenceLayer.Repositories.Abstractions;
using SharedLibrary.Configs;

namespace DataGeneratorApi.Services;

public class HashService : IHashService
{
    private readonly ILogger<HashService> _logger;
    private readonly IHashRecordRepository _hashRecordRepository;
    private readonly IModel _channel;

    public HashService(
        ILogger<HashService> logger,
        IOptions<RabbitMqConfiguration> rabbitMqOptions,
        IHashRecordRepository hashRecordRepository)
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

    public async Task<GroupedDataByDate> GetGroupedDataByDates()
    {
        try
        {
            var sinceDate = DateTime.UtcNow.AddYears(-1);
            var groupedData = await _hashRecordRepository.GetLatestRecordsAsGroupedByDateAsync(sinceDate);

            var hashList = groupedData
                .Select(h => new CountOfDataInTime(h.Key.ToString("yyyy-MM-dd"), h.Count()));

            _logger.LogInformation($"{hashList.Count()} grouped data was fetched");
            return new GroupedDataByDate(hashList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during fetching the grouped data");
            return null;
        }
    }

    public async Task<int> GenerateAndSendHashesAsync(int numberOfHashes)
    {
        var successCount = 0;
        for (var i = 1; i <= numberOfHashes; i++)
        {
            try
            {
                var hash = GetRandomSha1Hash();
                var body = Encoding.UTF8.GetBytes(hash);

                _channel.BasicPublish(exchange: "", routingKey: "hashQueue", basicProperties: null, body: body);
                _logger.LogInformation($"{i}th Hash({hash}) was generated successfully!");
                successCount++;
                await Task.Yield();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during generation of {i}th Hash");
            }
        }

        return successCount;
    }

    private string GetRandomSha1Hash()
    {
        byte[] data = new byte[20]; // SHA1 uses 160-bit (20 bytes) numbers
        using var rng = new RNGCryptoServiceProvider();
        rng.GetBytes(data);

        using var sha1 = new SHA1Managed();
        byte[] hashBytes = sha1.ComputeHash(data);

        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}