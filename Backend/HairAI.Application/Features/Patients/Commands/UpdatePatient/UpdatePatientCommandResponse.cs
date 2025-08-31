using MediatR;

namespace HairAI.Application.Features.Patients.Commands.UpdatePatient;

public class UpdatePatientCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}