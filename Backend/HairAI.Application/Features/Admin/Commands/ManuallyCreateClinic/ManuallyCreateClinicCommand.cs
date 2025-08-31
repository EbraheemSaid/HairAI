using MediatR;

namespace HairAI.Application.Features.Admin.Commands.ManuallyCreateClinic;

public class ManuallyCreateClinicCommand : IRequest<ManuallyCreateClinicCommandResponse>
{
    public string Name { get; set; } = string.Empty;
}