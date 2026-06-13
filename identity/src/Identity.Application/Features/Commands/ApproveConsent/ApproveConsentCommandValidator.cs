using FluentValidation;

namespace Identity.Application.Features.Commands.ApproveConsent;

public class ApproveConsentCommandValidator : AbstractValidator<ApproveConsentCommand>
{
    public ApproveConsentCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RequestId).NotEmpty();
        RuleFor(x => x.BankId).NotEmpty();
        RuleFor(x => x.QrSignature).NotEmpty();
    }
}
