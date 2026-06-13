using Banking.Domain.Events;

namespace Banking.Application.Interfaces;

public interface IVerificationNotifier
{
    Task NotifyVerificationCompletedAsync(VerificationCompletedEvent completedEvent, CancellationToken cancellationToken = default);
}
