using Banking.Domain.Enums;

namespace Banking.Domain.Entities;

public class VerificationRequest
{
    public Guid Id { get; set; }
    public string BankId { get; set; } = string.Empty;
    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsUsed { get; set; }
    public string? IdentityUserId { get; set; }
    public string? IdentityPayloadJson { get; set; }
    public string? QrSignature { get; set; }
}
