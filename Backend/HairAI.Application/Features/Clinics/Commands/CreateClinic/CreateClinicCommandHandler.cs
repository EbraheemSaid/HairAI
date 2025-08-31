using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Domain.Entities;

namespace HairAI.Application.Features.Clinics.Commands.CreateClinic;

public class CreateClinicCommandHandler : IRequestHandler<CreateClinicCommand, CreateClinicCommandResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;

    public CreateClinicCommandHandler(IApplicationDbContext context, IClinicAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<CreateClinicCommandResponse> Handle(CreateClinicCommand request, CancellationToken cancellationToken)
    {
        // SECURITY: Only SuperAdmin can create clinics
        if (!await _authorizationService.IsSuperAdminAsync())
        {
            return new CreateClinicCommandResponse
            {
                Success = false,
                Message = "Access denied. Only SuperAdmin can create clinics.",
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

        return new CreateClinicCommandResponse
        {
            Success = true,
            Message = "Clinic created successfully",
            ClinicId = clinic.Id
        };
    }
}