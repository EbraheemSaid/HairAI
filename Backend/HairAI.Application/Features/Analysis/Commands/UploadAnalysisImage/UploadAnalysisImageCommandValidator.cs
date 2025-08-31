using FluentValidation;

namespace HairAI.Application.Features.Analysis.Commands.UploadAnalysisImage;

public class UploadAnalysisImageCommandValidator : AbstractValidator<UploadAnalysisImageCommand>
{
    public UploadAnalysisImageCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEqual(Guid.Empty).WithMessage("Session ID is required");

        RuleFor(x => x.PatientId)
            .NotEqual(Guid.Empty).WithMessage("Patient ID is required");

        RuleFor(x => x.CalibrationProfileId)
            .NotEqual(Guid.Empty).WithMessage("Calibration profile ID is required");

        RuleFor(x => x.CreatedByUserId)
            .NotEmpty().WithMessage("Created by user ID is required");

        RuleFor(x => x.LocationTag)
            .NotEmpty().WithMessage("Location tag is required")
            .MaximumLength(100).WithMessage("Location tag must not exceed 100 characters");

        RuleFor(x => x.ImageStorageKey)
            .NotEmpty().WithMessage("Image storage key is required");
    }
}