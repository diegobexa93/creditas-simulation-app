using CreditSimulatorService.Application.Interfaces;
using CreditSimulatorService.Domain.Mongo;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace CreditSimulatorService.Infrastructure.Persistence.Mongo
{
    public class LoanSimulationRepository : ILoanSimulationRepository
    {
        private readonly IMongoCollection<LoanSimulationDocument> _collection;

        public LoanSimulationRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["MongoDb:ConnectionString"]);
            var database = client.GetDatabase(configuration["MongoDb:Database"]);
            _collection = database.GetCollection<LoanSimulationDocument>("loan_simulations");
        }

        public Task InsertAsync(LoanSimulationDocument document, CancellationToken cancellationToken)
            => _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
    }
}
