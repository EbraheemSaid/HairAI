using MediatR;

namespace HairAI.Application.Features.Admin.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<DeleteUserCommandResponse>
{
    public string UserId { get; set; } = string.Empty;
}

