using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HairAI.Application.Features.Analysis.Commands.UploadAnalysisImage;

public class UploadAnalysisImageCommandHandler : IRequestHandler<UploadAnalysisImageCommand, UploadAnalysisImageCommandResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IQueueService _queueService;
    private readonly IClinicAuthorizationService _authorizationService;
    private readonly ILogger<UploadAnalysisImageCommandHandler> _logger;

    public UploadAnalysisImageCommandHandler(
        IApplicationDbContext context, 
        IQueueService queueService, 
        IClinicAuthorizationService authorizationService,
        ILogger<UploadAnalysisImageCommandHandler> logger)
    {
        _context = context;
        _queueService = queueService;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<UploadAnalysisImageCommandResponse> Handle(UploadAnalysisImageCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Uploading analysis image for patient {PatientId}, session {SessionId}", 
            request.PatientId, request.SessionId);

        // SECURITY: Check if user can access this patient and create analysis jobs
        if (!await _authorizationService.CanAccessPatientAsync(request.PatientId))
        {
            _logger.LogWarning("Unauthorized attempt to upload image for patient {PatientId} by user {UserId}", 
                request.PatientId, request.CreatedByUserId);
            return new UploadAnalysisImageCommandResponse
            {
                Success = false,
                Message = "Access denied. You cannot upload images for this patient.",
                Errors = new List<string> { "Unauthorized access to patient data" }
            };
        }

        // SECURITY: Validate session belongs to the same patient
        var session = await _context.AnalysisSessions.FindAsync(new object[] { request.SessionId }, cancellationToken);
        if (session == null || session.PatientId != request.PatientId)
        {
            _logger.LogWarning("Invalid session or patient mismatch for session {SessionId}, patient {PatientId}", 
                request.SessionId, request.PatientId);
            return new UploadAnalysisImageCommandResponse
            {
                Success = false,
                Message = "Invalid session or patient mismatch.",
                Errors = new List<string> { "Session-patient validation failed" }
            };
        }

        // SECURITY: Validate calibration profile belongs to user's clinic
        if (!await _authorizationService.CanAccessCalibrationProfileAsync(request.CalibrationProfileId))
        {
            _logger.LogWarning("Unauthorized attempt to use calibration profile {CalibrationProfileId} by user {UserId}", 
                request.CalibrationProfileId, request.CreatedByUserId);
            return new UploadAnalysisImageCommandResponse
            {
                Success = false,
                Message = "Access denied. Invalid calibration profile.",
                Errors = new List<string> { "Unauthorized calibration profile access" }
            };
        }

        // SECURITY: Validate image storage key format (prevent path traversal)
        if (string.IsNullOrEmpty(request.ImageStorageKey) || 
            request.ImageStorageKey.Contains("..") || 
            request.ImageStorageKey.Contains("\\") ||
            request.ImageStorageKey.Length > 255)
        {
            _logger.LogWarning("Invalid image storage key format: {ImageStorageKey}", request.ImageStorageKey);
            return new UploadAnalysisImageCommandResponse
            {
                Success = false,
                Message = "Invalid image storage key format.",
                Errors = new List<string> { "Image storage key validation failed" }
            };
        }

        // SECURITY: Validate location tag
        if (string.IsNullOrEmpty(request.LocationTag) || request.LocationTag.Length > 100)
        {
            _logger.LogWarning("Invalid location tag: {LocationTag}", request.LocationTag);
            return new UploadAnalysisImageCommandResponse
            {
                Success = false,
                Message = "Invalid location tag. Must be between 1 and 100 characters.",
                Errors = new List<string> { "Location tag validation failed" }
            };
        }

        var job = new AnalysisJob
        {
            SessionId = request.SessionId,
            PatientId = request.PatientId,
            CalibrationProfileId = request.CalibrationProfileId,
            CreatedByUserId = request.CreatedByUserId,
            LocationTag = request.LocationTag,
            ImageStorageKey = request.ImageStorageKey,
            Status = HairAI.Domain.Enums.JobStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.AnalysisJobs.Add(job);
        
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Analysis job {JobId} created successfully", job.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create analysis job for patient {PatientId}", request.PatientId);
            return new UploadAnalysisImageCommandResponse
            {
                Success = false,
                Message = "Failed to create analysis job.",
                Errors = new List<string> { "Database error" }
            };
        }

        // Publish the job to the queue for processing
        try
        {
            await _queueService.PublishAnalysisJobAsync(job.Id);
            _logger.LogInformation("Analysis job {JobId} published to queue successfully", job.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish analysis job {JobId} to queue", job.Id);
            // Don't fail the entire operation if queue publishing fails, but log it
            return new UploadAnalysisImageCommandResponse
            {
                Success = true,
                Message = "Analysis job created but failed to queue for processing. Please contact administrator.",
                JobId = job.Id,
                Errors = new List<string> { "Queue publishing failed" }
            };
        }

        return new UploadAnalysisImageCommandResponse
        {
            Success = true,
            Message = "Analysis job created and queued successfully",
            JobId = job.Id
        };
    }
}