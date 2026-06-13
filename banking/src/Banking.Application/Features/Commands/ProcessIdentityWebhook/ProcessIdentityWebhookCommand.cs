using MediatR;

namespace Banking.Application.Features.Commands.ProcessIdentityWebhook;

public record ProcessIdentityWebhookCommand(
    Guid RequestId,
    string UserId,
    string FullName,
    string NationalId,
    string DateOfBirth,
    string Address,
    string PhoneNumber,
    string Email,
    DateTime Timestamp,
    string Signature,
    string? IpAddress) : IRequest<bool>;
