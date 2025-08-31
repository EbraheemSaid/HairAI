using MediatR;

namespace HairAI.Application.Features.Invitations.Queries.GetInvitationByToken;

public class GetInvitationByTokenQuery : IRequest<GetInvitationByTokenQueryResponse>
{
    public string Token { get; set; } = string.Empty;
}