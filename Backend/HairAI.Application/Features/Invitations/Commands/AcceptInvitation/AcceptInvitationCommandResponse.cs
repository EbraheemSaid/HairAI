using MediatR;

namespace HairAI.Application.Features.Invitations.Commands.AcceptInvitation;

public class AcceptInvitationCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}