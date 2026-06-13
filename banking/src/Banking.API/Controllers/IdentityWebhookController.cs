using Banking.Application.DTOs;
using Banking.Application.Features.Commands.ProcessIdentityWebhook;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Banking.API.Controllers;

[ApiController]
[Route("api/identity")]
public class IdentityWebhookController : ControllerBase
{
    private readonly IMediator _mediator;

    public IdentityWebhookController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("webhook")]
    [EnableRateLimiting("sensitive")]
    public async Task<IActionResult> Webhook(
        [FromBody] IdentityWebhookDto payload,
        CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var command = new ProcessIdentityWebhookCommand(
            payload.RequestId,
            payload.UserId,
            payload.IdentityData.FullName,
            payload.IdentityData.NationalId,
            payload.IdentityData.DateOfBirth,
            payload.IdentityData.Address,
            payload.IdentityData.PhoneNumber,
            payload.IdentityData.Email,
            payload.Timestamp,
            payload.Signature,
            ipAddress);

        await _mediator.Send(command, cancellationToken);
        return Ok(new { success = true });
    }
}
