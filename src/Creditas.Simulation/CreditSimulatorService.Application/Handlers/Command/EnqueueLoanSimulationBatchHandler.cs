using CreditSimulator.BuildingBlocks.Messaging.Events;
using CreditSimulatorService.Application.Commands;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Polly;

namespace CreditSimulatorService.Application.Handlers.Command
{
    public class EnqueueLoanSimulationBatchHandler : IRequestHandler<CreateLoanSimulationBatchCommand, Guid>
    {
        private readonly ILogger<EnqueueLoanSimulationBatchHandler> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly AsyncPolicy _publishPolicy;

        public EnqueueLoanSimulationBatchHandler(ISendEndpointProvider sendEndpointProvider,
                                                 ILogger<EnqueueLoanSimulationBatchHandler> logger)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _logger = logger;

            _publishPolicy = Policy.Handle<Exception>()
                                   .WaitAndRetryAsync(
                                       retryCount: 3,
                                       sleepDurationProvider: retry => TimeSpan.FromMilliseconds(300),
                                       onRetry: (exception, timeSpan, retryCount, context) =>
                                       {
                                           var email = context["Email"]?.ToString() ?? "unknown";
                                           var batchId = context["BatchId"]?.ToString() ?? "unknown";
                                           _logger.LogWarning(exception,
                                               "[Retry {Retry}] Falha ao publicar simulação para {Email} no Batch {BatchId}. Retentando em {Delay}...",
                                               retryCount, email, batchId, timeSpan);
                                       });
        }

        public async Task<Guid> Handle(CreateLoanSimulationBatchCommand request, CancellationToken cancellationToken)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:loan_simulations"));
            var batchId = Guid.NewGuid();

            var tasks = request.Simulations.Select(simulation =>
            {
                var simulationMessage = new LoanSimulationGenerateEvent
                {
                    BatchId = batchId,
                    ValueLoan = simulation.ValueLoan,
                    PaymentTerm = simulation.PaymentTerm,
                    BirthDate = simulation.BirthDate,
                    Email = simulation.Email
                };

                var context = new Context
                {
                    ["Email"] = simulation.Email,
                    ["BatchId"] = batchId.ToString()
                };

                return _publishPolicy.ExecuteAsync(
                    async (ctx, ct) => await endpoint.Send(simulationMessage, ct),
                    context,
                    cancellationToken
                );
            });

            await Task.WhenAll(tasks);

            _logger.LogInformation("Batch {BatchId} publicado com {Count} simulações.", batchId, request.Simulations.Count);

            return batchId;
        }
    }

}
