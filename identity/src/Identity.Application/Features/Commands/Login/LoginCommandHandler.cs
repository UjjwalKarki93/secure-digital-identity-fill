using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using MediatR;

namespace Identity.Application.Features.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IAuditService _auditService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IAuditService auditService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _auditService = auditService;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!user.IsActive || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = _jwtTokenService.GenerateToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(1);

        await _auditService.LogAsync(
            "UserLogin",
            "User",
            user.Id.ToString(),
            $"User {user.Username} logged in",
            request.IpAddress,
            cancellationToken);

        return new LoginResponseDto
        {
            Token = token,
            UserId = user.Id,
            FullName = user.FullName,
            ExpiresAt = expiresAt
        };
    }
}
