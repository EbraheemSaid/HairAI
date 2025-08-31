namespace HairAI.Application.Features.Admin.Commands.ToggleUserStatus;

public class ToggleUserStatusCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public bool IsActive { get; set; }
}

