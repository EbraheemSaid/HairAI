using MediatR;
using HairAI.Application.DTOs;

namespace HairAI.Application.Features.Subscriptions.Queries.GetSubscriptionForClinic;

public class GetSubscriptionForClinicQueryResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public SubscriptionDto? Subscription { get; set; }
}