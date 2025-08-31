using System.ComponentModel.DataAnnotations;

namespace HairAI.Domain.Entities;

public class AnalysisSession
{
    public Guid Id { get; set; }

    public Guid PatientId { get; set; }

    public string CreatedByUserId { get; set; } = string.Empty;

    public DateOnly SessionDate { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = string.Empty;

    public string? FinalReportData { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Patient Patient { get; set; } = null!;
    public ICollection<AnalysisJob> AnalysisJobs { get; set; } = new List<AnalysisJob>();
}