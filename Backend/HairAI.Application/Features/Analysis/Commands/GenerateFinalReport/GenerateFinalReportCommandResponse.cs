using MediatR;

namespace HairAI.Application.Features.Analysis.Commands.GenerateFinalReport;

public class GenerateFinalReportCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public string? ReportData { get; set; }
}