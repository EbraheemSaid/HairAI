using FluentValidation;

namespace HairAI.Application.Features.Subscriptions.Queries.GetSubscriptionForClinic;

public class GetSubscriptionForClinicQueryValidator : AbstractValidator<GetSubscriptionForClinicQuery>
{
    public GetSubscriptionForClinicQueryValidator()
    {
        RuleFor(x => x.ClinicId)
            .NotEqual(Guid.Empty).WithMessage("Clinic ID is required");
    }
}