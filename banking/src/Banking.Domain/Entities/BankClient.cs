namespace Banking.Domain.Entities;

public class BankClient
{
    public Guid Id { get; set; }
    public string BankId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string HmacSecretKey { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
