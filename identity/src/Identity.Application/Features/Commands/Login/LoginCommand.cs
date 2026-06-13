using Identity.Application.DTOs;
using MediatR;

namespace Identity.Application.Features.Commands.Login;

public record LoginCommand(string Username, string Password, string? IpAddress) : IRequest<LoginResponseDto>;
