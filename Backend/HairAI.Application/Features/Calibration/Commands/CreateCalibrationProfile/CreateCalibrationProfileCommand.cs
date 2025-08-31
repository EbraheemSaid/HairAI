using MediatR;
using System.Text.Json;

namespace HairAI.Application.Features.Calibration.Commands.CreateCalibrationProfile;

public class CreateCalibrationProfileCommand : IRequest<CreateCalibrationProfileCommandResponse>
{
    public Guid ClinicId { get; set; }
    public string ProfileName { get; set; } = string.Empty;
    public JsonDocument CalibrationData { get; set; } = null!;
}