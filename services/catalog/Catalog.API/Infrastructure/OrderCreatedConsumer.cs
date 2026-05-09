using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Contracts;
using System.Text;
using System.Text.Json;

namespace Catalog.API.Infrastructure
{
    public class OrderCreatedConsumer : BackgroundService
    {
        protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
        {
            IConnection? connection = null;

            while (connection == null && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine(
                        "Connecting to RabbitMQ...");

                    var factory = new ConnectionFactory
                    {
                        HostName = "rabbitmq"
                    };

                    connection =
                        await factory.CreateConnectionAsync(
                            cancellationToken: stoppingToken);

                    Console.WriteLine(
                        "Connected to RabbitMQ");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"RabbitMQ unavailable: {ex.Message}");

                    await Task.Delay(
                        5000,
                        stoppingToken);
                }
            }

            if (connection == null)
                return;

            var channel =
                await connection.CreateChannelAsync(
                    cancellationToken: stoppingToken);

            await channel.QueueDeclareAsync(
                queue: "orders.created",
                durable: false,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);

            var consumer =
                new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var body = ea.Body.ToArray();

                var json =
                    Encoding.UTF8.GetString(body);

                var message =
                    JsonSerializer.Deserialize<OrderCreatedEvent>(
                        json);

                Console.WriteLine(
                    $"Received event: {message!.OrderId}");

                await Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(
                queue: "orders.created",
                autoAck: true,
                consumer: consumer,
                cancellationToken: stoppingToken);

            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
    }
}
