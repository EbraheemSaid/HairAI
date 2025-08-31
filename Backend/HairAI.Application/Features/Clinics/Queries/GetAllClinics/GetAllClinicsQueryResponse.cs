using MediatR;
using HairAI.Application.DTOs;

namespace HairAI.Application.Features.Clinics.Queries.GetAllClinics;

public class GetAllClinicsQueryResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public List<ClinicDto> Clinics { get; set; } = new();
}