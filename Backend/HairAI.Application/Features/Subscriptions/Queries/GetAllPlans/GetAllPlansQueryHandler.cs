using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HairAI.Application.Features.Subscriptions.Queries.GetAllPlans;

public class GetAllPlansQueryHandler : IRequestHandler<GetAllPlansQuery, GetAllPlansQueryResponse>
{
    private readonly IApplicationDbContext _context;

    public GetAllPlansQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetAllPlansQueryResponse> Handle(GetAllPlansQuery request, CancellationToken cancellationToken)
    {
        var plans = await _context.SubscriptionPlans
            .Select(p => new SubscriptionPlanDto
            {
                Id = p.Id,
                Name = p.Name,
                PriceMonthly = p.PriceMonthly,
                Currency = p.Currency,
                MaxUsers = p.MaxUsers,
                MaxAnalysesPerMonth = p.MaxAnalysesPerMonth
            })
            .ToListAsync(cancellationToken);

        return new GetAllPlansQueryResponse
        {
            Success = true,
            Message = "Subscription plans retrieved successfully",
            Plans = plans
        };
    }
}