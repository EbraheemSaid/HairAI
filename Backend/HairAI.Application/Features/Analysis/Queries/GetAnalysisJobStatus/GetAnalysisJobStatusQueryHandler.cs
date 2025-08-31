using MediatR;
using HairAI.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HairAI.Application.Features.Analysis.Queries.GetAnalysisJobStatus;

public class GetAnalysisJobStatusQueryHandler : IRequestHandler<GetAnalysisJobStatusQuery, GetAnalysisJobStatusQueryResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;

    public GetAnalysisJobStatusQueryHandler(IApplicationDbContext context, IClinicAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<GetAnalysisJobStatusQueryResponse> Handle(GetAnalysisJobStatusQuery request, CancellationToken cancellationToken)
    {
        // SECURITY: First get the job to check patient access
        var jobPatient = await _context.AnalysisJobs
            .Where(j => j.Id == request.JobId)
            .Select(j => new { j.PatientId })
            .FirstOrDefaultAsync(cancellationToken);

        if (jobPatient == null)
        {
            return new GetAnalysisJobStatusQueryResponse
            {
                Success = false,
                Message = "Analysis job not found",
                Errors = new List<string> { "Analysis job not found" }
            };
        }

        // SECURITY: Check if user can access this analysis job's patient
        if (!await _authorizationService.CanAccessPatientAsync(jobPatient.PatientId))
        {
            return new GetAnalysisJobStatusQueryResponse
            {
                Success = false,
                Message = "Access denied. You cannot access this analysis job status.",
                Errors = new List<string> { "Unauthorized access to analysis job" }
            };
        }

        var job = await _context.AnalysisJobs
            .Where(j => j.Id == request.JobId)
            .Select(j => new
            {
                j.Status,
                j.StartedAt,
                j.CompletedAt,
                j.ErrorMessage,
                j.ProcessingTimeMs
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (job == null)
        {
            return new GetAnalysisJobStatusQueryResponse
            {
                Success = false,
                Message = "Analysis job not found",
                Errors = new List<string> { "Analysis job not found" }
            };
        }

        return new GetAnalysisJobStatusQueryResponse
        {
            Success = true,
            Message = "Job status retrieved successfully",
            Status = job.Status.ToString(),
            StartedAt = job.StartedAt,
            CompletedAt = job.CompletedAt,
            ErrorMessage = job.ErrorMessage,
            ProcessingTimeMs = job.ProcessingTimeMs
        };
    }
}