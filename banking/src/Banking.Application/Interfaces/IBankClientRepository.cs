using Banking.Domain.Entities;

namespace Banking.Application.Interfaces;

public interface IBankClientRepository
{
    Task<BankClient?> GetByBankIdAsync(string bankId, CancellationToken cancellationToken = default);
}
