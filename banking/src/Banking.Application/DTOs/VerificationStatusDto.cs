namespace Banking.Application.DTOs;

public class VerificationStatusDto
{
    public Guid RequestId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public IdentityDataDto? IdentityData { get; set; }
}
