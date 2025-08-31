using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HairAI.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    public Guid? ClinicId { get; set; }

    // Navigation properties
    public Clinic? Clinic { get; set; }
}