namespace Identity.Domain.Entities;

public class KycData
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string NationalId { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime VerifiedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
