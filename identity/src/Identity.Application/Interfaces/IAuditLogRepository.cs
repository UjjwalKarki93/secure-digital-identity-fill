using Identity.Domain.Entities;

namespace Identity.Application.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log, CancellationToken cancellationToken = default);
}
