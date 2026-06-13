using Banking.Application.DTOs;
using MediatR;

namespace Banking.Application.Features.Queries.GetVerification;

public record GetVerificationQuery(Guid RequestId) : IRequest<VerificationStatusDto?>;
