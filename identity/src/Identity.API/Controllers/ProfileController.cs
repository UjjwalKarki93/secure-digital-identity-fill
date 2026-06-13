using Identity.Application.DTOs;
using Identity.Application.Features.Queries.GetUserProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<UserProfileDto>> Get(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue("sub")
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new GetUserProfileQuery(userId), cancellationToken);
        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
