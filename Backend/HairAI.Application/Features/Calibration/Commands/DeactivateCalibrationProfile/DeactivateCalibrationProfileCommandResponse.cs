using MediatR;

namespace HairAI.Application.Features.Calibration.Commands.DeactivateCalibrationProfile;

public class DeactivateCalibrationProfileCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}