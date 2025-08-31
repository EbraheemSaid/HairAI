using FluentValidation;

namespace HairAI.Application.Features.Patients.Queries.GetPatientsForClinic;

public class GetPatientsForClinicQueryValidator : AbstractValidator<GetPatientsForClinicQuery>
{
    public GetPatientsForClinicQueryValidator()
    {
        RuleFor(x => x.ClinicId)
            .NotEqual(Guid.Empty).WithMessage("Clinic ID is required");
    }
}