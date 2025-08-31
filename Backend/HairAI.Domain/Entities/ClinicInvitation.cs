using System.ComponentModel.DataAnnotations;
using HairAI.Domain.Enums;

namespace HairAI.Domain.Entities;

public class ClinicInvitation
{
    public Guid Id { get; set; }

    public Guid ClinicId { get; set; }

    public string InvitedByUserId { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Role { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Token { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = InvitationStatus.Pending.ToString();

    public DateTime ExpiresAt { get; set; }
    
    public DateTime? AcceptedAt { get; set; }
    
    public string? AcceptedByUserId { get; set; }

    // Navigation properties
    public Clinic Clinic { get; set; } = null!;
}