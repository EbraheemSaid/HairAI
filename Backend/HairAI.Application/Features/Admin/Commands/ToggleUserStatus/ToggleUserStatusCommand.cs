using MediatR;

namespace HairAI.Application.Features.Admin.Commands.ToggleUserStatus;

public class ToggleUserStatusCommand : IRequest<ToggleUserStatusCommandResponse>
{
    public string UserId { get; set; } = string.Empty;
    public bool IsActivating { get; set; } // true for activate, false for deactivate
}

