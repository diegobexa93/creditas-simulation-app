using Microsoft.Extensions.Hosting;

namespace CreditSimulatorService.Infrastructure.Messaging
{
    public class RabbitMqInitializerHostedService : IHostedService
    {
        private readonly RabbitMqPublisher _publisher;

        public RabbitMqInitializerHostedService(RabbitMqPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _publisher.InitAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
