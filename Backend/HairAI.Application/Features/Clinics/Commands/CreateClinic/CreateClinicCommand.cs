using MediatR;

namespace HairAI.Application.Features.Clinics.Commands.CreateClinic;

public class CreateClinicCommand : IRequest<CreateClinicCommandResponse>
{
    public string Name { get; set; } = string.Empty;
}