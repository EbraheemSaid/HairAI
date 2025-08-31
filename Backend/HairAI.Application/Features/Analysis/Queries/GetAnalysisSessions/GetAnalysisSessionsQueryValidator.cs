using FluentValidation;

namespace HairAI.Application.Features.Analysis.Queries.GetAnalysisSessions;

public class GetAnalysisSessionsQueryValidator : AbstractValidator<GetAnalysisSessionsQuery>
{
    public GetAnalysisSessionsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.SortBy)
            .Must(BeValidSortField)
            .WithMessage("Invalid sort field. Valid options: SessionDate, CreatedAt, PatientName, Status");

        RuleFor(x => x.SortDirection)
            .Must(BeValidSortDirection)
            .WithMessage("Invalid sort direction. Valid options: asc, desc");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("From date must be less than or equal to To date");

        RuleFor(x => x.Status)
            .Must(BeValidStatus)
            .When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Invalid status. Valid options: in_progress, completed, cancelled");
    }

    private bool BeValidSortField(string sortBy)
    {
        var validFields = new[] { "SessionDate", "CreatedAt", "PatientName", "Status" };
        return validFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
    }

    private bool BeValidSortDirection(string sortDirection)
    {
        var validDirections = new[] { "asc", "desc" };
        return validDirections.Contains(sortDirection, StringComparer.OrdinalIgnoreCase);
    }

    private bool BeValidStatus(string status)
    {
        var validStatuses = new[] { "in_progress", "completed", "cancelled" };
        return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
} 