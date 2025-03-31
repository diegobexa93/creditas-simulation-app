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

        public async Task<(IEnumerable<LoanSimulationDocument> Items, int TotalCount)> GetSimulationByBatchIdPagedAsync(Guid batchId, int pageNumber, 
            int pageSize, CancellationToken cancellationToken)
        {
            var filter = Builders<LoanSimulationDocument>.Filter.Eq(x => x.BatchId, batchId);

            var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

            var items = await _collection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            return (items, (int)totalCount);
        }

        public async Task<(IEnumerable<LoanSimulationDocument> Items, int TotalCount)> GetSimulationByEmailPagedAsync(string email, int pageNumber,
            int pageSize, CancellationToken cancellationToken)
        {
            var filter = Builders<LoanSimulationDocument>.Filter.Eq(x => x.Email, email);

            var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

            var items = await _collection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            return (items, (int)totalCount);
        }
    }
}
