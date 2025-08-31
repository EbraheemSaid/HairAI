using System.ComponentModel.DataAnnotations;

namespace HairAI.Domain.Entities;

public class Patient
{
    public Guid Id { get; set; }

    public Guid ClinicId { get; set; }

    [StringLength(50)]
    public string? ClinicPatientId { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Clinic Clinic { get; set; } = null!;
    public ICollection<AnalysisSession> AnalysisSessions { get; set; } = new List<AnalysisSession>();
    public ICollection<AnalysisJob> AnalysisJobs { get; set; } = new List<AnalysisJob>();
}