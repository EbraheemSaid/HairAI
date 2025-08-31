using MediatR;

namespace HairAI.Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<RegisterCommandResponse>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid? ClinicId { get; set; }
}