namespace HairAI.Application.DTOs;

public class AnalysisSessionDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public DateOnly SessionDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? FinalReportData { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<AnalysisJobDto> AnalysisJobs { get; set; } = new();
}