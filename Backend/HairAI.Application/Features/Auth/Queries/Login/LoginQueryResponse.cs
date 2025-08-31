using MediatR;

namespace HairAI.Application.Features.Auth.Queries.Login;

public class LoginQueryResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public string? Token { get; set; }
    public string? UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Guid? ClinicId { get; set; }
}