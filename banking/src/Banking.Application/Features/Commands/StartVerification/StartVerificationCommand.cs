using Banking.Application.DTOs;
using MediatR;

namespace Banking.Application.Features.Commands.StartVerification;

public record StartVerificationCommand(string BankId, string? IpAddress) : IRequest<StartVerificationResponseDto>;
