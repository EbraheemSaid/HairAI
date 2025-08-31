using System.ComponentModel.DataAnnotations;
using HairAI.Domain.Enums;

namespace HairAI.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; }

    public Guid SubscriptionId { get; set; }

    public decimal Amount { get; set; }

    [Required]
    [StringLength(3)]
    public string Currency { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = PaymentStatus.Succeeded.ToString();

    public string? PaymentGatewayReference { get; set; }

    public DateTime ProcessedAt { get; set; }

    // Navigation properties
    public Subscription Subscription { get; set; } = null!;
}