using System.Text;
using System.Text.Json;
using BooksApi.Models;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;

namespace BooksApi.Services {
    public static class SendBookMessageBroker {
        public static void Send(Book book, ILogger logger) 
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            string message = JsonSerializer.Serialize(book);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                routingKey: "hello",
                basicProperties: null,
                body: body);

            logger.LogInformation("message sent {0}", message);
        }
    }
}