using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HairAI.Application.Features.Patients.Queries.GetPatientsForClinic;

public class GetPatientsForClinicQueryHandler : IRequestHandler<GetPatientsForClinicQuery, GetPatientsForClinicQueryResponse>
{
    private readonly IApplicationDbContext _context;

    public GetPatientsForClinicQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetPatientsForClinicQueryResponse> Handle(GetPatientsForClinicQuery request, CancellationToken cancellationToken)
    {
        var patients = await _context.Patients
            .Where(p => p.ClinicId == request.ClinicId)
            .Select(p => new PatientDto
            {
                Id = p.Id,
                ClinicId = p.ClinicId,
                ClinicPatientId = p.ClinicPatientId,
                FirstName = p.FirstName,
                LastName = p.LastName,
                DateOfBirth = p.DateOfBirth,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new GetPatientsForClinicQueryResponse
        {
            Success = true,
            Message = "Patients retrieved successfully",
            Patients = patients
        };
    }
}