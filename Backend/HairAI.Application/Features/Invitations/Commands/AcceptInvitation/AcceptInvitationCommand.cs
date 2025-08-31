using MediatR;

namespace HairAI.Application.Features.Invitations.Commands.AcceptInvitation;

public class AcceptInvitationCommand : IRequest<AcceptInvitationCommandResponse>
{
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}