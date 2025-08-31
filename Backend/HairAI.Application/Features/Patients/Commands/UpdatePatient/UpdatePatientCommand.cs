using MediatR;

namespace HairAI.Application.Features.Patients.Commands.UpdatePatient;

public class UpdatePatientCommand : IRequest<UpdatePatientCommandResponse>
{
    public Guid PatientId { get; set; }
    public string? ClinicPatientId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
}