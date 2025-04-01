using CreditSimulatorService.Infrastructure.Persistence.Mongo;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using Testcontainers.MongoDb;
using Testcontainers.RabbitMq;


namespace CreditSimulator.EndToEndTests
{
    public class LoanSimulationApiE2ETests : IAsyncLifetime
    {
        private RabbitMqContainer _rabbit;
        private MongoDbContainer _mongo;
        private HttpClient _client;
        private LoanSimulationRepository _repository;

        #region [ Initialize and Dispose ]
        public async Task InitializeAsync()
        {
            _rabbit = new RabbitMqBuilder().WithUsername("guest").WithPassword("guest").Build();
            _mongo = new MongoDbBuilder().WithUsername("admin").WithPassword("admin").Build();

            await _rabbit.StartAsync();
            await _mongo.StartAsync();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["MongoDb:ConnectionString"] = _mongo.GetConnectionString(),
                    ["MongoDb:Database"] = "testdb",
                    ["RabbitMq:Host"] = "localhost",
                    ["RabbitMq:Port"] = _rabbit.GetMappedPublicPort(5672).ToString(),
                    ["RabbitMq:Username"] = "guest",
                    ["RabbitMq:Password"] = "guest"
                })
                .Build();

            _repository = new LoanSimulationRepository(configuration);

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
        }

        public async Task DisposeAsync()
        {
            await _mongo.DisposeAsync();
            await _rabbit.DisposeAsync();
        }

        #endregion

        [Fact]
        public async Task ShouldSimulateAndPersistLoanInMongo_ViaApi()
        {
            var batchId = Guid.NewGuid();

            var payload = new
            {
                Simulations = new[]
                {
                    new
                    {
                        Email = "cliente@teste.com",
                        ValueLoan = 10000,
                        PaymentTerm = 12,
                        BirthDate = DateTime.Today.AddYears(-20)
                    }
                }
            };

            var response = await _client.PostAsJsonAsync("/api/LoanSimulation/CreateBatch", payload);
            response.EnsureSuccessStatusCode();

            await Task.Delay(5000);
            var result = await _repository.GetSimulationByEmailPagedAsync("cliente@teste.com", 1, 10, CancellationToken.None);


            // Assert
            Assert.NotEmpty(result.Items);
            var doc = result.Items.First();
            Assert.Equal("cliente@teste.com", doc.Email);
            Assert.Equal(10000, doc.ValueLoan);
            Assert.Equal(12, doc.PaymentTerm);
        }
    }
}
