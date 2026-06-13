using Identity.Application.DTOs;
using MediatR;

namespace Identity.Application.Features.Queries.GetVerificationRequest;

public record GetVerificationRequestQuery(
    Guid RequestId,
    string BankId,
    string ExpiryIso,
    string Signature,
    string? IpAddress) : IRequest<VerificationRequestDto>;
