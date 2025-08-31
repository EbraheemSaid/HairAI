using FluentValidation;

namespace HairAI.Application.Features.Analysis.Queries.GetAnalysisSessionDetails;

public class GetAnalysisSessionDetailsQueryValidator : AbstractValidator<GetAnalysisSessionDetailsQuery>
{
    public GetAnalysisSessionDetailsQueryValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEqual(Guid.Empty).WithMessage("Session ID is required");
    }
}