using MediatR;
using HairAI.Application.Common.Interfaces;

namespace HairAI.Application.Features.Subscriptions.Commands.CancelSubscription;

public class CancelSubscriptionCommandHandler : IRequestHandler<CancelSubscriptionCommand, CancelSubscriptionCommandResponse>
{
    private readonly IApplicationDbContext _context;

    public CancelSubscriptionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CancelSubscriptionCommandResponse> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions.FindAsync(new object[] { request.SubscriptionId }, cancellationToken);
        
        if (subscription == null)
        {
            return new CancelSubscriptionCommandResponse
            {
                Success = false,
                Message = "Subscription not found",
                Errors = new List<string> { "Subscription not found" }
            };
        }

        subscription.Status = "canceled";
        subscription.CurrentPeriodEnd = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new CancelSubscriptionCommandResponse
        {
            Success = true,
            Message = "Subscription canceled successfully"
        };
    }
}