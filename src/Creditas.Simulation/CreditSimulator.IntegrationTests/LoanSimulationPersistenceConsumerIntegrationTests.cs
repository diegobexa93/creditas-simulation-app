using CreditSimulator.BuildingBlocks.Messaging.Events;
using CreditSimulatorService.Domain.Mongo;
using CreditSimulatorService.Infrastructure.Persistence.Mongo;
using CreditSimulatorService.Worker.Consumer;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Testcontainers.MongoDb;
using Testcontainers.RabbitMq;

namespace CreditSimulator.IntegrationTests
{
    public class LoanSimulationPersistenceConsumerIntegrationTests : IAsyncLifetime
    {
        private MongoDbContainer _mongo;
        private RabbitMqContainer _rabbit;
        private IBusControl _bus;
        private LoanSimulationRepository _repository;

        #region [ Initialize and Dispose ]
        public async Task InitializeAsync()
        {
            // Start MongoDB container
            _mongo = new MongoDbBuilder()
                .WithUsername("admin")
                .WithPassword("admin")
                .Build();

            await _mongo.StartAsync();

            // Configure Mongo settings
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["MongoDb:ConnectionString"] = _mongo.GetConnectionString(),
                    ["MongoDb:Database"] = "testdb"
                })
                .Build();

            _repository = new LoanSimulationRepository(configuration);

            // Start RabbitMQ container
            _rabbit = new RabbitMqBuilder()
                .WithUsername("guest")
                .WithPassword("guest")
                .Build();

            await _rabbit.StartAsync();

            // Configure MassTransit with RabbitMQ
            _bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost", _rabbit.GetMappedPublicPort(5672), "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("loan_simulation_persistence", e =>
                {
                    var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<LoanSimulationPersistenceConsumer>();
                    e.Consumer(() => new LoanSimulationPersistenceConsumer(logger, _repository));
                });
            });

            await _bus.StartAsync();
        }

        public async Task DisposeAsync()
        {
            if (_bus != null)
                await _bus.StopAsync();

            if (_mongo != null)
                await _mongo.DisposeAsync();

            if (_rabbit != null)
                await _rabbit.DisposeAsync();
        }

        #endregion

        [Fact]
        public async Task ShouldPersistSimulationToMongo()
        {
            var batchId = Guid.NewGuid();

            await _bus.Publish(new LoanSimulationPersistenceEvent
            {
                Email = "cliente@teste.com",
                ValueLoan = 10000,
                PaymentTerm = 12,
                BirthDate = DateTime.Today.AddYears(-30),
                MonthlyInstallment = 859.20m,
                TotalToPay = 10310.40m,
                InterestPaid = 310.40m,
                BatchId = batchId,
                SimulatedAt = DateTime.UtcNow
            });

            IEnumerable<LoanSimulationDocument> documents = null;
            await Task.Delay(5000);

            var result = await _repository.GetSimulationByBatchIdPagedAsync(batchId, 1, 10, CancellationToken.None);
            if (result.TotalCount > 0)
            {
                documents = result.Items;
            }

            Assert.NotNull(documents);
            Assert.NotEmpty(documents);
            Assert.Equal("cliente@teste.com", documents.First().Email);
            Assert.Equal(batchId.ToString(), documents.First().BatchId.ToString());

        }
    }
}
