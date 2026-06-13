using System.Text.Json;
using Banking.Application.DTOs;
using Banking.Application.Interfaces;
using Banking.Domain.Enums;
using MediatR;

namespace Banking.Application.Features.Queries.GetVerification;

public class GetVerificationQueryHandler : IRequestHandler<GetVerificationQuery, VerificationStatusDto?>
{
    private readonly IVerificationRepository _verificationRepository;

    public GetVerificationQueryHandler(IVerificationRepository verificationRepository)
    {
        _verificationRepository = verificationRepository;
    }

    public async Task<VerificationStatusDto?> Handle(GetVerificationQuery request, CancellationToken cancellationToken)
    {
        var verification = await _verificationRepository.GetByIdAsync(request.RequestId, cancellationToken);
        if (verification is null)
            return null;

        if (verification.Status != VerificationStatus.Completed
            && verification.ExpiresAt < DateTime.UtcNow
            && verification.Status != VerificationStatus.Expired)
        {
            verification.Status = VerificationStatus.Expired;
            await _verificationRepository.UpdateAsync(verification, cancellationToken);
        }

        IdentityDataDto? identityData = null;
        if (!string.IsNullOrEmpty(verification.IdentityPayloadJson))
        {
            identityData = JsonSerializer.Deserialize<IdentityDataDto>(
                verification.IdentityPayloadJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        return new VerificationStatusDto
        {
            RequestId = verification.Id,
            Status = verification.Status.ToString(),
            ExpiresAt = verification.ExpiresAt,
            CompletedAt = verification.CompletedAt,
            IdentityData = identityData
        };
    }
}
