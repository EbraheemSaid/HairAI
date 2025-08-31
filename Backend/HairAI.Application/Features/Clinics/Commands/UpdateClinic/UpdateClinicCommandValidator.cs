using FluentValidation;

namespace HairAI.Application.Features.Clinics.Commands.UpdateClinic;

public class UpdateClinicCommandValidator : AbstractValidator<UpdateClinicCommand>
{
    public UpdateClinicCommandValidator()
    {
        RuleFor(x => x.ClinicId)
            .NotEqual(Guid.Empty).WithMessage("Clinic ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Clinic name is required")
            .MaximumLength(255).WithMessage("Clinic name must not exceed 255 characters");
    }
}