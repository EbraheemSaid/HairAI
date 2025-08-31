using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HairAI.Application.Features.Invitations.Commands.AcceptInvitation;

public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand, AcceptInvitationCommandResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AcceptInvitationCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<AcceptInvitationCommandResponse> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await _context.ClinicInvitations
            .FirstOrDefaultAsync(ci => ci.Token == request.Token, cancellationToken);

        if (invitation == null)
        {
            return new AcceptInvitationCommandResponse
            {
                Success = false,
                Message = "Invalid invitation token",
                Errors = new List<string> { "Invalid invitation token" }
            };
        }

        if (invitation.Status != "pending")
        {
            return new AcceptInvitationCommandResponse
            {
                Success = false,
                Message = "Invitation is no longer valid",
                Errors = new List<string> { "Invitation is no longer valid" }
            };
        }

        if (invitation.ExpiresAt < DateTime.UtcNow)
        {
            invitation.Status = "expired";
            await _context.SaveChangesAsync(cancellationToken);

            return new AcceptInvitationCommandResponse
            {
                Success = false,
                Message = "Invitation has expired",
                Errors = new List<string> { "Invitation has expired" }
            };
        }

        // Get the user
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
        {
            return new AcceptInvitationCommandResponse
            {
                Success = false,
                Message = "User not found",
                Errors = new List<string> { "User not found" }
            };
        }

        // Assign the role to the user
        await _userManager.AddToRoleAsync(user, invitation.Role);

        // Update invitation status
        invitation.Status = "accepted";
        invitation.AcceptedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new AcceptInvitationCommandResponse
        {
            Success = true,
            Message = "Invitation accepted successfully"
        };
    }
}