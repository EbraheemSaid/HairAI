using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HairAI.Application.Features.Patients.Commands.CreatePatient;

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, CreatePatientCommandResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;

    public CreatePatientCommandHandler(IApplicationDbContext context, IClinicAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<CreatePatientCommandResponse> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        // SECURITY: Check if user can create patients for this clinic
        if (!await _authorizationService.CanAccessClinicAsync(request.ClinicId))
        {
            return new CreatePatientCommandResponse
            {
                Success = false,
                Message = "Access denied. You cannot create patients for this clinic.",
                Errors = new List<string> { "Unauthorized clinic access" }
            };
        }

        // CRITICAL BUG FIX: Check for duplicate ClinicPatientId within the same clinic
        if (!string.IsNullOrEmpty(request.ClinicPatientId))
        {
            var existingPatient = await _context.Patients
                .FirstOrDefaultAsync(p => p.ClinicId == request.ClinicId && 
                                         p.ClinicPatientId == request.ClinicPatientId, 
                                    cancellationToken);
                                    
            if (existingPatient != null)
            {
                return new CreatePatientCommandResponse
                {
                    Success = false,
                    Message = "A patient with this clinic patient ID already exists.",
                    Errors = new List<string> { "Duplicate clinic patient ID" }
                };
            }
        }

        // EDGE CASE: Validate date of birth is reasonable
        if (request.DateOfBirth.HasValue)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (request.DateOfBirth > today)
            {
                return new CreatePatientCommandResponse
                {
                    Success = false,
                    Message = "Date of birth cannot be in the future.",
                    Errors = new List<string> { "Invalid date of birth" }
                };
            }
            
            var minDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-150));
            if (request.DateOfBirth < minDate)
            {
                return new CreatePatientCommandResponse
                {
                    Success = false,
                    Message = "Date of birth is unrealistic (more than 150 years ago).",
                    Errors = new List<string> { "Invalid date of birth" }
                };
            }
        }

        var patient = new Patient
        {
            ClinicId = request.ClinicId,
            ClinicPatientId = request.ClinicPatientId,
            FirstName = request.FirstName?.Trim(),
            LastName = request.LastName?.Trim(),
            DateOfBirth = request.DateOfBirth,
            CreatedAt = DateTime.UtcNow
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreatePatientCommandResponse
        {
            Success = true,
            Message = "Patient created successfully",
            PatientId = patient.Id
        };
    }
}