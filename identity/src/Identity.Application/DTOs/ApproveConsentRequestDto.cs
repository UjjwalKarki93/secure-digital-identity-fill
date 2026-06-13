namespace Identity.Application.DTOs;

public class ApproveConsentRequestDto
{
    public Guid RequestId { get; set; }
    public string BankId { get; set; } = string.Empty;
    public string ExpiryIso { get; set; } = string.Empty;
    public string QrSignature { get; set; } = string.Empty;
}
