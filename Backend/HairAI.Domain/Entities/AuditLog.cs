using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace HairAI.Domain.Entities;

public class AuditLog
{
    public long Id { get; set; }

    public string? UserId { get; set; }

    [StringLength(50)]
    public string? TargetEntityType { get; set; }

    public string? TargetEntityId { get; set; }

    [Required]
    [StringLength(100)]
    public string ActionType { get; set; } = string.Empty;

    public string? Details { get; set; }

    public DateTime Timestamp { get; set; }

    // Navigation properties
    public ApplicationUser? User { get; set; }
}