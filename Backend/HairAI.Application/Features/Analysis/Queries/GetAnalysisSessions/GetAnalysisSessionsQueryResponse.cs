using HairAI.Application.DTOs;

namespace HairAI.Application.Features.Analysis.Queries.GetAnalysisSessions;

public class GetAnalysisSessionsQueryResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public List<AnalysisSessionSummaryDto> Sessions { get; set; } = new();
    public PaginationMetadata Pagination { get; set; } = new();
}

public class AnalysisSessionSummaryDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string CreatedByUserId { get; set; } = string.Empty;
    public string CreatedByUserName { get; set; } = string.Empty;
    public DateTime SessionDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int TotalJobs { get; set; }
    public int CompletedJobs { get; set; }
    public int PendingJobs { get; set; }
    public List<string> LocationTags { get; set; } = new();
}

public class PaginationMetadata
{
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
} 