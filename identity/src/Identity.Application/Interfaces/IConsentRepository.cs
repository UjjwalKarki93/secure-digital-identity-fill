using Identity.Domain.Entities;

namespace Identity.Application.Interfaces;

public interface IConsentRepository
{
    Task<ConsentRecord?> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task AddAsync(ConsentRecord record, CancellationToken cancellationToken = default);
}
