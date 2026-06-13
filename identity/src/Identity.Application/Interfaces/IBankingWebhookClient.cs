using Identity.Application.DTOs;

namespace Identity.Application.Interfaces;

public interface IBankingWebhookClient
{
    Task SendVerificationCallbackAsync(IdentityWebhookPayloadDto payload, CancellationToken cancellationToken = default);
}
