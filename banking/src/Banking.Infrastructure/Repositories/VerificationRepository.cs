using Banking.Application.Interfaces;
using Banking.Domain.Entities;
using Banking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Repositories;

public class VerificationRepository : IVerificationRepository
{
    private readonly BankingDbContext _context;

    public VerificationRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task<VerificationRequest?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        return await _context.VerificationRequests
            .FirstOrDefaultAsync(v => v.Id == requestId, cancellationToken);
    }

    public async Task AddAsync(VerificationRequest request, CancellationToken cancellationToken = default)
    {
        await _context.VerificationRequests.AddAsync(request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(VerificationRequest request, CancellationToken cancellationToken = default)
    {
        _context.VerificationRequests.Update(request);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAndUnusedAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        return await _context.VerificationRequests
            .AnyAsync(v => v.Id == requestId && !v.IsUsed, cancellationToken);
    }
}
