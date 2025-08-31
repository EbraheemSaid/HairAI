using MediatR;
using HairAI.Application.DTOs;

namespace HairAI.Application.Features.Invitations.Queries.GetInvitationByToken;

public class GetInvitationByTokenQueryResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public ClinicInvitationDto? Invitation { get; set; }
}