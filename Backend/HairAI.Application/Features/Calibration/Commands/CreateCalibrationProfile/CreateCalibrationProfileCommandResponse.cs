using MediatR;

namespace HairAI.Application.Features.Calibration.Commands.CreateCalibrationProfile;

public class CreateCalibrationProfileCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Guid? ProfileId { get; set; }
}