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
                                       retryCount: 3, // Tentar no máximo 3 vezes
                                       sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(300),
                                       onRetry: (exception, timeSpan, retryCount, context) =>
                                       {
                                           var batchId = context["BatchId"]?.ToString();
                                           _logger.LogWarning(exception,
                                               "Retry {Retry} falhou ao publicar para {BatchId}, aguardando {Delay}...",
                                               retryCount, batchId, timeSpan);
                                       }
                                   );
        }

        public async Task<Guid> Handle(CreateLoanSimulationBatchCommand request, CancellationToken cancellationToken)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:loan_simulations"));

            var tasks = request.Simulations.Select(x =>
            {
                x.BatchId = request.BatchId;

                var context = new Context { ["BatchId"] = x.BatchId };

                return _publishPolicy.ExecuteAsync(
                    async (ctx, ct) => await endpoint.Send(x, ct),
                    context,
                    cancellationToken
                );
            });

            await Task.WhenAll(tasks);

            return request.BatchId;
        }
    }
}
