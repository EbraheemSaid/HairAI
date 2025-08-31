using FluentValidation;

namespace HairAI.Application.Features.Patients.Queries.GetPatientById;

public class GetPatientByIdQueryValidator : AbstractValidator<GetPatientByIdQuery>
{
    public GetPatientByIdQueryValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEqual(Guid.Empty).WithMessage("Patient ID is required");
    }
}