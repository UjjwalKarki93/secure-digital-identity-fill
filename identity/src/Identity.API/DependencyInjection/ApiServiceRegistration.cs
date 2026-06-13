using Identity.API.Filters;
using MediatR;

namespace Identity.API.DependencyInjection;

public static class ApiServiceRegistration
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
