using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public class KycRepository : IKycRepository
{
    private readonly IdentityDbContext _context;

    public KycRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<KycData?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.KycData.FirstOrDefaultAsync(k => k.UserId == userId, cancellationToken);
    }
}
