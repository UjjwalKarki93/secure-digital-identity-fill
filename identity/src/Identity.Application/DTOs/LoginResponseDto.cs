namespace Identity.Application.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
