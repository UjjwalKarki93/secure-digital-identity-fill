using System.Text.Json;
using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;
using MediatR;

namespace Identity.Application.Features.Commands.ApproveConsent;

public class ApproveConsentCommandHandler : IRequestHandler<ApproveConsentCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IKycRepository _kycRepository;
    private readonly IConsentRepository _consentRepository;
    private readonly IHmacService _hmacService;
    private readonly IBankingWebhookClient _webhookClient;
    private readonly IAuditService _auditService;

    public ApproveConsentCommandHandler(
        IUserRepository userRepository,
        IKycRepository kycRepository,
        IConsentRepository consentRepository,
        IHmacService hmacService,
        IBankingWebhookClient webhookClient,
        IAuditService auditService)
    {
        _userRepository = userRepository;
        _kycRepository = kycRepository;
        _consentRepository = consentRepository;
        _hmacService = hmacService;
        _webhookClient = webhookClient;
        _auditService = auditService;
    }

    public async Task<bool> Handle(ApproveConsentCommand request, CancellationToken cancellationToken)
    {
        var payload = new QrScanPayload
        {
            RequestId = request.RequestId,
            BankId = request.BankId,
            ExpiryIso = request.ExpiryIso,
            Signature = request.QrSignature
        };

        if (!DateTime.TryParse(request.ExpiryIso, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expiryTime)
            || expiryTime.ToUniversalTime() < DateTime.UtcNow)
            throw new InvalidOperationException("Verification request has expired.");

        if (!_hmacService.Verify(payload.ToCanonicalString(), request.QrSignature))
            throw new UnauthorizedAccessException("Invalid QR signature.");

        var existingConsent = await _consentRepository.GetByRequestIdAsync(request.RequestId, cancellationToken);
        if (existingConsent is not null)
            throw new InvalidOperationException("Consent already recorded for this request.");

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new InvalidOperationException("User not found.");

        var kyc = await _kycRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new InvalidOperationException("KYC data not found.");

        var consent = new ConsentRecord
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            RequestId = request.RequestId,
            BankId = request.BankId,
            IsApproved = true,
            ConsentedAt = DateTime.UtcNow,
            IpAddress = request.IpAddress
        };

        await _consentRepository.AddAsync(consent, cancellationToken);

        var identityData = new IdentityDataDto
        {
            FullName = user.FullName,
            NationalId = kyc.NationalId,
            DateOfBirth = kyc.DateOfBirth,
            Address = kyc.Address,
            PhoneNumber = kyc.PhoneNumber,
            Email = kyc.Email
        };

        var timestamp = DateTime.UtcNow;
        var identityJson = JsonSerializer.Serialize(identityData);
        var canonicalPayload = $"{request.RequestId}|{request.UserId}|{timestamp:O}|{identityJson}";
        var signature = _hmacService.Sign(canonicalPayload);

        var webhookPayload = new IdentityWebhookPayloadDto
        {
            RequestId = request.RequestId,
            UserId = request.UserId.ToString(),
            IdentityData = identityData,
            Timestamp = timestamp,
            Signature = signature
        };

        await _webhookClient.SendVerificationCallbackAsync(webhookPayload, cancellationToken);

        await _auditService.LogAsync(
            "ConsentApproved",
            nameof(ConsentRecord),
            consent.Id.ToString(),
            $"User {user.Username} approved consent for request {request.RequestId}",
            request.IpAddress,
            cancellationToken);

        return true;
    }
}
