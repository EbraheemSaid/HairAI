using FluentValidation;

namespace HairAI.Application.Features.Admin.Commands.ManuallyLogPayment;

public class ManuallyLogPaymentCommandValidator : AbstractValidator<ManuallyLogPaymentCommand>
{
    public ManuallyLogPaymentCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEqual(Guid.Empty).WithMessage("Subscription ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .MaximumLength(3).WithMessage("Currency must be a 3-character code");

        RuleFor(x => x.PaymentGatewayReference)
            .NotEmpty().WithMessage("Payment gateway reference is required");
    }
}