using MediatR;
using HairAI.Application.DTOs;

namespace HairAI.Application.Features.Subscriptions.Queries.GetAllPlans;

public class GetAllPlansQueryResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public List<SubscriptionPlanDto> Plans { get; set; } = new();
}