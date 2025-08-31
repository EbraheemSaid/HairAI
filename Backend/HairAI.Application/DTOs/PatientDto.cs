namespace HairAI.Application.DTOs;

public class PatientDto
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public string? ClinicPatientId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
}