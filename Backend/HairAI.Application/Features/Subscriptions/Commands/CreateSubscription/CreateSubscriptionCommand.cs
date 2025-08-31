using MediatR;

namespace HairAI.Application.Features.Subscriptions.Commands.CreateSubscription;

public class CreateSubscriptionCommand : IRequest<CreateSubscriptionCommandResponse>
{
    public Guid ClinicId { get; set; }
    public Guid PlanId { get; set; }
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
}