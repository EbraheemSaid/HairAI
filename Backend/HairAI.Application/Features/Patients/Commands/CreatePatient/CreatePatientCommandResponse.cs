using MediatR;

namespace HairAI.Application.Features.Patients.Commands.CreatePatient;

public class CreatePatientCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Guid? PatientId { get; set; }
}