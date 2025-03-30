using CreditSimulatorService.Application.Validation;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CreditSimulatorService.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddAutoMapper(typeof(UserProfile).Assembly);
            services.AddValidatorsFromAssemblyContaining<CreateLoanSimulationBatchCommandValidator>();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

            return services;
        }
    }
}
