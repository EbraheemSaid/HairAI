using MediatR;

namespace HairAI.Application.Features.Subscriptions.Commands.CancelSubscription;

public class CancelSubscriptionCommand : IRequest<CancelSubscriptionCommandResponse>
{
    public Guid SubscriptionId { get; set; }
}