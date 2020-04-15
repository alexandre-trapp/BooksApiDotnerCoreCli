using System;
using System.Text;
using System.Text.Json;

using BooksApi.Services.Consumer.Domain;

using Microsoft.Extensions.Logging;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BooksApi.Services.Consumer {

    public static class ConsumeMessageBroker {

        public static void Consume(ILogger logger) {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "hello",
                durable : false,
                exclusive : false,
                autoDelete : false,
                arguments : null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) => {

                try {

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    logger.LogInformation("message received.", message);

                    var book = JsonSerializer.Deserialize<Book>(message);

                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception e) {

                    logger.LogError("error receive message.", e);
                    channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            channel.BasicConsume(queue: "hello",
                autoAck : false,
                consumer : consumer);

            logger.LogInformation("message consumed.");
        }
    }
}