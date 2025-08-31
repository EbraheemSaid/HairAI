using FluentValidation;

namespace HairAI.Application.Features.Analysis.Commands.GenerateFinalReport;

public class GenerateFinalReportCommandValidator : AbstractValidator<GenerateFinalReportCommand>
{
    public GenerateFinalReportCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEqual(Guid.Empty).WithMessage("Session ID is required");
    }
}