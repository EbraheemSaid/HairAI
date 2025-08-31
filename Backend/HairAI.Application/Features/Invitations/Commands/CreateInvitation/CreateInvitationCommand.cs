using MediatR;

namespace HairAI.Application.Features.Invitations.Commands.CreateInvitation;

public class CreateInvitationCommand : IRequest<CreateInvitationCommandResponse>
{
    public Guid ClinicId { get; set; }
    public string InvitedByUserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}