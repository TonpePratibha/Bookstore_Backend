using DataAccessLayer.Interface;
using DataAccessLayer.Modal;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
   public class RabitmqProducer:IRabitmqProducer
    {

       
        
            private readonly IConfiguration _configuration;
            private readonly RabbitMQ.Client.ConnectionFactory _factory;

            public RabitmqProducer(IConfiguration configuration)
            {
                _configuration = configuration;
                _factory = new RabbitMQ.Client.ConnectionFactory()
                {
                    HostName = _configuration["RabbitMQ:HostName"] ?? "localhost"
                };
            }

            public void PublishEmailMessage(EmailMessage emailMessage)
            {
                using var connection = _factory.CreateConnection();    //connection to rabitmq server
                using var channel = connection.CreateModel();          //viirtual connection to send msgs

                channel.QueueDeclare(queue: "emailQueue", durable: false, exclusive: false, autoDelete: false);

                var json = JsonSerializer.Serialize(emailMessage);
                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish(exchange: "", routingKey: "emailQueue", basicProperties: null, body: body);
            }
        }

    
}
