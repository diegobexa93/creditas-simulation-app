using CreditSimulatorService.Application.Commands;
using CreditSimulatorService.Application.Messaging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CreditSimulatorService.Infrastructure.Messaging
{
    public class RabbitMqPublisher : IAsyncDisposable, IMessageQueuePublisher
    {
        private readonly RabbitMqConfiguration _options;
        private IConnection _connection = default!;
        private IChannel _channel = default!;

        private const string ExchangeName = "loan_simulation_exchange";
        private const string QueueName = "loan_simulations";
        private const string RoutingKey = "simulate.loan";

        public RabbitMqPublisher(IOptions<RabbitMqConfiguration> options)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                AutomaticRecoveryEnabled = true
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Direct, durable: true, cancellationToken: cancellationToken);

            await _channel.QueueDeclareAsync(queue: QueueName,
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false,
                                             cancellationToken: cancellationToken);

            await _channel.QueueBindAsync(queue: QueueName,
                                          exchange: ExchangeName,
                                          routingKey: RoutingKey,
                                          cancellationToken: cancellationToken);
        }

        public async Task PublishAsync(CreateLoanSimulationCommand command, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(command);
            var body = Encoding.UTF8.GetBytes(json);

            await _channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: RoutingKey,
                body: body,
                cancellationToken: cancellationToken
            );
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel is not null)
                await _channel.CloseAsync();

            if (_connection is not null)
                await _connection.CloseAsync();
        }
    }
}
