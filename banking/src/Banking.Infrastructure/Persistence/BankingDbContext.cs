using Banking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Persistence;

public class BankingDbContext : DbContext
{
    public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options)
    {
    }

    public DbSet<VerificationRequest> VerificationRequests => Set<VerificationRequest>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<BankClient> BankClients => Set<BankClient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VerificationRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BankId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(30);
            entity.HasIndex(e => e.BankId);
            entity.HasIndex(e => e.ExpiresAt);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EntityType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EntityId).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.CreatedAt);
        });

        modelBuilder.Entity<BankClient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BankId).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.BankId).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.HmacSecretKey).HasMaxLength(500).IsRequired();
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BankClient>().HasData(new BankClient
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            BankId = "BANK-001",
            Name = "Laxmi Sunrise Bank",
            HmacSecretKey = "shared-hmac-secret-key-change-in-production",
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
