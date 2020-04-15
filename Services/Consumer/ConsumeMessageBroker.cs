using System;
using System.Text;
using System.Text.Json;

using BooksApi.Services.Consumer.Domain;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BooksApi.Services.Consumer {

    public static class ConsumeMessageBroker {

        public static void Consume() {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel()) {
                channel.QueueDeclare(queue: "hello",
                    durable : false,
                    exclusive : false,
                    autoDelete : false,
                    arguments : null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) => {

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    var book = JsonSerializer.Deserialize<Book>(message);

                };
                channel.BasicConsume(queue: "hello",
                    autoAck : true,
                    consumer : consumer);

            }
        }
    }
}