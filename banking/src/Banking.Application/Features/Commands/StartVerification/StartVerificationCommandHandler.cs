using System.Text.Json;
using Banking.Application.DTOs;
using Banking.Application.Interfaces;
using Banking.Domain.Entities;
using Banking.Domain.Enums;
using Banking.Domain.ValueObjects;
using MediatR;

namespace Banking.Application.Features.Commands.StartVerification;

public class StartVerificationCommandHandler : IRequestHandler<StartVerificationCommand, StartVerificationResponseDto>
{
    private const int ExpiryMinutes = 2;
    private readonly IBankClientRepository _bankClientRepository;
    private readonly IVerificationRepository _verificationRepository;
    private readonly IHmacService _hmacService;
    private readonly IAuditService _auditService;

    public StartVerificationCommandHandler(
        IBankClientRepository bankClientRepository,
        IVerificationRepository verificationRepository,
        IHmacService hmacService,
        IAuditService auditService)
    {
        _bankClientRepository = bankClientRepository;
        _verificationRepository = verificationRepository;
        _hmacService = hmacService;
        _auditService = auditService;
    }

    public async Task<StartVerificationResponseDto> Handle(StartVerificationCommand request, CancellationToken cancellationToken)
    {
        var bankClient = await _bankClientRepository.GetByBankIdAsync(request.BankId, cancellationToken)
            ?? throw new InvalidOperationException($"Bank '{request.BankId}' is not registered.");

        if (!bankClient.IsActive)
            throw new InvalidOperationException($"Bank '{request.BankId}' is inactive.");

        var requestId = Guid.NewGuid();
        var expiry = DateTime.UtcNow.AddMinutes(ExpiryMinutes);
        var expiryIso = expiry.ToUniversalTime().ToString("O");

        var qrPayload = new QrPayload
        {
            RequestId = requestId,
            BankId = request.BankId,
            ExpiryIso = expiryIso
        };

        var signature = _hmacService.Sign(qrPayload.ToCanonicalString());
        qrPayload.Signature = signature;

        var verificationRequest = new VerificationRequest
        {
            Id = requestId,
            BankId = request.BankId,
            Status = VerificationStatus.QrGenerated,
            ExpiresAt = expiry,
            QrSignature = signature,
            IsUsed = false
        };

        await _verificationRepository.AddAsync(verificationRequest, cancellationToken);

        await _auditService.LogAsync(
            "VerificationStarted",
            nameof(VerificationRequest),
            requestId.ToString(),
            $"QR generated for bank {request.BankId}",
            request.IpAddress,
            cancellationToken);

        var qrJson = JsonSerializer.Serialize(new
        {
            requestId,
            bankId = request.BankId,
            expiry = expiryIso,
            signature
        });

        return new StartVerificationResponseDto
        {
            RequestId = requestId,
            BankId = request.BankId,
            Expiry = expiry,
            Signature = signature,
            QrPayload = qrJson
        };
    }
}
