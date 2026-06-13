using Identity.Application.Interfaces;
using Identity.Infrastructure.ExternalServices;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentityDb")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Identity database connection string is not configured.");

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IKycRepository, KycRepository>();
        services.AddScoped<IConsentRepository, ConsentRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddSingleton<IHmacService, HmacService>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        services.AddHttpClient<IBankingWebhookClient, BankingWebhookClient>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
