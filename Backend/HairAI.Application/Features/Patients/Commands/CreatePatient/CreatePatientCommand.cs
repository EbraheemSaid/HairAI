using MediatR;

namespace HairAI.Application.Features.Patients.Commands.CreatePatient;

public class CreatePatientCommand : IRequest<CreatePatientCommandResponse>
{
    public Guid ClinicId { get; set; }
    public string? ClinicPatientId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
}