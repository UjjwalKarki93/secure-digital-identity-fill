using FluentValidation;

namespace Banking.Application.Features.Commands.ProcessIdentityWebhook;

public class ProcessIdentityWebhookCommandValidator : AbstractValidator<ProcessIdentityWebhookCommand>
{
    public ProcessIdentityWebhookCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Signature).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NationalId).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Timestamp).NotEmpty();
    }
}
