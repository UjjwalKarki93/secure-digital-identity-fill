using Identity.Application.DTOs;
using MediatR;

namespace Identity.Application.Features.Queries.GetUserProfile;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserProfileDto?>;
