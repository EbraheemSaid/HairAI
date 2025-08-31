using MediatR;
using Microsoft.AspNetCore.Identity;
using HairAI.Application.Common.Interfaces;
using HairAI.Domain.Entities;

namespace HairAI.Application.Features.Admin.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, DeleteUserCommandResponse>
{
    private readonly IClinicAuthorizationService _authorizationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public DeleteUserCommandHandler(
        IClinicAuthorizationService authorizationService,
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUserService)
    {
        _authorizationService = authorizationService;
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<DeleteUserCommandResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        // SECURITY: Only SuperAdmin can delete users
        if (!await _authorizationService.IsSuperAdminAsync())
        {
            return new DeleteUserCommandResponse
            {
                Success = false,
                Message = "Access denied. Only SuperAdmin can delete users.",
                Errors = new List<string> { "Insufficient permissions for user deletion" }
            };
        }

        // Prevent self-deletion
        var currentUserId = _currentUserService.UserId;
        if (request.UserId == currentUserId)
        {
            return new DeleteUserCommandResponse
            {
                Success = false,
                Message = "You cannot delete your own account.",
                Errors = new List<string> { "Self-deletion is not allowed" }
            };
        }

        // Find the user to delete
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return new DeleteUserCommandResponse
            {
                Success = false,
                Message = "User not found.",
                Errors = new List<string> { "User with specified ID does not exist" }
            };
        }

        // Delete the user
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return new DeleteUserCommandResponse
            {
                Success = false,
                Message = "Failed to delete user.",
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        return new DeleteUserCommandResponse
        {
            Success = true,
            Message = $"User {user.Email} has been deleted successfully."
        };
    }
}

