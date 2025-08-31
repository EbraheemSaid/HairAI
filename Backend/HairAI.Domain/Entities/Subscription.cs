using System.ComponentModel.DataAnnotations;
using HairAI.Domain.Enums;

namespace HairAI.Domain.Entities;

public class Subscription
{
    public Guid Id { get; set; }

    public Guid ClinicId { get; set; }

    public Guid PlanId { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = SubscriptionStatus.Active.ToString();

    public DateTime? CurrentPeriodStart { get; set; }

    public DateTime? CurrentPeriodEnd { get; set; }

    // Navigation properties
    public Clinic Clinic { get; set; } = null!;
    public SubscriptionPlan Plan { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}