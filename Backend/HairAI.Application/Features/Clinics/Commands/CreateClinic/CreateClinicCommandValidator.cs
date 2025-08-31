using FluentValidation;

namespace HairAI.Application.Features.Clinics.Commands.CreateClinic;

public class CreateClinicCommandValidator : AbstractValidator<CreateClinicCommand>
{
    public CreateClinicCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Clinic name is required")
            .MaximumLength(255).WithMessage("Clinic name must not exceed 255 characters");
    }
}