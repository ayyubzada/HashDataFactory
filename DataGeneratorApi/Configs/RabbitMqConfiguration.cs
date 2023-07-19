
namespace SharedLibrary.Configs;

public class RabbitMqConfiguration
{
    public static string ConfigName = "RabbitMq";

    public string HostName { get; set; }
    public string QueueName { get; set; }
}