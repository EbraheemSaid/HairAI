using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Domain.Entities;

namespace HairAI.Application.Features.Admin.Commands.ManuallyCreateClinic;

public class ManuallyCreateClinicCommandHandler : IRequestHandler<ManuallyCreateClinicCommand, ManuallyCreateClinicCommandResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;

    public ManuallyCreateClinicCommandHandler(IApplicationDbContext context, IClinicAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<ManuallyCreateClinicCommandResponse> Handle(ManuallyCreateClinicCommand request, CancellationToken cancellationToken)
    {
        // SECURITY: Only SuperAdmin can manually create clinics
        if (!await _authorizationService.IsSuperAdminAsync())
        {
            return new ManuallyCreateClinicCommandResponse
            {
                Success = false,
                Message = "Access denied. Only SuperAdmin can manually create clinics.",
                Errors = new List<string> { "Insufficient permissions for clinic creation" }
            };
        }

        var clinic = new Clinic
        {
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Clinics.Add(clinic);
        await _context.SaveChangesAsync(cancellationToken);

        return new ManuallyCreateClinicCommandResponse
        {
            Success = true,
            Message = "Clinic created successfully",
            ClinicId = clinic.Id
        };
    }
}