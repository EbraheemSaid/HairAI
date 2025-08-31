using FluentValidation;

namespace HairAI.Application.Features.Subscriptions.Commands.CreateSubscription;

public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.ClinicId)
            .NotEqual(Guid.Empty).WithMessage("Clinic ID is required");

        RuleFor(x => x.PlanId)
            .NotEqual(Guid.Empty).WithMessage("Plan ID is required");

        RuleFor(x => x.CurrentPeriodStart)
            .NotEmpty().WithMessage("Current period start is required");

        RuleFor(x => x.CurrentPeriodEnd)
            .NotEmpty().WithMessage("Current period end is required")
            .GreaterThan(x => x.CurrentPeriodStart).WithMessage("Current period end must be after start date");
    }
}