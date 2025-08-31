using MediatR;

namespace HairAI.Application.Features.Analysis.Commands.GenerateFinalReport;

public class GenerateFinalReportCommand : IRequest<GenerateFinalReportCommandResponse>
{
    public Guid SessionId { get; set; }
}