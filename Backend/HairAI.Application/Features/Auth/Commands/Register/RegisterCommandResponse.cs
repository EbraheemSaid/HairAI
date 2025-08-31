using MediatR;

namespace HairAI.Application.Features.Auth.Commands.Register;

public class RegisterCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid? ClinicId { get; set; }
    public string Role { get; set; } = string.Empty;
}