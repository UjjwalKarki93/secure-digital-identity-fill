using Banking.Domain.Entities;

namespace Banking.Application.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log, CancellationToken cancellationToken = default);
}
