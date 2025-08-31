using MediatR;
using HairAI.Application.DTOs;

namespace HairAI.Application.Features.Clinics.Queries.GetClinicById;

public class GetClinicByIdQuery : IRequest<GetClinicByIdQueryResponse>
{
    public Guid ClinicId { get; set; }
}