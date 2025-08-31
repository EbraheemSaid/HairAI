using MediatR;

namespace HairAI.Application.Features.Subscriptions.Queries.GetSubscriptionForClinic;

public class GetSubscriptionForClinicQuery : IRequest<GetSubscriptionForClinicQueryResponse>
{
    public Guid ClinicId { get; set; }
}