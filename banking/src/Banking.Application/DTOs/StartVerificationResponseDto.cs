namespace Banking.Application.DTOs;

public class StartVerificationResponseDto
{
    public Guid RequestId { get; set; }
    public string BankId { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
    public string Signature { get; set; } = string.Empty;
    public string QrPayload { get; set; } = string.Empty;
}
