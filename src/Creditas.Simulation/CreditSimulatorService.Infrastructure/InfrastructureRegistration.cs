﻿using CreditSimulatorService.Application.Interfaces;
using CreditSimulatorService.Infrastructure.Persistence.Mongo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CreditSimulatorService.Infrastructure
{
    public static class InfrastructureRegistration
    {
        public static IServiceCollection AddInfrastructureRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            //Conexão com o banco de dados do mongo para buscar os resultados;
            services.AddSingleton<ILoanSimulationRepository, LoanSimulationRepository>();


            return services;
        }
    }
}
