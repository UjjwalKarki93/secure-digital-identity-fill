using Identity.Application.Interfaces;
using Identity.Domain.Entities;

namespace Identity.Infrastructure.ExternalServices;

public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task LogAsync(
        string action,
        string entityType,
        string entityId,
        string? details = null,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow
        };

        await _auditLogRepository.AddAsync(log, cancellationToken);
    }
}
