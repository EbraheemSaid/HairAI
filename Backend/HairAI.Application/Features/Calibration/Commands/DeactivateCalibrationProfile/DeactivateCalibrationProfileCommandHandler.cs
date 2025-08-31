using MediatR;
using HairAI.Application.Common.Interfaces;

namespace HairAI.Application.Features.Calibration.Commands.DeactivateCalibrationProfile;

public class DeactivateCalibrationProfileCommandHandler : IRequestHandler<DeactivateCalibrationProfileCommand, DeactivateCalibrationProfileCommandResponse>
{
    private readonly IApplicationDbContext _context;

    public DeactivateCalibrationProfileCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DeactivateCalibrationProfileCommandResponse> Handle(DeactivateCalibrationProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _context.CalibrationProfiles.FindAsync(new object[] { request.ProfileId }, cancellationToken);
        
        if (profile == null)
        {
            return new DeactivateCalibrationProfileCommandResponse
            {
                Success = false,
                Message = "Calibration profile not found",
                Errors = new List<string> { "Calibration profile not found" }
            };
        }

        profile.IsActive = false;
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new DeactivateCalibrationProfileCommandResponse
        {
            Success = true,
            Message = "Calibration profile deactivated successfully"
        };
    }
}