using MediatR;
using Microsoft.AspNetCore.Identity;
using HairAI.Application.Common.Interfaces;
using HairAI.Domain.Entities;

namespace HairAI.Application.Features.Admin.Commands.ToggleUserStatus;

public class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand, ToggleUserStatusCommandResponse>
{
    private readonly IClinicAuthorizationService _authorizationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public ToggleUserStatusCommandHandler(
        IClinicAuthorizationService authorizationService,
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUserService)
    {
        _authorizationService = authorizationService;
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<ToggleUserStatusCommandResponse> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
    {
        // SECURITY: Only SuperAdmin can toggle user status
        if (!await _authorizationService.IsSuperAdminAsync())
        {
            return new ToggleUserStatusCommandResponse
            {
                Success = false,
                Message = "Access denied. Only SuperAdmin can modify user status.",
                Errors = new List<string> { "Insufficient permissions for user status modification" }
            };
        }

        // Prevent self-deactivation
        var currentUserId = _currentUserService.UserId;
        if (request.UserId == currentUserId && !request.IsActivating)
        {
            return new ToggleUserStatusCommandResponse
            {
                Success = false,
                Message = "You cannot deactivate your own account.",
                Errors = new List<string> { "Self-deactivation is not allowed" }
            };
        }

        // Find the user
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return new ToggleUserStatusCommandResponse
            {
                Success = false,
                Message = "User not found.",
                Errors = new List<string> { "User with specified ID does not exist" }
            };
        }

        IdentityResult result;
        string action;

        if (request.IsActivating)
        {
            // Activate user by removing lockout
            result = await _userManager.SetLockoutEndDateAsync(user, null);
            action = "activated";
        }
        else
        {
            // Deactivate user by setting lockout to far future
            result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            action = "deactivated";
        }

        if (!result.Succeeded)
        {
            return new ToggleUserStatusCommandResponse
            {
                Success = false,
                Message = $"Failed to {action} user.",
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        return new ToggleUserStatusCommandResponse
        {
            Success = true,
            Message = $"User {user.Email} has been {action} successfully.",
            IsActive = request.IsActivating
        };
    }
}

