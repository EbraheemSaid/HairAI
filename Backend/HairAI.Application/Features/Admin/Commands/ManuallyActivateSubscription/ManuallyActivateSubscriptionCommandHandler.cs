using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Domain.Entities;

namespace HairAI.Application.Features.Admin.Commands.ManuallyActivateSubscription;

public class ManuallyActivateSubscriptionCommandHandler : IRequestHandler<ManuallyActivateSubscriptionCommand, ManuallyActivateSubscriptionCommandResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;

    public ManuallyActivateSubscriptionCommandHandler(IApplicationDbContext context, IClinicAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<ManuallyActivateSubscriptionCommandResponse> Handle(ManuallyActivateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        // SECURITY: Only SuperAdmin can manually activate subscriptions
        if (!await _authorizationService.IsSuperAdminAsync())
        {
            return new ManuallyActivateSubscriptionCommandResponse
            {
                Success = false,
                Message = "Access denied. Only SuperAdmin can manually activate subscriptions.",
                Errors = new List<string> { "Insufficient permissions for subscription operations" }
            };
        }

        var subscription = new Subscription
        {
            ClinicId = request.ClinicId,
            PlanId = request.PlanId,
            Status = "active",
            CurrentPeriodStart = request.CurrentPeriodStart,
            CurrentPeriodEnd = request.CurrentPeriodEnd
        };

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync(cancellationToken);

        return new ManuallyActivateSubscriptionCommandResponse
        {
            Success = true,
            Message = "Subscription activated successfully",
            SubscriptionId = subscription.Id
        };
    }
}