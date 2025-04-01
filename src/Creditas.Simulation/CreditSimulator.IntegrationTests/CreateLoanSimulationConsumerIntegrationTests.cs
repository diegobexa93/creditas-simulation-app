using CreditSimulator.BuildingBlocks.Messaging.Events;
using CreditSimulatorService.Worker.Consumer;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Testcontainers.RabbitMq;

namespace CreditSimulator.IntegrationTests
{
    public class CreateLoanSimulationConsumerIntegrationTests : IAsyncLifetime
    {
        private RabbitMqContainer _rabbit;
        private IBusControl _bus;
        private readonly ConcurrentBag<object> _receivedMessages = new();

        #region [ Initialize And Dispose]
        public async Task InitializeAsync()
        {
            _rabbit = new RabbitMqBuilder()
                .WithUsername("guest")
                .WithPassword("guest")
                .Build();

            await _rabbit.StartAsync();

            _bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost", _rabbit.GetMappedPublicPort(5672), "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // Consumer principal
                cfg.ReceiveEndpoint("generate_simulation", e =>
                {
                    var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CreateLoanSimulationConsumer>();
                    e.Consumer(() => new CreateLoanSimulationConsumer(logger));
                });

                // Captura os eventos publicados para validação no teste
                cfg.ReceiveEndpoint("loan_simulation_email", e =>
                {
                    e.Handler<LoanSimulationEmailEvent>(ctx =>
                    {
                        _receivedMessages.Add(ctx.Message);
                        return Task.CompletedTask;
                    });
                });

                cfg.ReceiveEndpoint("loan_simulation_persistence", e =>
                {
                    e.Handler<LoanSimulationPersistenceEvent>(ctx =>
                    {
                        _receivedMessages.Add(ctx.Message);
                        return Task.CompletedTask;
                    });
                });
            });

            await _bus.StartAsync();
        }

        public async Task DisposeAsync()
        {
            if (_bus != null)
                await _bus.StopAsync();

            if (_rabbit != null)
                await _rabbit.DisposeAsync();
        }

        #endregion

        [Fact]
        public async Task ShouldSimulateAndPublishBothEvents()
        {
            var batchId = Guid.NewGuid();

            await _bus.Publish(new LoanSimulationGenerateEvent
            {
                BatchId = batchId,
                Email = "cliente@test.com",
                ValueLoan = 10000,
                PaymentTerm = 12,
                BirthDate = DateTime.Today.AddYears(-30)
            });

            await Task.Delay(5000);

            var emailEvent = _receivedMessages.OfType<LoanSimulationEmailEvent>().FirstOrDefault();
            var persistEvent = _receivedMessages.OfType<LoanSimulationPersistenceEvent>().FirstOrDefault();

            Assert.NotNull(emailEvent);
            Assert.NotNull(persistEvent);
            Assert.Equal("cliente@test.com", emailEvent.Email);
            Assert.Equal("cliente@test.com", persistEvent.Email);
            Assert.Equal(batchId.ToString(), emailEvent.BatchId.ToString());
            Assert.Equal(batchId.ToString(), persistEvent.BatchId.ToString());
        }
    }
}
