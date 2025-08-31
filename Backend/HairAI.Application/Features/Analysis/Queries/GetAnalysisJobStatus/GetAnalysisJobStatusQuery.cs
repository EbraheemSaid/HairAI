using MediatR;

namespace HairAI.Application.Features.Analysis.Queries.GetAnalysisJobStatus;

public class GetAnalysisJobStatusQuery : IRequest<GetAnalysisJobStatusQueryResponse>
{
    public Guid JobId { get; set; }
}