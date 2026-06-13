namespace Identity.Domain.Entities;

public class ConsentRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RequestId { get; set; }
    public string BankId { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime ConsentedAt { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }

    public User User { get; set; } = null!;
}
