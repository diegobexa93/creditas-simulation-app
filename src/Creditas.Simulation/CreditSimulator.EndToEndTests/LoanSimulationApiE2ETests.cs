using CreditSimulator.BuildingBlocks.Messaging.Events;
using CreditSimulatorService.Domain.Mongo;
using CreditSimulatorService.Infrastructure.Persistence.Mongo;
using CreditSimulatorService.Worker.Consumer;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using Testcontainers.MongoDb;
using Testcontainers.RabbitMq;

namespace CreditSimulator.EndToEndTests
{
    public class LoanSimulationApiE2ETests : IAsyncLifetime
    {
        private RabbitMqContainer _rabbit;
        private MongoDbContainer _mongo;
        private IBusControl _bus;
        private LoanSimulationRepository _repository;
        private HttpClient _client;
        private readonly ConcurrentBag<object> _receivedMessages = new();

        public async Task InitializeAsync()
        {

            #region [ MongoDb and Repository Config ]

            _mongo = new MongoDbBuilder().WithUsername("admin").WithPassword("admin").Build();

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

            #endregion


            #region [ RabbitMQ Config ]

            _rabbit = new RabbitMqBuilder().WithUsername("guest").WithPassword("guest").Build();

            await _rabbit.StartAsync();

            _bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost", _rabbit.GetMappedPublicPort(5672), "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // Consumer principal
                cfg.ReceiveEndpoint("loan_simulations", e =>
                {
                    var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CreateLoanSimulationConsumer>();
                    e.Consumer(() => new CreateLoanSimulationConsumer(logger));

                    e.Handler<LoanSimulationGenerateEvent>(ctx =>
                    {
                        _receivedMessages.Add(ctx.Message);
                        return Task.CompletedTask;
                    });
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
                    var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<LoanSimulationPersistenceConsumer>();
                    e.Consumer(() => new LoanSimulationPersistenceConsumer(logger, _repository));

                    e.Handler<LoanSimulationPersistenceEvent>(ctx =>
                    {
                        _receivedMessages.Add(ctx.Message);
                        return Task.CompletedTask;
                    });
                });
            });

            await _bus.StartAsync();

            #endregion




            #region [ API Cconfig ]

            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((_, config) =>
                    {
                        config.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["MongoDb:ConnectionString"] = _mongo.GetConnectionString(),
                            ["MongoDb:Database"] = "testdb",
                            ["RabbitMq:Host"] = "localhost",
                            ["RabbitMq:Port"] = _rabbit.GetMappedPublicPort(5672).ToString(),
                            ["RabbitMq:Username"] = "guest",
                            ["RabbitMq:Password"] = "guest"
                        });
                    });
                });

            _client = factory.CreateClient();

            #endregion
        }

        public async Task DisposeAsync()
        {
            await _bus.StopAsync();
            await _mongo.DisposeAsync();
            await _rabbit.DisposeAsync();
        }

        [Fact]
        public async Task ShouldSimulateAndPersistLoanInMongo_ViaApi()
        {
            var payload = new
            {
                Simulations = new[]
                {
                new
                {
                    Email = "cliente@teste.com",
                    ValueLoan = 10000,
                    PaymentTerm = 12,
                    BirthDate = DateTime.Today.AddYears(-30)
                }
            }
            };

            // Act: chama o endpoint da API que publica o evento no Rabbit
            var response = await _client.PostAsJsonAsync("/api/LoanSimulation/CreateBatch", payload);
            response.EnsureSuccessStatusCode();

            IEnumerable<LoanSimulationDocument> documents = null;
            // Aguarda persistência no Mongo
            await Task.Delay(5000);

            var result = await _repository.GetSimulationByEmailPagedAsync("cliente@teste.com", 1, 10, CancellationToken.None);
            if (result.TotalCount > 0)
            {
                documents = result.Items;
            }

            // Assert
            Assert.NotNull(documents);
            Assert.Equal("cliente@teste.com", documents.First().Email);
            Assert.Equal(10000, documents.First().ValueLoan);
            Assert.Equal(12, documents.First().PaymentTerm);
        }
    }
}
