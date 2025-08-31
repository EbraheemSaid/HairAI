using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HairAI.Application.Features.Calibration.Commands.CreateCalibrationProfile;

public class CreateCalibrationProfileCommandHandler : IRequestHandler<CreateCalibrationProfileCommand, CreateCalibrationProfileCommandResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;

    public CreateCalibrationProfileCommandHandler(IApplicationDbContext context, IClinicAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<CreateCalibrationProfileCommandResponse> Handle(CreateCalibrationProfileCommand request, CancellationToken cancellationToken)
    {
        // SECURITY: Check if user can create calibration profiles for this clinic
        if (!await _authorizationService.CanAccessClinicAsync(request.ClinicId))
        {
            return new CreateCalibrationProfileCommandResponse
            {
                Success = false,
                Message = "Access denied. You cannot create calibration profiles for this clinic.",
                Errors = new List<string> { "Unauthorized clinic access" }
            };
        }

        // Deactivate any existing active profiles with the same name
        var existingProfiles = await _context.CalibrationProfiles
            .Where(cp => cp.ClinicId == request.ClinicId && 
                         cp.ProfileName == request.ProfileName && 
                         cp.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var profile in existingProfiles)
        {
            profile.IsActive = false;
        }

        // Create the new profile
        var calibrationProfile = new CalibrationProfile
        {
            ClinicId = request.ClinicId,
            ProfileName = request.ProfileName,
            CalibrationData = request.CalibrationData,
            Version = existingProfiles.Any() ? existingProfiles.Max(cp => cp.Version) + 1 : 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.CalibrationProfiles.Add(calibrationProfile);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateCalibrationProfileCommandResponse
        {
            Success = true,
            Message = "Calibration profile created successfully",
            ProfileId = calibrationProfile.Id
        };
    }
}