using Banking.Application.Interfaces;
using Banking.Domain.Entities;
using Banking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Repositories;

public class BankClientRepository : IBankClientRepository
{
    private readonly BankingDbContext _context;

    public BankClientRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task<BankClient?> GetByBankIdAsync(string bankId, CancellationToken cancellationToken = default)
    {
        return await _context.BankClients
            .FirstOrDefaultAsync(b => b.BankId == bankId, cancellationToken);
    }
}
