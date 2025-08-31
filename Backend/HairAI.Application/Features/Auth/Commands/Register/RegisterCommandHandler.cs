using MediatR;
using HairAI.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HairAI.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterCommandResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;

    public RegisterCommandHandler(IIdentityService identityService, IApplicationDbContext context)
    {
        _identityService = identityService;
        _context = context;
    }

    public async Task<RegisterCommandResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // SECURITY: Validate invitation before allowing registration
        var invitation = await _context.ClinicInvitations
            .FirstOrDefaultAsync(i => i.Email.ToLower() == request.Email.ToLower() && 
                                     i.Status == "pending" &&
                                     i.ExpiresAt > DateTime.UtcNow, 
                                cancellationToken);

        if (invitation == null)
        {
            return new RegisterCommandResponse
            {
                Success = false,
                Message = "Registration requires a valid invitation. Please contact your clinic administrator.",
                Errors = new List<string> { "No valid invitation found for this email address" }
            };
        }

        // SECURITY: Ensure clinic ID matches invitation clinic
        if (request.ClinicId.HasValue && request.ClinicId != invitation.ClinicId)
        {
            return new RegisterCommandResponse
            {
                Success = false,
                Message = "Invalid clinic assignment. Registration denied.",
                Errors = new List<string> { "Clinic ID mismatch with invitation" }
            };
        }

        // Use the clinic ID from the invitation to prevent manipulation
        var (result, userId) = await _identityService.CreateUserAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            invitation.ClinicId);

        if (!result.Succeeded)
        {
            return new RegisterCommandResponse
            {
                Success = false,
                Message = "User registration failed",
                Errors = result.Errors.ToList()
            };
        }

        // SECURITY: Mark invitation as accepted and assign role
        invitation.Status = "accepted";
        invitation.AcceptedAt = DateTime.UtcNow;
        invitation.AcceptedByUserId = userId;

        // Assign the role specified in the invitation
        if (!string.IsNullOrEmpty(invitation.Role))
        {
            await _identityService.AssignRoleAsync(userId, invitation.Role);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new RegisterCommandResponse
        {
            Success = true,
            Message = "User registered successfully with invitation validation",
            UserId = userId,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            ClinicId = invitation.ClinicId,
            Role = invitation.Role
        };
    }
}