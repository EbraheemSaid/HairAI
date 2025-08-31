using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Application.DTOs;

namespace HairAI.Application.Features.Clinics.Queries.GetClinicById;

public class GetClinicByIdQueryHandler : IRequestHandler<GetClinicByIdQuery, GetClinicByIdQueryResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;

    public GetClinicByIdQueryHandler(IApplicationDbContext context, IClinicAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<GetClinicByIdQueryResponse> Handle(GetClinicByIdQuery request, CancellationToken cancellationToken)
    {
        // SECURITY: Check if user can access this clinic
        if (!await _authorizationService.CanAccessClinicAsync(request.ClinicId))
        {
            return new GetClinicByIdQueryResponse
            {
                Success = false,
                Message = "Access denied. You cannot access this clinic.",
                Errors = new List<string> { "Unauthorized access to clinic data" }
            };
        }

        var clinic = await _context.Clinics.FindAsync(new object[] { request.ClinicId }, cancellationToken);
        
        if (clinic == null)
        {
            return new GetClinicByIdQueryResponse
            {
                Success = false,
                Message = "Clinic not found",
                Errors = new List<string> { "Clinic not found" }
            };
        }

        var clinicDto = new ClinicDto
        {
            Id = clinic.Id,
            Name = clinic.Name,
            CreatedAt = clinic.CreatedAt,
            UpdatedAt = clinic.UpdatedAt
        };

        return new GetClinicByIdQueryResponse
        {
            Success = true,
            Message = "Clinic retrieved successfully",
            Clinic = clinicDto
        };
    }
}