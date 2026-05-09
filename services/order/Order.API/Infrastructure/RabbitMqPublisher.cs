using RabbitMQ.Client;
using Shared.Contracts;
using System.Text;
using System.Text.Json;

namespace Order.API.Infrastructure
{
    public class RabbitMqPublisher
    {
        public async Task PublishAsync(
            OrderCreatedEvent message)
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq"
            };

            await using var connection =
                await factory.CreateConnectionAsync();

            await using var channel =
                await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "orders.created",
                durable: false,
                exclusive: false,
                autoDelete: false);

            var body = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(message));

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "orders.created",
                body: body);

            Console.WriteLine(
                $"Published event: {message.OrderId}");
        }
    }
}
