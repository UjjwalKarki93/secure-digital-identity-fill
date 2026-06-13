using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<KycData> KycData => Set<KycData>();
    public DbSet<ConsentRecord> ConsentRecords => Set<ConsentRecord>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<KycData>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NationalId).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
        });

        modelBuilder.Entity<ConsentRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BankId).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.RequestId).IsUnique();
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.CreatedAt);
        });

    }
}
