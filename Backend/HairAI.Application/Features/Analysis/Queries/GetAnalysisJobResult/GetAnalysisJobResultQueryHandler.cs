using MediatR;
using HairAI.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using HairAI.Domain.Enums;

namespace HairAI.Application.Features.Analysis.Queries.GetAnalysisJobResult;

public class GetAnalysisJobResultQueryHandler : IRequestHandler<GetAnalysisJobResultQuery, GetAnalysisJobResultQueryResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;

    public GetAnalysisJobResultQueryHandler(IApplicationDbContext context, IClinicAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<GetAnalysisJobResultQueryResponse> Handle(GetAnalysisJobResultQuery request, CancellationToken cancellationToken)
    {
        // SECURITY: First get the job to check patient access
        var jobPatient = await _context.AnalysisJobs
            .Where(j => j.Id == request.JobId)
            .Select(j => new { j.PatientId })
            .FirstOrDefaultAsync(cancellationToken);

        if (jobPatient == null)
        {
            return new GetAnalysisJobResultQueryResponse
            {
                Success = false,
                Message = "Analysis job not found",
                Errors = new List<string> { "Analysis job not found" }
            };
        }

        // SECURITY: Check if user can access this analysis job's patient
        if (!await _authorizationService.CanAccessPatientAsync(jobPatient.PatientId))
        {
            return new GetAnalysisJobResultQueryResponse
            {
                Success = false,
                Message = "Access denied. You cannot access this analysis job result.",
                Errors = new List<string> { "Unauthorized access to analysis job" }
            };
        }

        var job = await _context.AnalysisJobs
            .Where(j => j.Id == request.JobId)
            .Select(j => new
            {
                j.AnalysisResult,
                j.AnnotatedImageKey,
                j.DoctorNotes,
                j.Status
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (job == null)
        {
            return new GetAnalysisJobResultQueryResponse
            {
                Success = false,
                Message = "Analysis job not found",
                Errors = new List<string> { "Analysis job not found" }
            };
        }

        if (job.Status != JobStatus.Completed)
        {
            return new GetAnalysisJobResultQueryResponse
            {
                Success = false,
                Message = "Analysis job is not completed yet",
                Errors = new List<string> { "Analysis job is not completed yet" }
            };
        }

        return new GetAnalysisJobResultQueryResponse
        {
            Success = true,
            Message = "Analysis result retrieved successfully",
            AnalysisResult = job.AnalysisResult,
            AnnotatedImageKey = job.AnnotatedImageKey,
            DoctorNotes = job.DoctorNotes
        };
    }
}