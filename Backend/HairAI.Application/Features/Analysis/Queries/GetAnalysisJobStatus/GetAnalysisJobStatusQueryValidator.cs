using FluentValidation;

namespace HairAI.Application.Features.Analysis.Queries.GetAnalysisJobStatus;

public class GetAnalysisJobStatusQueryValidator : AbstractValidator<GetAnalysisJobStatusQuery>
{
    public GetAnalysisJobStatusQueryValidator()
    {
        RuleFor(x => x.JobId)
            .NotEqual(Guid.Empty).WithMessage("Job ID is required");
    }
}