using MediatR;
using HairAI.Application.DTOs;

namespace HairAI.Application.Features.Clinics.Queries.GetClinicById;

public class GetClinicByIdQueryResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public ClinicDto? Clinic { get; set; }
}