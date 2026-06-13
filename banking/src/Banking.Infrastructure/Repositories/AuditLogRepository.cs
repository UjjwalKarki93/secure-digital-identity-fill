using Banking.Application.Interfaces;
using Banking.Domain.Entities;
using Banking.Infrastructure.Persistence;

namespace Banking.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly BankingDbContext _context;

    public AuditLogRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditLog log, CancellationToken cancellationToken = default)
    {
        await _context.AuditLogs.AddAsync(log, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
