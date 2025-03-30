using CreditSimulatorService.Application.Messaging;
using CreditSimulatorService.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CreditSimulatorService.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            //Conexão com o banco de dados;

            //Inicialização do RabbitMQ
            services.AddSingleton<RabbitMqPublisher>();
            services.AddSingleton<IMessageQueuePublisher>(sp => sp.GetRequiredService<RabbitMqPublisher>());

            services.AddHostedService<RabbitMqInitializerHostedService>();

            return services;
        }
    }
}
