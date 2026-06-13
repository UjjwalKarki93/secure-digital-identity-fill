namespace Identity.Application.DTOs;

public class VerificationRequestDto
{
    public Guid RequestId { get; set; }
    public string BankId { get; set; } = string.Empty;
    public string ExpiryIso { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
}
