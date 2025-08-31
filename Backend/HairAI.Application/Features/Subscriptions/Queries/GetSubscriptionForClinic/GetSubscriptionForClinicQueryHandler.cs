using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HairAI.Application.Features.Subscriptions.Queries.GetSubscriptionForClinic;

public class GetSubscriptionForClinicQueryHandler : IRequestHandler<GetSubscriptionForClinicQuery, GetSubscriptionForClinicQueryResponse>
{
    private readonly IApplicationDbContext _context;

    public GetSubscriptionForClinicQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetSubscriptionForClinicQueryResponse> Handle(GetSubscriptionForClinicQuery request, CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions
            .Where(s => s.ClinicId == request.ClinicId)
            .Select(s => new SubscriptionDto
            {
                Id = s.Id,
                ClinicId = s.ClinicId,
                PlanId = s.PlanId,
                Status = s.Status,
                CurrentPeriodStart = s.CurrentPeriodStart,
                CurrentPeriodEnd = s.CurrentPeriodEnd
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (subscription == null)
        {
            return new GetSubscriptionForClinicQueryResponse
            {
                Success = false,
                Message = "Subscription not found for this clinic",
                Errors = new List<string> { "Subscription not found for this clinic" }
            };
        }

        return new GetSubscriptionForClinicQueryResponse
        {
            Success = true,
            Message = "Subscription retrieved successfully",
            Subscription = subscription
        };
    }
}