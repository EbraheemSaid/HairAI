namespace HairAI.Application.Features.Admin.Commands.DeleteUser;

public class DeleteUserCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}

