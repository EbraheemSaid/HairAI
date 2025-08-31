using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HairAI.Application.Features.Clinics.Queries.GetAllClinics;

public class GetAllClinicsQueryHandler : IRequestHandler<GetAllClinicsQuery, GetAllClinicsQueryResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;

    public GetAllClinicsQueryHandler(IApplicationDbContext context, IClinicAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<GetAllClinicsQueryResponse> Handle(GetAllClinicsQuery request, CancellationToken cancellationToken)
    {
        // SECURITY: Get only clinics the user has access to
        var accessibleClinicIds = await _authorizationService.GetUserAccessibleClinicsAsync();
        
        // MEMORY SAFETY: Limit number of clinics to prevent memory issues
        if (accessibleClinicIds.Count > 1000)
        {
            return new GetAllClinicsQueryResponse
            {
                Success = false,
                Message = "Too many accessible clinics. Please contact administrator.",
                Errors = new List<string> { "Clinic count exceeds safe processing limit" },
                Clinics = new List<ClinicDto>()
            };
        }
        
        var clinics = await _context.Clinics
            .Where(c => accessibleClinicIds.Contains(c.Id))
            .OrderBy(c => c.Name) // PERFORMANCE: Add predictable ordering
            .Take(500) // MEMORY SAFETY: Hard limit to prevent excessive memory usage
            .Select(c => new ClinicDto
            {
                Id = c.Id,
                Name = c.Name.Length > 200 ? c.Name.Substring(0, 200) : c.Name, // MEMORY SAFETY: Truncate long names
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new GetAllClinicsQueryResponse
        {
            Success = true,
            Message = $"Retrieved {clinics.Count} accessible clinics",
            Clinics = clinics
        };
    }
}