using MediatR;

namespace HairAI.Application.Features.Admin.Commands.ManuallyCreateClinic;

public class ManuallyCreateClinicCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Guid? ClinicId { get; set; }
}