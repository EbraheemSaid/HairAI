using MediatR;
using System.Text.Json;

namespace HairAI.Application.Features.Calibration.Commands.UpdateCalibrationProfile;

public class UpdateCalibrationProfileCommand : IRequest<UpdateCalibrationProfileCommandResponse>
{
    public Guid ProfileId { get; set; }
    public string ProfileName { get; set; } = string.Empty;
    public JsonDocument CalibrationData { get; set; } = null!;
}