using System.Text.Json;
using Banking.Application.DTOs;
using Banking.Application.Interfaces;
using Banking.Domain.Entities;
using Banking.Domain.Enums;
using Banking.Domain.Events;
using MediatR;

namespace Banking.Application.Features.Commands.ProcessIdentityWebhook;

public class ProcessIdentityWebhookCommandHandler : IRequestHandler<ProcessIdentityWebhookCommand, bool>
{
    private readonly IVerificationRepository _verificationRepository;
    private readonly IHmacService _hmacService;
    private readonly IVerificationNotifier _notifier;
    private readonly IAuditService _auditService;

    public ProcessIdentityWebhookCommandHandler(
        IVerificationRepository verificationRepository,
        IHmacService hmacService,
        IVerificationNotifier notifier,
        IAuditService auditService)
    {
        _verificationRepository = verificationRepository;
        _hmacService = hmacService;
        _notifier = notifier;
        _auditService = auditService;
    }

    public async Task<bool> Handle(ProcessIdentityWebhookCommand request, CancellationToken cancellationToken)
    {
        var verification = await _verificationRepository.GetByIdAsync(request.RequestId, cancellationToken)
            ?? throw new InvalidOperationException("Verification request not found.");

        if (verification.IsUsed)
            throw new InvalidOperationException("Verification request has already been used.");

        if (verification.ExpiresAt < DateTime.UtcNow)
        {
            verification.Status = VerificationStatus.Expired;
            await _verificationRepository.UpdateAsync(verification, cancellationToken);
            throw new InvalidOperationException("Verification request has expired.");
        }

        if (verification.Status == VerificationStatus.Completed)
            throw new InvalidOperationException("Verification already completed.");

        var identityData = new IdentityDataDto
        {
            FullName = request.FullName,
            NationalId = request.NationalId,
            DateOfBirth = request.DateOfBirth,
            Address = request.Address,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email
        };

        var identityJson = JsonSerializer.Serialize(identityData);
        var canonicalPayload = $"{request.RequestId}|{request.UserId}|{request.Timestamp:O}|{identityJson}";

        if (!_hmacService.Verify(canonicalPayload, request.Signature))
            throw new UnauthorizedAccessException("Invalid webhook signature.");

        verification.Status = VerificationStatus.Completed;
        verification.IsUsed = true;
        verification.CompletedAt = DateTime.UtcNow;
        verification.IdentityUserId = request.UserId;
        verification.IdentityPayloadJson = identityJson;

        await _verificationRepository.UpdateAsync(verification, cancellationToken);

        await _auditService.LogAsync(
            "VerificationCompleted",
            nameof(VerificationRequest),
            verification.Id.ToString(),
            $"Identity verified for user {request.UserId}",
            request.IpAddress,
            cancellationToken);

        var completedEvent = new VerificationCompletedEvent
        {
            RequestId = verification.Id,
            BankId = verification.BankId,
            IdentityData = identityData,
            CompletedAt = verification.CompletedAt.Value
        };

        await _notifier.NotifyVerificationCompletedAsync(completedEvent, cancellationToken);

        return true;
    }
}
