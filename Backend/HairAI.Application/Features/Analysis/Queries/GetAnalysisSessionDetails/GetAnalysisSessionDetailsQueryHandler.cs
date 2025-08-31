using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HairAI.Application.Features.Analysis.Queries.GetAnalysisSessionDetails;

public class GetAnalysisSessionDetailsQueryHandler : IRequestHandler<GetAnalysisSessionDetailsQuery, GetAnalysisSessionDetailsQueryResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;
    private readonly ILogger<GetAnalysisSessionDetailsQueryHandler> _logger;

    public GetAnalysisSessionDetailsQueryHandler(
        IApplicationDbContext context, 
        IClinicAuthorizationService authorizationService,
        ILogger<GetAnalysisSessionDetailsQueryHandler> logger)
    {
        _context = context;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<GetAnalysisSessionDetailsQueryResponse> Handle(GetAnalysisSessionDetailsQuery request, CancellationToken cancellationToken)
    {
        // SECURITY: Check if user can access this analysis session
        if (!await _authorizationService.CanAccessAnalysisSessionAsync(request.SessionId))
        {
            _logger.LogWarning("Unauthorized access attempt to analysis session {SessionId}", request.SessionId);
            return new GetAnalysisSessionDetailsQueryResponse
            {
                Success = false,
                Message = "Access denied. You cannot access this analysis session.",
                Errors = new List<string> { "Unauthorized access to analysis session data" }
            };
        }

        // PERFORMANCE: Check job count before loading to prevent memory issues
        var jobCount = await _context.AnalysisJobs
            .Where(j => j.SessionId == request.SessionId)
            .CountAsync(cancellationToken);

        _logger.LogInformation("Analysis session {SessionId} has {JobCount} jobs", request.SessionId, jobCount);

        // PERFORMANCE: More reasonable limits with better error handling
        if (jobCount > 2000)
        {
            _logger.LogWarning("Analysis session {SessionId} has excessive job count: {JobCount}", request.SessionId, jobCount);
            return new GetAnalysisSessionDetailsQueryResponse
            {
                Success = false,
                Message = "Session has too many analysis jobs. Please contact administrator.",
                Errors = new List<string> { "Analysis job count exceeds safe display limit" }
            };
        }

        // PERFORMANCE: Use AsNoTracking for read-only queries
        var session = await _context.AnalysisSessions
            .AsNoTracking()
            .Include(s => s.AnalysisJobs.Take(1000)) // PERFORMANCE: Increased limit with better memory management
            .Where(s => s.Id == request.SessionId)
            .Select(s => new AnalysisSessionDto
            {
                Id = s.Id,
                PatientId = s.PatientId,
                CreatedByUserId = s.CreatedByUserId,
                SessionDate = s.SessionDate,
                Status = s.Status,
                // PERFORMANCE: More efficient truncation with null check
                FinalReportData = TruncateString(s.FinalReportData, 50000),
                CreatedAt = s.CreatedAt,
                AnalysisJobs = s.AnalysisJobs.Select(j => new AnalysisJobDto
                {
                    Id = j.Id,
                    SessionId = j.SessionId,
                    PatientId = j.PatientId,
                    CalibrationProfileId = j.CalibrationProfileId,
                    CreatedByUserId = j.CreatedByUserId,
                    // PERFORMANCE: More efficient truncation
                    LocationTag = TruncateString(j.LocationTag, 100),
                    ImageStorageKey = j.ImageStorageKey,
                    AnnotatedImageKey = j.AnnotatedImageKey,
                    // DESIGN IMPROVEMENT: Use enum instead of string
                    Status = j.Status.ToString(),
                    // PERFORMANCE: More efficient truncation
                    AnalysisResult = TruncateString(j.AnalysisResult, 25000),
                    DoctorNotes = TruncateString(j.DoctorNotes, 2000),
                    CreatedAt = j.CreatedAt,
                    StartedAt = j.StartedAt,
                    CompletedAt = j.CompletedAt,
                    ErrorMessage = TruncateString(j.ErrorMessage, 500),
                    ProcessingTimeMs = j.ProcessingTimeMs
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (session == null)
        {
            _logger.LogWarning("Analysis session {SessionId} not found", request.SessionId);
            return new GetAnalysisSessionDetailsQueryResponse
            {
                Success = false,
                Message = "Analysis session not found",
                Errors = new List<string> { "Analysis session not found" }
            };
        }

        _logger.LogInformation("Analysis session {SessionId} details retrieved successfully with {JobCount} jobs", request.SessionId, session.AnalysisJobs?.Count ?? 0);
        return new GetAnalysisSessionDetailsQueryResponse
        {
            Success = true,
            Message = "Analysis session details retrieved successfully",
            Session = session
        };
    }

    // PERFORMANCE: Helper method for efficient string truncation
    private static string? TruncateString(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        if (value.Length <= maxLength)
            return value;

        // PERFORMANCE: Ensure we don't split UTF-8 multi-byte characters
        // For simplicity, we'll just truncate, but in production you might want to handle this more carefully
        return value.Substring(0, maxLength);
    }
}