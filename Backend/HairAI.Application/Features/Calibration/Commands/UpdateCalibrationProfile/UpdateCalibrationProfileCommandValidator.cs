using FluentValidation;
using System.Text.Json;

namespace HairAI.Application.Features.Calibration.Commands.UpdateCalibrationProfile;

public class UpdateCalibrationProfileCommandValidator : AbstractValidator<UpdateCalibrationProfileCommand>
{
    public UpdateCalibrationProfileCommandValidator()
    {
        RuleFor(x => x.ProfileId)
            .NotEqual(Guid.Empty).WithMessage("Profile ID is required");

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