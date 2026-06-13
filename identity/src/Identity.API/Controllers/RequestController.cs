using Identity.Application.DTOs;
using Identity.Application.Features.Queries.GetVerificationRequest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/request")]
[Authorize]
public class RequestController : ControllerBase
{
    private readonly IMediator _mediator;

    public RequestController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{requestId:guid}")]
    public async Task<ActionResult<VerificationRequestDto>> Get(
        Guid requestId,
        [FromQuery] string bankId,
        [FromQuery] string expiry,
        [FromQuery] string signature,
        CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _mediator.Send(
            new GetVerificationRequestQuery(requestId, bankId, expiry, signature, ipAddress),
            cancellationToken);
        return Ok(result);
    }
}
