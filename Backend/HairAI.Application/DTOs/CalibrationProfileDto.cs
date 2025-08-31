namespace HairAI.Application.DTOs;

public class CalibrationProfileDto
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public string ProfileName { get; set; } = string.Empty;
    public string CalibrationData { get; set; } = string.Empty;
    public int Version { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}