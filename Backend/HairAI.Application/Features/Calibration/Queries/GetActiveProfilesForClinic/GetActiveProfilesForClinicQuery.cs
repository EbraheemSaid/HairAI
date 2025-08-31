using MediatR;
using HairAI.Application.DTOs;

namespace HairAI.Application.Features.Calibration.Queries.GetActiveProfilesForClinic;

public class GetActiveProfilesForClinicQuery : IRequest<GetActiveProfilesForClinicQueryResponse>
{
    public Guid ClinicId { get; set; }
}