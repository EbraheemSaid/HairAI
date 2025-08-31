using MediatR;

namespace HairAI.Application.Features.Analysis.Queries.GetAnalysisJobStatus;

public class GetAnalysisJobStatusQueryResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public string? Status { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int? ProcessingTimeMs { get; set; }
}