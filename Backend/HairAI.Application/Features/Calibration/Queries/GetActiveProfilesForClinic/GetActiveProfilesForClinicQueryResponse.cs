using MediatR;
using HairAI.Application.DTOs;

namespace HairAI.Application.Features.Calibration.Queries.GetActiveProfilesForClinic;

public class GetActiveProfilesForClinicQueryResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public List<CalibrationProfileDto> Profiles { get; set; } = new();
}