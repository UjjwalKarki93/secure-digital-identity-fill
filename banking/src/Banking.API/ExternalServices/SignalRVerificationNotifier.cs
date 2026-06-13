using Banking.API.SignalR;
using Banking.Application.Interfaces;
using Banking.Domain.Events;
using Microsoft.AspNetCore.SignalR;

namespace Banking.API.ExternalServices;

public class SignalRVerificationNotifier : IVerificationNotifier
{
    private readonly IHubContext<VerificationHub> _hubContext;

    public SignalRVerificationNotifier(IHubContext<VerificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyVerificationCompletedAsync(
        VerificationCompletedEvent completedEvent,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients
            .Group(completedEvent.RequestId.ToString())
            .SendAsync("VerificationCompleted", new
            {
                completedEvent.RequestId,
                completedEvent.BankId,
                completedEvent.IdentityData,
                completedEvent.CompletedAt
            }, cancellationToken);
    }
}
