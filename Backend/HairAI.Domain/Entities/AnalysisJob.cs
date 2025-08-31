using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using HairAI.Domain.Enums;

namespace HairAI.Domain.Entities;

public class AnalysisJob
{
    public Guid Id { get; set; }

    public Guid SessionId { get; set; }

    public Guid PatientId { get; set; }

    public Guid CalibrationProfileId { get; set; }

    public string CreatedByUserId { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LocationTag { get; set; } = string.Empty;

    [Required]
    public string ImageStorageKey { get; set; } = string.Empty;

    public string? AnnotatedImageKey { get; set; }

    // DESIGN IMPROVEMENT: Use enum instead of string for type safety
    public JobStatus Status { get; set; } = JobStatus.Pending;

    public string? AnalysisResult { get; set; }

    public string? DoctorNotes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? ErrorMessage { get; set; }

    public int? ProcessingTimeMs { get; set; }

    // Navigation properties
    public AnalysisSession Session { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
    public CalibrationProfile CalibrationProfile { get; set; } = null!;
}