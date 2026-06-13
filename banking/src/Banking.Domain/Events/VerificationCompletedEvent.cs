namespace Banking.Domain.Events;

public class VerificationCompletedEvent
{
    public Guid RequestId { get; init; }
    public string BankId { get; init; } = string.Empty;
    public object IdentityData { get; init; } = new();
    public DateTime CompletedAt { get; init; }
}
