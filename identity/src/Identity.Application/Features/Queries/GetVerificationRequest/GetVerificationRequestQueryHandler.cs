using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Domain.ValueObjects;
using MediatR;

namespace Identity.Application.Features.Queries.GetVerificationRequest;

public class GetVerificationRequestQueryHandler : IRequestHandler<GetVerificationRequestQuery, VerificationRequestDto>
{
    private readonly IHmacService _hmacService;
    private readonly IConsentRepository _consentRepository;
    private readonly IAuditService _auditService;

    public GetVerificationRequestQueryHandler(
        IHmacService hmacService,
        IConsentRepository consentRepository,
        IAuditService auditService)
    {
        _hmacService = hmacService;
        _consentRepository = consentRepository;
        _auditService = auditService;
    }

    public async Task<VerificationRequestDto> Handle(GetVerificationRequestQuery request, CancellationToken cancellationToken)
    {
        var payload = new QrScanPayload
        {
            RequestId = request.RequestId,
            BankId = request.BankId,
            ExpiryIso = request.ExpiryIso,
            Signature = request.Signature
        };

        if (!DateTime.TryParse(request.ExpiryIso, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expiryTime)
            || expiryTime.ToUniversalTime() < DateTime.UtcNow)
        {
            await _auditService.LogAsync(
                "QrValidationFailed",
                "VerificationRequest",
                request.RequestId.ToString(),
                "QR code expired",
                request.IpAddress,
                cancellationToken);

            return new VerificationRequestDto
            {
                RequestId = request.RequestId,
                BankId = request.BankId,
                ExpiryIso = request.ExpiryIso,
                IsValid = false,
                Message = "QR code has expired."
            };
        }

        if (!_hmacService.Verify(payload.ToCanonicalString(), request.Signature))
        {
            await _auditService.LogAsync(
                "QrValidationFailed",
                "VerificationRequest",
                request.RequestId.ToString(),
                "Invalid QR signature",
                request.IpAddress,
                cancellationToken);

            return new VerificationRequestDto
            {
                RequestId = request.RequestId,
                BankId = request.BankId,
                ExpiryIso = request.ExpiryIso,
                IsValid = false,
                Message = "Invalid QR signature."
            };
        }

        var existingConsent = await _consentRepository.GetByRequestIdAsync(request.RequestId, cancellationToken);
        if (existingConsent is not null)
        {
            return new VerificationRequestDto
            {
                RequestId = request.RequestId,
                BankId = request.BankId,
                ExpiryIso = request.ExpiryIso,
                IsValid = false,
                Message = "This verification request has already been used."
            };
        }

        await _auditService.LogAsync(
            "QrValidated",
            "VerificationRequest",
            request.RequestId.ToString(),
            $"QR validated for bank {request.BankId}",
            request.IpAddress,
            cancellationToken);

        return new VerificationRequestDto
        {
            RequestId = request.RequestId,
            BankId = request.BankId,
            ExpiryIso = request.ExpiryIso,
            IsValid = true,
            Message = "Verification request is valid."
        };
    }
}
