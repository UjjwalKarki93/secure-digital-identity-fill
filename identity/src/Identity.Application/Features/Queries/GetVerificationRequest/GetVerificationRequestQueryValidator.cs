using FluentValidation;

namespace Identity.Application.Features.Queries.GetVerificationRequest;

public class GetVerificationRequestQueryValidator : AbstractValidator<GetVerificationRequestQuery>
{
    public GetVerificationRequestQueryValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty();
        RuleFor(x => x.BankId).NotEmpty();
        RuleFor(x => x.Signature).NotEmpty();
    }
}
