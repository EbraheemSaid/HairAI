using FluentValidation;

namespace HairAI.Application.Features.Admin.Commands.ManuallyCreateClinic;

public class ManuallyCreateClinicCommandValidator : AbstractValidator<ManuallyCreateClinicCommand>
{
    public ManuallyCreateClinicCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Clinic name is required")
            .MaximumLength(255).WithMessage("Clinic name must not exceed 255 characters");
    }
}