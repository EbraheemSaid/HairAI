using MediatR;

namespace HairAI.Application.Features.Analysis.Commands.UploadAnalysisImage;

public class UploadAnalysisImageCommand : IRequest<UploadAnalysisImageCommandResponse>
{
    public Guid SessionId { get; set; }
    public Guid PatientId { get; set; }
    public Guid CalibrationProfileId { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public string LocationTag { get; set; } = string.Empty;
    public string ImageStorageKey { get; set; } = string.Empty;
}