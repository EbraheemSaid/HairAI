using FluentValidation;
using System.Text.Json;

namespace HairAI.Application.Features.Calibration.Commands.CreateCalibrationProfile;

public class CreateCalibrationProfileCommandValidator : AbstractValidator<CreateCalibrationProfileCommand>
{
    public CreateCalibrationProfileCommandValidator()
    {
        RuleFor(x => x.ClinicId)
            .NotEqual(Guid.Empty).WithMessage("Clinic ID is required");

        RuleFor(x => x.ProfileName)
            .NotEmpty().WithMessage("Profile name is required")
            .MaximumLength(100).WithMessage("Profile name must not exceed 100 characters");

        RuleFor(x => x.CalibrationData)
            .NotNull().WithMessage("Calibration data is required")
            .Must(BeValidJson).WithMessage("Calibration data must be valid JSON");
    }

    private bool BeValidJson(JsonDocument json)
    {
        return json != null;
    }
}