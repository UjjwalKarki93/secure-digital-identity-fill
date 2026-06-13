namespace Identity.Application.DTOs;

public class UserProfileDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public IdentityDataDto KycData { get; set; } = new();
}
