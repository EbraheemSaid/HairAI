using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HairAI.Application.Features.Analysis.Commands.CreateAnalysisSession;

public class CreateAnalysisSessionCommandHandler : IRequestHandler<CreateAnalysisSessionCommand, CreateAnalysisSessionCommandResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;
    private readonly ILogger<CreateAnalysisSessionCommandHandler> _logger;

    public CreateAnalysisSessionCommandHandler(
        IApplicationDbContext context, 
        IClinicAuthorizationService authorizationService,
        ILogger<CreateAnalysisSessionCommandHandler> logger)
    {
        _context = context;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<CreateAnalysisSessionCommandResponse> Handle(CreateAnalysisSessionCommand request, CancellationToken cancellationToken)
    {
        // SECURITY: Check if user can create analysis sessions for this patient
        if (!await _authorizationService.CanAccessPatientAsync(request.PatientId))
        {
            _logger.LogWarning("Unauthorized attempt to create analysis session for patient {PatientId} by user {UserId}", 
                request.PatientId, request.CreatedByUserId);
            return new CreateAnalysisSessionCommandResponse
            {
                Success = false,
                Message = "Access denied. You cannot create analysis sessions for this patient.",
                Errors = new List<string> { "Unauthorized patient access" }
            };
        }

        // SECURITY: Validate session date is reasonable
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (request.SessionDate > today.AddDays(30)) // No future sessions more than 30 days
        {
            _logger.LogWarning("Attempt to create analysis session with future date {SessionDate} by user {UserId}", 
                request.SessionDate, request.CreatedByUserId);
            return new CreateAnalysisSessionCommandResponse
            {
                Success = false,
                Message = "Session date cannot be more than 30 days in the future.",
                Errors = new List<string> { "Invalid session date" }
            };
        }

        if (request.SessionDate < today.AddYears(-1)) // No past sessions more than 1 year
        {
            _logger.LogWarning("Attempt to create analysis session with past date {SessionDate} by user {UserId}", 
                request.SessionDate, request.CreatedByUserId);
            return new CreateAnalysisSessionCommandResponse
            {
                Success = false,
                Message = "Session date cannot be more than 1 year in the past.",
                Errors = new List<string> { "Invalid session date" }
            };
        }

        var session = new AnalysisSession
        {
            PatientId = request.PatientId,
            CreatedByUserId = request.CreatedByUserId,
            SessionDate = request.SessionDate,
            Status = "in_progress",
            CreatedAt = DateTime.UtcNow
        };

        _context.AnalysisSessions.Add(session);
        
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Analysis session {SessionId} created successfully for patient {PatientId} by user {UserId}", 
                session.Id, request.PatientId, request.CreatedByUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create analysis session for patient {PatientId} by user {UserId}", 
                request.PatientId, request.CreatedByUserId);
            return new CreateAnalysisSessionCommandResponse
            {
                Success = false,
                Message = "Failed to create analysis session. Please try again.",
                Errors = new List<string> { "Database error" }
            };
        }

        return new CreateAnalysisSessionCommandResponse
        {
            Success = true,
            Message = "Analysis session created successfully",
            SessionId = session.Id
        };
    }
}