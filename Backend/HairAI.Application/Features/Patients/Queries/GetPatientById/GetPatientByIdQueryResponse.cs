using MediatR;
using HairAI.Application.DTOs;

namespace HairAI.Application.Features.Patients.Queries.GetPatientById;

public class GetPatientByIdQueryResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public PatientDto? Patient { get; set; }
}