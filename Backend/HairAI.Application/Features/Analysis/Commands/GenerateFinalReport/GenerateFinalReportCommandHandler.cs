using MediatR;
using HairAI.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace HairAI.Application.Features.Analysis.Commands.GenerateFinalReport;

public class GenerateFinalReportCommandHandler : IRequestHandler<GenerateFinalReportCommand, GenerateFinalReportCommandResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;
    private readonly ILogger<GenerateFinalReportCommandHandler> _logger;

    public GenerateFinalReportCommandHandler(
        IApplicationDbContext context, 
        IClinicAuthorizationService authorizationService,
        ILogger<GenerateFinalReportCommandHandler> logger)
    {
        _context = context;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<GenerateFinalReportCommandResponse> Handle(GenerateFinalReportCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating final report for session {SessionId}", request.SessionId);
        
        // SECURITY: First get the session to check patient access
        var sessionInfo = await _context.AnalysisSessions
            .Where(s => s.Id == request.SessionId)
            .Select(s => new { s.PatientId })
            .FirstOrDefaultAsync(cancellationToken);

        if (sessionInfo == null)
        {
            _logger.LogWarning("Attempt to generate report for non-existent session {SessionId}", request.SessionId);
            return new GenerateFinalReportCommandResponse
            {
                Success = false,
                Message = "Analysis session not found",
                Errors = new List<string> { "Analysis session not found" }
            };
        }

        // SECURITY: Check if user can access this analysis session's patient
        if (!await _authorizationService.CanAccessPatientAsync(sessionInfo.PatientId))
        {
            _logger.LogWarning("Unauthorized attempt to generate report for session {SessionId} by user without access to patient {PatientId}", 
                request.SessionId, sessionInfo.PatientId);
            return new GenerateFinalReportCommandResponse
            {
                Success = false,
                Message = "Access denied. You cannot generate reports for this session.",
                Errors = new List<string> { "Unauthorized access to analysis session" }
            };
        }

        var session = await _context.AnalysisSessions
            .Include(s => s.AnalysisJobs)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

        if (session == null)
        {
            _logger.LogWarning("Analysis session {SessionId} not found", request.SessionId);
            return new GenerateFinalReportCommandResponse
            {
                Success = false,
                Message = "Analysis session not found",
                Errors = new List<string> { "Analysis session not found" }
            };
        }

        // Aggregate data from all completed jobs
        var completedJobs = session.AnalysisJobs.Where(j => j.Status == HairAI.Domain.Enums.JobStatus.Completed).ToList();

        if (!completedJobs.Any())
        {
            _logger.LogWarning("No completed analysis jobs found for session {SessionId}", request.SessionId);
            return new GenerateFinalReportCommandResponse
            {
                Success = false,
                Message = "No completed analysis jobs found for this session",
                Errors = new List<string> { "No completed analysis jobs found for this session" }
            };
        }

        // CRASH PREVENTION: Limit number of jobs to prevent memory issues
        if (completedJobs.Count > 1000)
        {
            _logger.LogWarning("Too many analysis jobs ({JobCount}) for session {SessionId}", completedJobs.Count, request.SessionId);
            return new GenerateFinalReportCommandResponse
            {
                Success = false,
                Message = "Too many analysis jobs. Please contact administrator.",
                Errors = new List<string> { "Analysis job count exceeds processing limit" }
            };
        }

        try
        {
            _logger.LogInformation("Processing {JobCount} completed jobs for report generation", completedJobs.Count);
            
            // MEMORY OPTIMIZATION: Process jobs safely with error handling
            var totalHairCount = 0;
            var densitySum = 0.0;
            var validDensityCount = 0;
            var analyzedLocations = new List<object>();

            foreach (var job in completedJobs)
            {
                try
                {
                    // CRASH PREVENTION: Validate JSON before deserialization
                    if (string.IsNullOrEmpty(job.AnalysisResult))
                        continue;

                    // MEMORY LEAK FIX: Limit JSON size to prevent OutOfMemoryException
                    if (job.AnalysisResult.Length > 100000) // 100KB limit
                    {
                        _logger.LogWarning("Skipping oversized analysis result for job {JobId} ({Size} bytes)", job.Id, job.AnalysisResult.Length);
                        continue; // Skip oversized results
                    }

                    // CRASH PREVENTION: Safe JSON deserialization with error handling
                    using var document = JsonDocument.Parse(job.AnalysisResult);
                    var root = document.RootElement;

                    // Safe extraction with default values
                    var hairCount = 0;
                    var density = 0.0;

                    if (root.TryGetProperty("hair_count", out var hairCountElement))
                    {
                        if (hairCountElement.ValueKind == JsonValueKind.Number)
                        {
                            hairCount = hairCountElement.GetInt32();
                            totalHairCount += Math.Max(0, Math.Min(hairCount, 10000)); // Sanity check
                        }
                    }

                    if (root.TryGetProperty("density", out var densityElement))
                    {
                        if (densityElement.ValueKind == JsonValueKind.Number)
                        {
                            density = densityElement.GetDouble();
                            if (density >= 0 && density <= 1000) // Sanity check for density
                            {
                                densitySum += density;
                                validDensityCount++;
                            }
                        }
                    }

                    // MEMORY OPTIMIZATION: Limit location data
                    if (analyzedLocations.Count < 500) // Limit to prevent memory issues
                    {
                        analyzedLocations.Add(new
                        {
                            Location = job.LocationTag?.Length > 100 ? job.LocationTag.Substring(0, 100) : job.LocationTag, // Truncate long location names
                            HairCount = hairCount,
                            Density = Math.Round(density, 2),
                            DoctorNotes = job.DoctorNotes?.Length > 1000 ? job.DoctorNotes.Substring(0, 1000) : job.DoctorNotes // Truncate long notes
                        });
                    }
                }
                catch (JsonException)
                {
                    // CRASH PREVENTION: Skip invalid JSON without crashing
                    _logger.LogWarning("Invalid JSON in analysis result for job {JobId}", job.Id);
                    continue;
                }
                catch (Exception ex)
                {
                    // CRASH PREVENTION: Skip any problematic jobs
                    _logger.LogWarning(ex, "Error processing analysis job {JobId}", job.Id);
                    continue;
                }
            }

            var averageDensity = validDensityCount > 0 ? Math.Round(densitySum / validDensityCount, 2) : 0.0;

            var reportData = new
            {
                SessionId = session.Id,
                PatientId = session.PatientId,
                SessionDate = session.SessionDate,
                TotalAnalyzedAreas = completedJobs.Count,
                TotalHairCount = totalHairCount,
                AverageDensity = averageDensity,
                AnalyzedLocations = analyzedLocations,
                GeneratedAt = DateTime.UtcNow
            };

            // MEMORY OPTIMIZATION: Use JsonSerializerOptions to control serialization
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = false, // Compact JSON to save memory
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            // Update the session with the final report
            session.FinalReportData = JsonSerializer.Serialize(reportData, jsonOptions);
            session.Status = "completed";

            await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Final report generated successfully for session {SessionId} with {JobCount} jobs", 
                request.SessionId, completedJobs.Count);

            return new GenerateFinalReportCommandResponse
            {
                Success = true,
                Message = "Final report generated successfully",
                ReportData = session.FinalReportData
            };
        }
        catch (OutOfMemoryException ex)
        {
            _logger.LogError(ex, "OutOfMemoryException during report generation for session {SessionId}", request.SessionId);
            return new GenerateFinalReportCommandResponse
            {
                Success = false,
                Message = "Report generation failed due to memory constraints. Please contact administrator.",
                Errors = new List<string> { "Memory limit exceeded during report generation" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during report generation for session {SessionId}", request.SessionId);
            return new GenerateFinalReportCommandResponse
            {
                Success = false,
                Message = "Failed to generate final report",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}