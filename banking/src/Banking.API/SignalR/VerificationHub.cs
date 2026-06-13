using Microsoft.AspNetCore.SignalR;

namespace Banking.API.SignalR;

public class VerificationHub : Hub
{
    public async Task JoinVerificationGroup(string requestId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, requestId);
    }

    public async Task LeaveVerificationGroup(string requestId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, requestId);
    }
}
