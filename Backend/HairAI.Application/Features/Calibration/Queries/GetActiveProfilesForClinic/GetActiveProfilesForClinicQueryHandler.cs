using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HairAI.Application.Features.Calibration.Queries.GetActiveProfilesForClinic;

public class GetActiveProfilesForClinicQueryHandler : IRequestHandler<GetActiveProfilesForClinicQuery, GetActiveProfilesForClinicQueryResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;

    public GetActiveProfilesForClinicQueryHandler(IApplicationDbContext context, IClinicAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<GetActiveProfilesForClinicQueryResponse> Handle(GetActiveProfilesForClinicQuery request, CancellationToken cancellationToken)
    {
        // SECURITY: Check if user can access this clinic's calibration profiles
        if (!await _authorizationService.CanAccessClinicAsync(request.ClinicId))
        {
            return new GetActiveProfilesForClinicQueryResponse
            {
                Success = false,
                Message = "Access denied. You cannot access calibration profiles for this clinic.",
                Errors = new List<string> { "Unauthorized clinic access" }
            };
        }

        var profiles = await _context.CalibrationProfiles
            .Where(cp => cp.ClinicId == request.ClinicId && cp.IsActive)
            .Select(cp => new CalibrationProfileDto
            {
                Id = cp.Id,
                ClinicId = cp.ClinicId,
                ProfileName = cp.ProfileName,
                CalibrationData = cp.CalibrationData.RootElement.ToString(),
                Version = cp.Version,
                IsActive = cp.IsActive,
                CreatedAt = cp.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new GetActiveProfilesForClinicQueryResponse
        {
            Success = true,
            Message = "Active calibration profiles retrieved successfully",
            Profiles = profiles
        };
    }
}