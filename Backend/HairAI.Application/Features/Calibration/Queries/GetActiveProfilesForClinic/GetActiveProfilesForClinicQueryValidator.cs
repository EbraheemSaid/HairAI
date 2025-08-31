using FluentValidation;

namespace HairAI.Application.Features.Calibration.Queries.GetActiveProfilesForClinic;

public class GetActiveProfilesForClinicQueryValidator : AbstractValidator<GetActiveProfilesForClinicQuery>
{
    public GetActiveProfilesForClinicQueryValidator()
    {
        RuleFor(x => x.ClinicId)
            .NotEqual(Guid.Empty).WithMessage("Clinic ID is required");
    }
}