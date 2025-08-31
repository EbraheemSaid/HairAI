using MediatR;
using HairAI.Application.DTOs;

namespace HairAI.Application.Features.Patients.Queries.GetPatientsForClinic;

public class GetPatientsForClinicQueryResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public List<PatientDto> Patients { get; set; } = new();
}