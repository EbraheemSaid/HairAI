namespace HairAI.Application.DTOs;

public class AnalysisJobDto
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public Guid PatientId { get; set; }
    public Guid CalibrationProfileId { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public string LocationTag { get; set; } = string.Empty;
    public string ImageStorageKey { get; set; } = string.Empty;
    public string? AnnotatedImageKey { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? AnalysisResult { get; set; }
    public string? DoctorNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int? ProcessingTimeMs { get; set; }
}