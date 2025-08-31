using System.ComponentModel.DataAnnotations;

namespace HairAI.Domain.Entities;

public class SubscriptionPlan
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public decimal PriceMonthly { get; set; }

    [Required]
    [StringLength(3)]
    public string Currency { get; set; } = string.Empty;

    public int MaxUsers { get; set; }

    public int MaxAnalysesPerMonth { get; set; }

    // Navigation properties
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}