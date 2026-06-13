using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public class ConsentRepository : IConsentRepository
{
    private readonly IdentityDbContext _context;

    public ConsentRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<ConsentRecord?> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        return await _context.ConsentRecords.FirstOrDefaultAsync(c => c.RequestId == requestId, cancellationToken);
    }

    public async Task AddAsync(ConsentRecord record, CancellationToken cancellationToken = default)
    {
        await _context.ConsentRecords.AddAsync(record, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
