using MediatR;

namespace HairAI.Application.Features.Subscriptions.Commands.CancelSubscription;

public class CancelSubscriptionCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}