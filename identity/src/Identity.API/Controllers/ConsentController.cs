using Identity.Application.DTOs;
using Identity.Application.Features.Commands.ApproveConsent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/consent")]
[Authorize]
public class ConsentController : ControllerBase
{
    private readonly IMediator _mediator;

    public ConsentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("approve")]
    [EnableRateLimiting("sensitive")]
    public async Task<IActionResult> Approve(
        [FromBody] ApproveConsentRequestDto request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue("sub")
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        await _mediator.Send(
            new ApproveConsentCommand(
                userId,
                request.RequestId,
                request.BankId,
                request.ExpiryIso,
                request.QrSignature,
                ipAddress),
            cancellationToken);

        return Ok(new { success = true });
    }
}
