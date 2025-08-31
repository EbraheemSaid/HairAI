using MediatR;

namespace HairAI.Application.Features.Analysis.Commands.CreateAnalysisSession;

public class CreateAnalysisSessionCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Guid? SessionId { get; set; }
}