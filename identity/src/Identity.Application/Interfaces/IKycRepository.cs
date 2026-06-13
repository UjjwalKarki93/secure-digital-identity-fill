using Identity.Domain.Entities;

namespace Identity.Application.Interfaces;

public interface IKycRepository
{
    Task<KycData?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
