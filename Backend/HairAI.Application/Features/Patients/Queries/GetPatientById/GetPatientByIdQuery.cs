using MediatR;
using HairAI.Application.DTOs;

namespace HairAI.Application.Features.Patients.Queries.GetPatientById;

public class GetPatientByIdQuery : IRequest<GetPatientByIdQueryResponse>
{
    public Guid PatientId { get; set; }
}