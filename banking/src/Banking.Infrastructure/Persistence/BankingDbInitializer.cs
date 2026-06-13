using Banking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Persistence;

public static class BankingDbInitializer
{
    public static async Task SeedAsync(BankingDbContext context)
    {
        if (await context.BankClients.AnyAsync())
            return;

        await context.BankClients.AddAsync(new BankClient
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            BankId = "BANK-001",
            Name = "Laxmi Sunrise Bank",
            HmacSecretKey = "shared-hmac-secret-key-change-in-production",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();
    }
}
