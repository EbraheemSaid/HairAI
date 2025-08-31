using MediatR;

namespace HairAI.Application.Features.Analysis.Queries.GetAnalysisSessionDetails;

public class GetAnalysisSessionDetailsQuery : IRequest<GetAnalysisSessionDetailsQueryResponse>
{
    public Guid SessionId { get; set; }
}