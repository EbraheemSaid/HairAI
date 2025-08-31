using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Domain.Entities;

namespace HairAI.Application.Features.Invitations.Commands.CreateInvitation;

public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, CreateInvitationCommandResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IClinicAuthorizationService _authorizationService;

    public CreateInvitationCommandHandler(IApplicationDbContext context, IEmailService emailService, IClinicAuthorizationService authorizationService)
    {
        _context = context;
        _emailService = emailService;
        _authorizationService = authorizationService;
    }

    public async Task<CreateInvitationCommandResponse> Handle(CreateInvitationCommand request, CancellationToken cancellationToken)
    {
        // SECURITY: Check if user can create invitations for this clinic
        if (!await _authorizationService.CanAccessClinicAsync(request.ClinicId))
        {
            return new CreateInvitationCommandResponse
            {
                Success = false,
                Message = "Access denied. You cannot create invitations for this clinic.",
                Errors = new List<string> { "Unauthorized clinic access" }
            };
        }

        // SECURITY: Only ClinicAdmin or SuperAdmin can create invitations
        if (!await _authorizationService.IsClinicAdminAsync() && !await _authorizationService.IsSuperAdminAsync())
        {
            return new CreateInvitationCommandResponse
            {
                Success = false,
                Message = "Access denied. Only clinic administrators can create invitations.",
                Errors = new List<string> { "Insufficient permissions for invitation creation" }
            };
        }

        // Generate a unique token for the invitation
        var token = Guid.NewGuid().ToString();

        var invitation = new ClinicInvitation
        {
            ClinicId = request.ClinicId,
            InvitedByUserId = request.InvitedByUserId,
            Email = request.Email,
            Role = request.Role,
            Token = token,
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddDays(7) // Token expires in 7 days
        };

        _context.ClinicInvitations.Add(invitation);
        await _context.SaveChangesAsync(cancellationToken);

        // Send invitation email
        var invitationLink = $"http://localhost:5173/invitations/accept?token={token}";
        await _emailService.SendInvitationEmailAsync(request.Email, invitationLink);

        return new CreateInvitationCommandResponse
        {
            Success = true,
            Message = "Invitation created and sent successfully",
            InvitationId = invitation.Id,
            InvitationToken = token
        };
    }
}