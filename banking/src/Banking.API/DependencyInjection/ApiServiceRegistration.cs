using Banking.API.ExternalServices;
using Banking.API.Filters;
using Banking.Application.Interfaces;
using MediatR;

namespace Banking.API.DependencyInjection;

public static class ApiServiceRegistration
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<IVerificationNotifier, SignalRVerificationNotifier>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
