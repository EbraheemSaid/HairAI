using MediatR;

namespace HairAI.Application.Features.Analysis.Commands.AddDoctorNotes;

public class AddDoctorNotesCommand : IRequest<AddDoctorNotesCommandResponse>
{
    public Guid JobId { get; set; }
    public string DoctorNotes { get; set; } = string.Empty;
}