using MediatR;

namespace Identity.Application.Features.Commands.ApproveConsent;

public record ApproveConsentCommand(
    Guid UserId,
    Guid RequestId,
    string BankId,
    string ExpiryIso,
    string QrSignature,
    string? IpAddress) : IRequest<bool>;
