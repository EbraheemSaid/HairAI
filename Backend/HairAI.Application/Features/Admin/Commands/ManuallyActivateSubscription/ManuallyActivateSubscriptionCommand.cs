using MediatR;

namespace HairAI.Application.Features.Admin.Commands.ManuallyActivateSubscription;

public class ManuallyActivateSubscriptionCommand : IRequest<ManuallyActivateSubscriptionCommandResponse>
{
    public Guid ClinicId { get; set; }
    public Guid PlanId { get; set; }
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
}