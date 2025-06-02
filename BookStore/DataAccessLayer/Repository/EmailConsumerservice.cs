using DataAccessLayer.Interface;
using DataAccessLayer.Modal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class EmailConsumerservice : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public EmailConsumerservice(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new RabbitMQ.Client.ConnectionFactory()
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "emailQueue", durable: false, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var jsonMessage = Encoding.UTF8.GetString(body);
            var emailMessage = JsonSerializer.Deserialize<EmailMessage>(jsonMessage);

            Console.WriteLine($"[RabbitMQ] Received email message: {jsonMessage}");

            
            using (var scope = _serviceProvider.CreateScope())
            {
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                userRepository.SendEmail(emailMessage.ToEmail, emailMessage.Subject, emailMessage.Body);
            }
        };

        channel.BasicConsume(queue: "emailQueue", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
