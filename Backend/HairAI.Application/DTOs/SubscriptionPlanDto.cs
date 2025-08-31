namespace HairAI.Application.DTOs;

public class SubscriptionPlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceMonthly { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int MaxUsers { get; set; }
    public int MaxAnalysesPerMonth { get; set; }
}