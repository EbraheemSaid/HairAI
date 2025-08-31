using MediatR;
using HairAI.Application.Common.Interfaces;

namespace HairAI.Application.Features.Patients.Commands.UpdatePatient;

public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, UpdatePatientCommandResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;

    public UpdatePatientCommandHandler(IApplicationDbContext context, IClinicAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<UpdatePatientCommandResponse> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        // SECURITY: Check if user can access and modify this patient
        if (!await _authorizationService.CanAccessPatientAsync(request.PatientId))
        {
            return new UpdatePatientCommandResponse
            {
                Success = false,
                Message = "Access denied. You cannot modify this patient's data.",
                Errors = new List<string> { "Unauthorized access to patient data" }
            };
        }

        var patient = await _context.Patients.FindAsync(new object[] { request.PatientId }, cancellationToken);
        
        if (patient == null)
        {
            return new UpdatePatientCommandResponse
            {
                Success = false,
                Message = "Patient not found",
                Errors = new List<string> { "Patient not found" }
            };
        }

        patient.ClinicPatientId = request.ClinicPatientId;
        patient.FirstName = request.FirstName;
        patient.LastName = request.LastName;
        patient.DateOfBirth = request.DateOfBirth;

        await _context.SaveChangesAsync(cancellationToken);

        return new UpdatePatientCommandResponse
        {
            Success = true,
            Message = "Patient updated successfully"
        };
    }
}