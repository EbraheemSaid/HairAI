using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace HairAI.Domain.Entities;

public class CalibrationProfile
{
    public Guid Id { get; set; }

    public Guid ClinicId { get; set; }

    [Required]
    [StringLength(100)]
    public string ProfileName { get; set; } = string.Empty;

    [Required]
    public JsonDocument CalibrationData { get; set; } = null!;

    public int Version { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Clinic Clinic { get; set; } = null!;
    public ICollection<AnalysisJob> AnalysisJobs { get; set; } = new List<AnalysisJob>();
}