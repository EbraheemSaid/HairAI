using MediatR;

namespace HairAI.Application.Features.Calibration.Commands.UpdateCalibrationProfile;

public class UpdateCalibrationProfileCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}