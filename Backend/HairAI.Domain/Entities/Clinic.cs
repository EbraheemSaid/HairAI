using System.ComponentModel.DataAnnotations;

namespace HairAI.Domain.Entities;

public class Clinic
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Patient> Patients { get; set; } = new List<Patient>();
    public ICollection<CalibrationProfile> CalibrationProfiles { get; set; } = new List<CalibrationProfile>();
    public Subscription? Subscription { get; set; }
    public ICollection<ClinicInvitation> ClinicInvitations { get; set; } = new List<ClinicInvitation>();
    public ICollection<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
}