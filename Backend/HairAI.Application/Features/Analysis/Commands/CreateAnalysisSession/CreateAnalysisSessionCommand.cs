using MediatR;

namespace HairAI.Application.Features.Analysis.Commands.CreateAnalysisSession;

public class CreateAnalysisSessionCommand : IRequest<CreateAnalysisSessionCommandResponse>
{
    public Guid PatientId { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public DateOnly SessionDate { get; set; }
}