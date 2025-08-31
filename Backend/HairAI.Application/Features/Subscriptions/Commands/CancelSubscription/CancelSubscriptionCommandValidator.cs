using FluentValidation;

namespace HairAI.Application.Features.Subscriptions.Commands.CancelSubscription;

public class CancelSubscriptionCommandValidator : AbstractValidator<CancelSubscriptionCommand>
{
    public CancelSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEqual(Guid.Empty).WithMessage("Subscription ID is required");
    }
}