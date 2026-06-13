using Banking.Domain.Entities;

namespace Banking.Application.Interfaces;

public interface IVerificationRepository
{
    Task<VerificationRequest?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task AddAsync(VerificationRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(VerificationRequest request, CancellationToken cancellationToken = default);
    Task<bool> ExistsAndUnusedAsync(Guid requestId, CancellationToken cancellationToken = default);
}
