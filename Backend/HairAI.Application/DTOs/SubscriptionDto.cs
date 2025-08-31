namespace HairAI.Application.DTOs;

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid PlanId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? CurrentPeriodStart { get; set; }
    public DateTime? CurrentPeriodEnd { get; set; }
}