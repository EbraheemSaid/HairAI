using MediatR;

namespace HairAI.Application.Features.Admin.Commands.ManuallyActivateSubscription;

public class ManuallyActivateSubscriptionCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Guid? SubscriptionId { get; set; }
}