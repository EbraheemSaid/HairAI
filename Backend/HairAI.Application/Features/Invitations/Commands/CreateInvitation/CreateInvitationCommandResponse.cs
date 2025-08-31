using MediatR;

namespace HairAI.Application.Features.Invitations.Commands.CreateInvitation;

public class CreateInvitationCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Guid? InvitationId { get; set; }
    public string? InvitationToken { get; set; }
}