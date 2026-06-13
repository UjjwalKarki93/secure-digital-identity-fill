using Banking.Application.DTOs;
using Banking.Application.Features.Commands.StartVerification;
using Banking.Application.Features.Queries.GetVerification;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Banking.API.Controllers;

[ApiController]
[Route("api/verification")]
public class VerificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public VerificationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("start")]
    [EnableRateLimiting("sensitive")]
    public async Task<ActionResult<StartVerificationResponseDto>> Start(
        [FromBody] StartVerificationRequest request,
        CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _mediator.Send(
            new StartVerificationCommand(request.BankId, ipAddress),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{requestId:guid}")]
    public async Task<ActionResult<VerificationStatusDto>> Get(
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetVerificationQuery(requestId), cancellationToken);
        if (result is null)
            return NotFound();
        return Ok(result);
    }
}

public class StartVerificationRequest
{
    public string BankId { get; set; } = "BANK-001";
}
