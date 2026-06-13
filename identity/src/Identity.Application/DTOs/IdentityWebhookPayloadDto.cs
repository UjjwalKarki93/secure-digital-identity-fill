namespace Identity.Application.DTOs;

public class IdentityWebhookPayloadDto
{
    public Guid RequestId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public IdentityDataDto IdentityData { get; set; } = new();
    public DateTime Timestamp { get; set; }
    public string Signature { get; set; } = string.Empty;
}
