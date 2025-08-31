using FluentValidation;

namespace HairAI.Application.Features.Calibration.Commands.DeactivateCalibrationProfile;

public class DeactivateCalibrationProfileCommandValidator : AbstractValidator<DeactivateCalibrationProfileCommand>
{
    public DeactivateCalibrationProfileCommandValidator()
    {
        RuleFor(x => x.ProfileId)
            .NotEqual(Guid.Empty).WithMessage("Profile ID is required");
    }
}