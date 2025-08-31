using MediatR;

namespace HairAI.Application.Features.Clinics.Commands.UpdateClinic;

public class UpdateClinicCommand : IRequest<UpdateClinicCommandResponse>
{
    public Guid ClinicId { get; set; }
    public string Name { get; set; } = string.Empty;
}