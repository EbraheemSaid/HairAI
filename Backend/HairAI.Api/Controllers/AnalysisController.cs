using HairAI.Application.Features.Analysis.Commands.AddDoctorNotes;
using HairAI.Application.Features.Analysis.Commands.CreateAnalysisSession;
using HairAI.Application.Features.Analysis.Commands.GenerateFinalReport;
using HairAI.Application.Features.Analysis.Commands.UploadAnalysisImage;
using HairAI.Application.Features.Analysis.Queries.GetAnalysisJobResult;
using HairAI.Application.Features.Analysis.Queries.GetAnalysisJobStatus;
using HairAI.Application.Features.Analysis.Queries.GetAnalysisSessionDetails;
using HairAI.Application.Features.Analysis.Queries.GetAnalysisSessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace HairAI.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AnalysisController : BaseController
{
    /// <summary>
    /// Get paginated list of analysis sessions with filtering and sorting options
    /// </summary>
    /// <param name="patientId">Filter by specific patient (optional)</param>
    /// <param name="clinicId">Filter by specific clinic (optional, SuperAdmin only)</param>
    /// <param name="status">Filter by session status (optional)</param>
    /// <param name="fromDate">Filter sessions from this date (optional)</param>
    /// <param name="toDate">Filter sessions to this date (optional)</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="sortBy">Sort field: SessionDate, CreatedAt, PatientName, Status (default: SessionDate)</param>
    /// <param name="sortDirection">Sort direction: asc, desc (default: desc)</param>
    /// <returns>Paginated list of analysis sessions with metadata</returns>
    [HttpGet("sessions")]
    [EnableRateLimiting("AuthPolicy")] // Add rate limiting for security
    public async Task<ActionResult<GetAnalysisSessionsQueryResponse>> GetSessions(
        [FromQuery] Guid? patientId = null,
        [FromQuery] Guid? clinicId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "SessionDate",
        [FromQuery] string sortDirection = "desc")
    {
        // SECURITY: Validate page size
        if (pageSize > 100)
            pageSize = 100;
            
        if (pageNumber < 1)
            pageNumber = 1;

        var query = new GetAnalysisSessionsQuery
        {
            PatientId = patientId,
            ClinicId = clinicId,
            Status = status,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDirection = sortDirection
        };

        var response = await Mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Get analysis session details including all analysis jobs
    /// </summary>
    /// <param name="id">Session ID</param>
    /// <returns>Session details with analysis jobs</returns>
    [HttpGet("session/{id:guid}")]
    [EnableRateLimiting("AuthPolicy")] // Add rate limiting for security
    public async Task<ActionResult<GetAnalysisSessionDetailsQueryResponse>> GetSessionDetails(Guid id)
    {
        var query = new GetAnalysisSessionDetailsQuery { SessionId = id };
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Get the status of a specific analysis job
    /// </summary>
    /// <param name="id">Job ID</param>
    /// <returns>Job status and processing information</returns>
    [HttpGet("job/{id:guid}/status")]
    [EnableRateLimiting("AuthPolicy")] // Add rate limiting for security
    public async Task<ActionResult<GetAnalysisJobStatusQueryResponse>> GetJobStatus(Guid id)
    {
        var query = new GetAnalysisJobStatusQuery { JobId = id };
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Get the result of a completed analysis job
    /// </summary>
    /// <param name="id">Job ID</param>
    /// <returns>Analysis results and metrics</returns>
    [HttpGet("job/{id:guid}/result")]
    [EnableRateLimiting("AuthPolicy")] // Add rate limiting for security
    public async Task<ActionResult<GetAnalysisJobResultQueryResponse>> GetJobResult(Guid id)
    {
        var query = new GetAnalysisJobResultQuery { JobId = id };
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Create a new analysis session for a patient visit
    /// </summary>
    /// <param name="command">Session creation details</param>
    /// <returns>Created session information</returns>
    [HttpPost("session")]
    [EnableRateLimiting("AuthPolicy")] // Add rate limiting for security
    public async Task<ActionResult<CreateAnalysisSessionCommandResponse>> CreateSession(CreateAnalysisSessionCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Upload an image for analysis (rate limited for security)
    /// </summary>
    /// <param name="file">The image file to upload</param>
    /// <param name="sessionId">The analysis session ID</param>
    /// <param name="patientId">The patient ID</param>
    /// <param name="calibrationProfileId">The calibration profile ID</param>
    /// <param name="locationTag">The location tag for this image</param>
    /// <returns>Analysis job information</returns>
    [HttpPost("job")]
    [EnableRateLimiting("UploadPolicy")] // Enable rate limiting for uploads
    public async Task<ActionResult<UploadAnalysisImageCommandResponse>> UploadJob(
        IFormFile file,
        [FromForm] Guid sessionId,
        [FromForm] Guid patientId,
        [FromForm] Guid calibrationProfileId,
        [FromForm] string locationTag)
    {
        // Validate the file
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { success = false, message = "No file uploaded" });
        }

        // Validate file type (only allow image files)
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
        {
            return BadRequest(new { success = false, message = "Invalid file type. Only JPG and PNG files are allowed." });
        }

        // Validate file size (limit to 10MB)
        if (file.Length > 10 * 1024 * 1024)
        {
            return BadRequest(new { success = false, message = "File size exceeds 10MB limit." });
        }

        // Generate a unique filename
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var uploadPath = Path.Combine("uploads", "analysis_images");
        var fullPath = Path.Combine(uploadPath, fileName);

        // Ensure the upload directory exists
        Directory.CreateDirectory(uploadPath);

        // Save the file
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Create the command with the file path as ImageStorageKey
        var command = new UploadAnalysisImageCommand
        {
            SessionId = sessionId,
            PatientId = patientId,
            CalibrationProfileId = calibrationProfileId,
            CreatedByUserId = GetCurrentUserId(),
            LocationTag = locationTag,
            ImageStorageKey = fullPath
        };

        var response = await Mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Add doctor notes to an analysis job
    /// </summary>
    /// <param name="id">Job ID</param>
    /// <param name="notes">Doctor's notes</param>
    /// <returns>Success confirmation</returns>
    [HttpPost("job/{id:guid}/notes")]
    [EnableRateLimiting("AuthPolicy")] // Add rate limiting for security
    public async Task<ActionResult<AddDoctorNotesCommandResponse>> AddNotes(Guid id, [FromBody] string notes)
    {
        // SECURITY: Validate notes length
        if (notes?.Length > 5000)
        {
            return BadRequest(new { 
                success = false, 
                message = "Notes exceed maximum length of 5000 characters" 
            });
        }

        var command = new AddDoctorNotesCommand { JobId = id, DoctorNotes = notes };
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Generate a final report for an analysis session
    /// </summary>
    /// <param name="id">Session ID</param>
    /// <returns>Generated report data</returns>
    [HttpPost("session/{id:guid}/report")]
    [EnableRateLimiting("AuthPolicy")] // Add rate limiting for security
    public async Task<ActionResult<GenerateFinalReportCommandResponse>> GenerateReport(Guid id)
    {
        var command = new GenerateFinalReportCommand { SessionId = id };
        var response = await Mediator.Send(command);
        return Ok(response);
    }
}