using MediatR;
using HairAI.Application.DTOs;

namespace HairAI.Application.Features.Patients.Queries.GetPatientsForClinic;

public class GetPatientsForClinicQuery : IRequest<GetPatientsForClinicQueryResponse>
{
    public Guid ClinicId { get; set; }
}