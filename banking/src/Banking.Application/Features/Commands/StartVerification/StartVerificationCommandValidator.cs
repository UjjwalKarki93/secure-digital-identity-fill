using FluentValidation;

namespace Banking.Application.Features.Commands.StartVerification;

public class StartVerificationCommandValidator : AbstractValidator<StartVerificationCommand>
{
    public StartVerificationCommandValidator()
    {
        RuleFor(x => x.BankId)
            .NotEmpty()
            .MaximumLength(50);
    }
}
