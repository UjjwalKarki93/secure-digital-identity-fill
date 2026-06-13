using FluentValidation;

namespace Banking.Application.Features.Queries.GetVerification;

public class GetVerificationQueryValidator : AbstractValidator<GetVerificationQuery>
{
    public GetVerificationQueryValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty();
    }
}
