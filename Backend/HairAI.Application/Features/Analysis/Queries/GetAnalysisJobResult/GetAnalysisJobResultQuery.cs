using MediatR;

namespace HairAI.Application.Features.Analysis.Queries.GetAnalysisJobResult;

public class GetAnalysisJobResultQuery : IRequest<GetAnalysisJobResultQueryResponse>
{
    public Guid JobId { get; set; }
}