using MediatR;
using HairAI.Application.DTOs;

namespace HairAI.Application.Features.Analysis.Queries.GetAnalysisSessionDetails;

public class GetAnalysisSessionDetailsQueryResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public AnalysisSessionDto? Session { get; set; }
}