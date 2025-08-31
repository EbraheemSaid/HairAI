using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HairAI.Application.Features.Invitations.Queries.GetInvitationByToken;

public class GetInvitationByTokenQueryHandler : IRequestHandler<GetInvitationByTokenQuery, GetInvitationByTokenQueryResponse>
{
    private readonly IApplicationDbContext _context;

    public GetInvitationByTokenQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetInvitationByTokenQueryResponse> Handle(GetInvitationByTokenQuery request, CancellationToken cancellationToken)
    {
        // SECURITY: Validate token format to prevent enumeration attacks
        if (string.IsNullOrEmpty(request.Token) || request.Token.Length != 36)
        {
            return new GetInvitationByTokenQueryResponse
            {
                Success = false,
                Message = "Invalid invitation token format",
                Errors = new List<string> { "Invalid invitation token format" }
            };
        }

        // SECURITY: Only return valid, pending, non-expired invitations
        var invitation = await _context.ClinicInvitations
            .Where(ci => ci.Token == request.Token && 
                         ci.Status == "pending" && 
                         ci.ExpiresAt > DateTime.UtcNow)
            .Select(ci => new ClinicInvitationDto
            {
                Id = ci.Id,
                ClinicId = ci.ClinicId,
                Email = ci.Email,
                Role = ci.Role,
                Status = ci.Status,
                ExpiresAt = ci.ExpiresAt
                // SECURITY: Don't expose InvitedByUserId or sensitive data
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (invitation == null)
        {
            // SECURITY: Return same error for non-existent, expired, or used tokens
            // to prevent token enumeration attacks
            return new GetInvitationByTokenQueryResponse
            {
                Success = false,
                Message = "Invitation not found or no longer valid",
                Errors = new List<string> { "Invitation not found or no longer valid" }
            };
        }

        return new GetInvitationByTokenQueryResponse
        {
            Success = true,
            Message = "Invitation retrieved successfully",
            Invitation = invitation
        };
    }
}