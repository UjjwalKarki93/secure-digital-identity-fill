using Banking.Application.Interfaces;
using Banking.Infrastructure.ExternalServices;
using Banking.Infrastructure.Persistence;
using Banking.Infrastructure.Repositories;
using Banking.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BankingDb")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Banking database connection string is not configured.");

        services.AddDbContext<BankingDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IVerificationRepository, VerificationRepository>();
        services.AddScoped<IBankClientRepository, BankClientRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddSingleton<IHmacService, HmacService>();

        return services;
    }
}
