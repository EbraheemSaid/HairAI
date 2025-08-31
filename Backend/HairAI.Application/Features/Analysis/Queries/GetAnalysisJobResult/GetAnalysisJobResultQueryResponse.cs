using MediatR;

namespace HairAI.Application.Features.Analysis.Queries.GetAnalysisJobResult;

public class GetAnalysisJobResultQueryResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public string? AnalysisResult { get; set; }
    public string? AnnotatedImageKey { get; set; }
    public string? DoctorNotes { get; set; }
}