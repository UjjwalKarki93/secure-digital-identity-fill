using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

public static class IdentityDbInitializer
{
    public static async Task SeedAsync(IdentityDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var userId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var user = new User
        {
            Id = userId,
            Username = "citizen",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("nagarik123"),
            FullName = "Ram Bahadur Thapa",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var kyc = new KycData
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            UserId = userId,
            NationalId = "123-456-78901",
            DateOfBirth = "1990-05-15",
            Address = "Kathmandu, Bagmati Province, Nepal",
            PhoneNumber = "+977-9841234567",
            Email = "ram.thapa@example.com",
            VerifiedAt = DateTime.UtcNow
        };

        var user2Id = Guid.Parse("44444444-4444-4444-4444-444444444444");

        var user2 = new User
        {
            Id = user2Id,
            Username = "sita",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("nagarik123"),
            FullName = "Sita Devi Sharma",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var kyc2 = new KycData
        {
            Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            UserId = user2Id,
            NationalId = "987-654-32109",
            DateOfBirth = "1992-08-22",
            Address = "Pokhara, Gandaki Province, Nepal",
            PhoneNumber = "+977-9812345678",
            Email = "sita.sharma@example.com",
            VerifiedAt = DateTime.UtcNow
        };

        await context.Users.AddRangeAsync(user, user2);
        await context.KycData.AddRangeAsync(kyc, kyc2);
        await context.SaveChangesAsync();
    }
}
