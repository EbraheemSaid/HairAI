using MediatR;
using HairAI.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HairAI.Application.Features.Calibration.Commands.UpdateCalibrationProfile;

public class UpdateCalibrationProfileCommandHandler : IRequestHandler<UpdateCalibrationProfileCommand, UpdateCalibrationProfileCommandResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;

    public UpdateCalibrationProfileCommandHandler(IApplicationDbContext context, IClinicAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<UpdateCalibrationProfileCommandResponse> Handle(UpdateCalibrationProfileCommand request, CancellationToken cancellationToken)
    {
        // SECURITY: Check if user can access and modify this calibration profile
        if (!await _authorizationService.CanAccessCalibrationProfileAsync(request.ProfileId))
        {
            return new UpdateCalibrationProfileCommandResponse
            {
                Success = false,
                Message = "Access denied. You cannot modify this calibration profile.",
                Errors = new List<string> { "Unauthorized access to calibration profile" }
            };
        }

        var profile = await _context.CalibrationProfiles.FindAsync(new object[] { request.ProfileId }, cancellationToken);
        
        if (profile == null)
        {
            return new UpdateCalibrationProfileCommandResponse
            {
                Success = false,
                Message = "Calibration profile not found",
                Errors = new List<string> { "Calibration profile not found" }
            };
        }

        // If the profile name is changing, we need to handle versioning
        if (profile.ProfileName != request.ProfileName)
        {
            // Deactivate any existing active profiles with the new name
            var existingProfiles = await _context.CalibrationProfiles
                .Where(cp => cp.ClinicId == profile.ClinicId && 
                             cp.ProfileName == request.ProfileName && 
                             cp.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var existingProfile in existingProfiles)
            {
                existingProfile.IsActive = false;
            }

            // Update the profile
            profile.ProfileName = request.ProfileName;
            profile.Version = existingProfiles.Any() ? existingProfiles.Max(cp => cp.Version) + 1 : 1;
        }

        profile.CalibrationData = request.CalibrationData;
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateCalibrationProfileCommandResponse
        {
            Success = true,
            Message = "Calibration profile updated successfully"
        };
    }
}