using MediatR;
using HairAI.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using HairAI.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HairAI.Application.Features.Analysis.Queries.GetAnalysisSessions;

public class GetAnalysisSessionsQueryHandler : IRequestHandler<GetAnalysisSessionsQuery, GetAnalysisSessionsQueryResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetAnalysisSessionsQueryHandler> _logger;

    public GetAnalysisSessionsQueryHandler(
        IApplicationDbContext context, 
        ICurrentUserService currentUserService,
        ILogger<GetAnalysisSessionsQueryHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<GetAnalysisSessionsQueryResponse> Handle(GetAnalysisSessionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting analysis sessions for user {UserId} with filters", _currentUserService.UserId);
            
            // Build the query with filters
            var query = _context.AnalysisSessions
                .Include(s => s.Patient)
                .AsNoTracking() // PERFORMANCE: Use AsNoTracking for read-only queries
                .AsQueryable();

            // Apply filters based on user role and clinic access
            var currentUserId = _currentUserService.UserId;
            if (!string.IsNullOrEmpty(currentUserId))
            {
                // Get user's clinic to filter sessions (doctors should only see their clinic's sessions)
                var userClinic = await _context.ApplicationUsers
                    .Where(u => u.Id == currentUserId)
                    .Select(u => u.ClinicId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (userClinic.HasValue)
                {
                    query = query.Where(s => s.Patient.ClinicId == userClinic.Value);
                }
            }

            // Apply additional filters
            if (request.PatientId.HasValue)
            {
                query = query.Where(s => s.PatientId == request.PatientId.Value);
            }

            if (request.ClinicId.HasValue)
            {
                query = query.Where(s => s.Patient.ClinicId == request.ClinicId.Value);
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(s => s.Status == request.Status);
            }

            if (request.FromDate.HasValue)
            {
                var fromDateOnly = DateOnly.FromDateTime(request.FromDate.Value);
                query = query.Where(s => s.SessionDate >= fromDateOnly);
            }

            if (request.ToDate.HasValue)
            {
                var toDateOnly = DateOnly.FromDateTime(request.ToDate.Value);
                query = query.Where(s => s.SessionDate <= toDateOnly);
            }

            // Apply sorting
            query = ApplySorting(query, request.SortBy, request.SortDirection);

            // Get total count for pagination
            var totalCount = await query.CountAsync(cancellationToken);

            _logger.LogInformation("Found {TotalCount} total analysis sessions matching filters", totalCount);

            // Validate pagination parameters
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;
            if (request.PageSize > 100) request.PageSize = 100; // SECURITY: Limit page size

            // Apply pagination with proper joins to prevent N+1 queries
            var sessions = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new AnalysisSessionSummaryDto
                {
                    Id = s.Id,
                    PatientId = s.PatientId,
                    PatientName = $"{s.Patient.FirstName} {s.Patient.LastName}",
                    CreatedByUserId = s.CreatedByUserId,
                    CreatedByUserName = "", // Will be populated with optimized join below
                    SessionDate = s.SessionDate.ToDateTime(TimeOnly.MinValue),
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    // PERFORMANCE: Use scalar subqueries instead of collection counts to avoid N+1
                    TotalJobs = _context.AnalysisJobs.Count(j => j.SessionId == s.Id),
                    CompletedJobs = _context.AnalysisJobs.Count(j => j.SessionId == s.Id && j.Status == HairAI.Domain.Enums.JobStatus.Completed),
                    PendingJobs = _context.AnalysisJobs.Count(j => j.SessionId == s.Id && 
                        (j.Status == HairAI.Domain.Enums.JobStatus.Pending || j.Status == HairAI.Domain.Enums.JobStatus.Processing)),
                    // PERFORMANCE: Load limited location tags to prevent memory issues
                    LocationTags = _context.AnalysisJobs
                        .Where(j => j.SessionId == s.Id && j.LocationTag != null)
                        .Select(j => j.LocationTag)
                        .Distinct()
                        .Take(50) // MEMORY SAFETY: Limit location tags
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {SessionCount} analysis sessions for page {PageNumber}", sessions.Count, request.PageNumber);

            // PERFORMANCE: Optimized single join to get user names
            var sessionUserData = await (from session in _context.AnalysisSessions
                                        where sessions.Select(s => s.Id).Contains(session.Id)
                                        join user in _context.ApplicationUsers on session.CreatedByUserId equals user.Id into userJoin
                                        from user in userJoin.DefaultIfEmpty()
                                        select new 
                                        { 
                                            SessionId = session.Id, 
                                            UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown User"
                                        }).ToListAsync(cancellationToken);

            var userNameDict = sessionUserData.ToDictionary(x => x.SessionId, x => x.UserName);
            foreach (var session in sessions)
            {
                session.CreatedByUserName = userNameDict.GetValueOrDefault(session.Id, "Unknown");
            }

            // Calculate pagination metadata
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var pagination = new PaginationMetadata
            {
                TotalCount = totalCount,
                PageSize = request.PageSize,
                CurrentPage = request.PageNumber,
                TotalPages = totalPages,
                HasNext = request.PageNumber < totalPages,
                HasPrevious = request.PageNumber > 1
            };

            return new GetAnalysisSessionsQueryResponse
            {
                Success = true,
                Message = $"Retrieved {sessions.Count} analysis sessions (page {request.PageNumber} of {totalPages})",
                Sessions = sessions,
                Pagination = pagination
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve analysis sessions for user {UserId}", _currentUserService.UserId);
            return new GetAnalysisSessionsQueryResponse
            {
                Success = false,
                Message = "Failed to retrieve analysis sessions",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    private IQueryable<AnalysisSession> ApplySorting(IQueryable<AnalysisSession> query, string sortBy, string sortDirection)
    {
        var isDescending = sortDirection.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "sessiondate" => isDescending ? query.OrderByDescending(s => s.SessionDate) : query.OrderBy(s => s.SessionDate),
            "createdat" => isDescending ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt),
            "patientname" => isDescending ? 
                query.OrderByDescending(s => s.Patient.LastName).ThenByDescending(s => s.Patient.FirstName) : 
                query.OrderBy(s => s.Patient.LastName).ThenBy(s => s.Patient.FirstName),
            "status" => isDescending ? query.OrderByDescending(s => s.Status) : query.OrderBy(s => s.Status),
            _ => query.OrderByDescending(s => s.SessionDate) // Default sort
        };
    }
} 