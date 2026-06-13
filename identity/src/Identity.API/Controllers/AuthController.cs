using Identity.Application.DTOs;
using Identity.Application.Features.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [EnableRateLimiting("sensitive")]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _mediator.Send(
            new LoginCommand(request.Username, request.Password, ipAddress),
            cancellationToken);
        return Ok(result);
    }
}
