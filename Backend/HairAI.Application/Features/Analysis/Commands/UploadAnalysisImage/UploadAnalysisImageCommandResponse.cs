using MediatR;

namespace HairAI.Application.Features.Analysis.Commands.UploadAnalysisImage;

public class UploadAnalysisImageCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Guid? JobId { get; set; }
}