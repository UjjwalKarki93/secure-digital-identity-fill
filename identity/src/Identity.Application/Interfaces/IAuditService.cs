namespace Identity.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(string action, string entityType, string entityId, string? details = null, string? ipAddress = null, CancellationToken cancellationToken = default);
}
