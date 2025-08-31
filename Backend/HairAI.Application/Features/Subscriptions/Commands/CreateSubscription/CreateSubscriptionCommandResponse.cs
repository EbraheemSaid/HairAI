using MediatR;

namespace HairAI.Application.Features.Subscriptions.Commands.CreateSubscription;

public class CreateSubscriptionCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Guid? SubscriptionId { get; set; }
    public string? PaymentKey { get; set; }
    public string? RedirectUrl { get; set; }
}