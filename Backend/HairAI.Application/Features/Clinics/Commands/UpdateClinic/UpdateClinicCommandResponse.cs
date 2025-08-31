using MediatR;

namespace HairAI.Application.Features.Clinics.Commands.UpdateClinic;

public class UpdateClinicCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}