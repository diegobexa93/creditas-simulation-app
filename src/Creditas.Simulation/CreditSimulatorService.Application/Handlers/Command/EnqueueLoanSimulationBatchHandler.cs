using CreditSimulatorService.Application.Commands;
using CreditSimulatorService.Application.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;
using Polly;

namespace CreditSimulatorService.Application.Handlers.Command
{
    public class EnqueueLoanSimulationBatchHandler : IRequestHandler<CreateLoanSimulationBatchCommand, Guid>
    {
        private readonly IMessageQueuePublisher _publisher;
        private readonly ILogger<EnqueueLoanSimulationBatchHandler> _logger;

        private readonly AsyncPolicy _publishPolicy;


        public EnqueueLoanSimulationBatchHandler(IMessageQueuePublisher publisher,
                                                  ILogger<EnqueueLoanSimulationBatchHandler> logger)
        {
            _publisher = publisher;
            _logger = logger;

            _publishPolicy = Policy.Handle<Exception>()
                                   .WaitAndRetryAsync(
                                       retryCount: 3, // Tentar no máximo 3 vezes
                                       sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(300),
                                       onRetry: (exception, timeSpan, retryCount, context) =>
                                       {
                                           var email = context["BatchId"]?.ToString();
                                           _logger.LogWarning(exception,
                                               "Retry {Retry} falhou ao publicar para {BatchId}, aguardando {Delay}...",
                                               retryCount, email, timeSpan);
                                       }
                                   );
        }

        public async Task<Guid> Handle(CreateLoanSimulationBatchCommand request, CancellationToken cancellationToken)
        {
            var tasks = request.Simulations.Select(x =>
            {
                x.BatchId = request.BatchId;

                var context = new Context
                {
                    ["BatchId"] = x.BatchId
                };

                return _publishPolicy.ExecuteAsync(
                    async (ctx, ct) => await _publisher.PublishAsync(x, ct),
                    context,
                    cancellationToken
                );
            });

            await Task.WhenAll(tasks);

            return request.BatchId;
        }
    }
}
