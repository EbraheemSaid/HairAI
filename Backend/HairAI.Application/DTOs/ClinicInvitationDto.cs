namespace HairAI.Application.DTOs;

public class ClinicInvitationDto
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public string InvitedByUserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}