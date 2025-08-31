using MediatR;

namespace HairAI.Application.Features.Analysis.Commands.AddDoctorNotes;

public class AddDoctorNotesCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}