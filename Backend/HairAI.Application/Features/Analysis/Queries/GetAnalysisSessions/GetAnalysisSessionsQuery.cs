using MediatR;

namespace HairAI.Application.Features.Analysis.Queries.GetAnalysisSessions;

public class GetAnalysisSessionsQuery : IRequest<GetAnalysisSessionsQueryResponse>
{
    public Guid? PatientId { get; set; }
    public Guid? ClinicId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "SessionDate";
    public string SortDirection { get; set; } = "desc";
} 