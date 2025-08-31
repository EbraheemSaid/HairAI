using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Application.DTOs;

namespace HairAI.Application.Features.Patients.Queries.GetPatientById;

public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, GetPatientByIdQueryResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;

    public GetPatientByIdQueryHandler(IApplicationDbContext context, IClinicAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<GetPatientByIdQueryResponse> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        // SECURITY: Check if user can access this patient
        if (!await _authorizationService.CanAccessPatientAsync(request.PatientId))
        {
            return new GetPatientByIdQueryResponse
            {
                Success = false,
                Message = "Access denied. You cannot access this patient.",
                Errors = new List<string> { "Unauthorized access to patient data" }
            };
        }

        var patient = await _context.Patients.FindAsync(new object[] { request.PatientId }, cancellationToken);
        
        if (patient == null)
        {
            return new GetPatientByIdQueryResponse
            {
                Success = false,
                Message = "Patient not found",
                Errors = new List<string> { "Patient not found" }
            };
        }

        var patientDto = new PatientDto
        {
            Id = patient.Id,
            ClinicId = patient.ClinicId,
            ClinicPatientId = patient.ClinicPatientId,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth,
            CreatedAt = patient.CreatedAt
        };

        return new GetPatientByIdQueryResponse
        {
            Success = true,
            Message = "Patient retrieved successfully",
            Patient = patientDto
        };
    }
}