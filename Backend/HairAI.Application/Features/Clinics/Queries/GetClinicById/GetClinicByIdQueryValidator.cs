using FluentValidation;

namespace HairAI.Application.Features.Clinics.Queries.GetClinicById;

public class GetClinicByIdQueryValidator : AbstractValidator<GetClinicByIdQuery>
{
    public GetClinicByIdQueryValidator()
    {
        RuleFor(x => x.ClinicId)
            .NotEqual(Guid.Empty).WithMessage("Clinic ID is required");
    }
}