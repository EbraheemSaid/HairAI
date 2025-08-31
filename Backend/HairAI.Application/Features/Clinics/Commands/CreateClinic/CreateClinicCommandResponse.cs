using MediatR;

namespace HairAI.Application.Features.Clinics.Commands.CreateClinic;

public class CreateClinicCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Guid? ClinicId { get; set; }
}