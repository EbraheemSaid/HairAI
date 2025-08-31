using MediatR;

namespace HairAI.Application.Features.Calibration.Commands.DeactivateCalibrationProfile;

public class DeactivateCalibrationProfileCommand : IRequest<DeactivateCalibrationProfileCommandResponse>
{
    public Guid ProfileId { get; set; }
}