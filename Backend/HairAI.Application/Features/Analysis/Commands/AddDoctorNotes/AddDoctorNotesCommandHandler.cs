using MediatR;
using HairAI.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HairAI.Application.Features.Analysis.Commands.AddDoctorNotes;

public class AddDoctorNotesCommandHandler : IRequestHandler<AddDoctorNotesCommand, AddDoctorNotesCommandResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;
    private readonly ILogger<AddDoctorNotesCommandHandler> _logger;

    public AddDoctorNotesCommandHandler(
        IApplicationDbContext context, 
        IClinicAuthorizationService authorizationService,
        ILogger<AddDoctorNotesCommandHandler> logger)
    {
        _context = context;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<AddDoctorNotesCommandResponse> Handle(AddDoctorNotesCommand request, CancellationToken cancellationToken)
    {
        var job = await _context.AnalysisJobs.FindAsync(new object[] { request.JobId }, cancellationToken);
        
        if (job == null)
        {
            _logger.LogWarning("Attempt to add notes to non-existent analysis job {JobId}", request.JobId);
            return new AddDoctorNotesCommandResponse
            {
                Success = false,
                Message = "Analysis job not found",
                Errors = new List<string> { "Analysis job not found" }
            };
        }

        // SECURITY: Check if user can access and modify this analysis job
        // This validates through the patient-clinic relationship
        if (!await _authorizationService.CanAccessPatientAsync(job.PatientId))
        {
            _logger.LogWarning("Unauthorized attempt to add notes to analysis job {JobId} for patient {PatientId}", 
                request.JobId, job.PatientId);
            return new AddDoctorNotesCommandResponse
            {
                Success = false,
                Message = "Access denied. You cannot modify this analysis job.",
                Errors = new List<string> { "Unauthorized access to analysis job" }
            };
        }

        // SECURITY: Validate notes length
        if (request.DoctorNotes?.Length > 2000)
        {
            _logger.LogWarning("Attempt to add excessively long notes to analysis job {JobId}", request.JobId);
            return new AddDoctorNotesCommandResponse
            {
                Success = false,
                Message = "Doctor notes exceed maximum length of 2000 characters",
                Errors = new List<string> { "Notes too long" }
            };
        }

        job.DoctorNotes = request.DoctorNotes;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Doctor notes added successfully to analysis job {JobId}", request.JobId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save doctor notes to analysis job {JobId}", request.JobId);
            return new AddDoctorNotesCommandResponse
            {
                Success = false,
                Message = "Failed to save doctor notes",
                Errors = new List<string> { "Database error" }
            };
        }

        return new AddDoctorNotesCommandResponse
        {
            Success = true,
            Message = "Doctor notes added successfully"
        };
    }
}