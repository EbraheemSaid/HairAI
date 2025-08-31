using FluentValidation;

namespace HairAI.Application.Features.Analysis.Commands.CreateAnalysisSession;

public class CreateAnalysisSessionCommandValidator : AbstractValidator<CreateAnalysisSessionCommand>
{
    public CreateAnalysisSessionCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEqual(Guid.Empty).WithMessage("Patient ID is required");

        RuleFor(x => x.CreatedByUserId)
            .NotEmpty().WithMessage("Created by user ID is required");

        RuleFor(x => x.SessionDate)
            .NotEmpty().WithMessage("Session date is required");
    }
}