using FluentValidation;

namespace HairAI.Application.Features.Analysis.Queries.GetAnalysisJobResult;

public class GetAnalysisJobResultQueryValidator : AbstractValidator<GetAnalysisJobResultQuery>
{
    public GetAnalysisJobResultQueryValidator()
    {
        RuleFor(x => x.JobId)
            .NotEqual(Guid.Empty).WithMessage("Job ID is required");
    }
}