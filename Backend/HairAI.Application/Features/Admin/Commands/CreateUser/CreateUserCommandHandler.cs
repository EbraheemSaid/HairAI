using MediatR;
using Microsoft.AspNetCore.Identity;
using HairAI.Application.Common.Interfaces;
using HairAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HairAI.Application.Features.Admin.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateUserCommandHandler(
        IApplicationDbContext context, 
        IClinicAuthorizationService authorizationService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _authorizationService = authorizationService;
        _userManager = userManager;
    }

    public async Task<CreateUserCommandResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // SECURITY: Only SuperAdmin can create users
        if (!await _authorizationService.IsSuperAdminAsync())
        {
            return new CreateUserCommandResponse
            {
                Success = false,
                Message = "Access denied. Only SuperAdmin can create users.",
                Errors = new List<string> { "Insufficient permissions for user creation" }
            };
        }

        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new CreateUserCommandResponse
            {
                Success = false,
                Message = "User with this email already exists.",
                Errors = new List<string> { "Email address is already in use" }
            };
        }

        // Validate clinic exists if ClinicId is provided
        if (request.ClinicId.HasValue)
        {
            var clinicExists = await _context.Clinics
                .AnyAsync(c => c.Id == request.ClinicId.Value, cancellationToken);
            
            if (!clinicExists)
            {
                return new CreateUserCommandResponse
                {
                    Success = false,
                    Message = "Specified clinic does not exist.",
                    Errors = new List<string> { "Invalid clinic ID" }
                };
            }
        }

        // Create the user
        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            EmailConfirmed = true,
            ClinicId = request.ClinicId
        };

        // Generate a temporary password
        var tempPassword = GenerateTemporaryPassword();
        
        var result = await _userManager.CreateAsync(user, tempPassword);
        if (!result.Succeeded)
        {
            return new CreateUserCommandResponse
            {
                Success = false,
                Message = "Failed to create user.",
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        // Assign role
        var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
        if (!roleResult.Succeeded)
        {
            // Clean up - delete the user if role assignment fails
            await _userManager.DeleteAsync(user);
            
            return new CreateUserCommandResponse
            {
                Success = false,
                Message = "Failed to assign role to user.",
                Errors = roleResult.Errors.Select(e => e.Description).ToList()
            };
        }

        return new CreateUserCommandResponse
        {
            Success = true,
            Message = "User created successfully. Temporary password has been generated.",
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = request.Role,
            ClinicId = request.ClinicId
        };
    }

    private static string GenerateTemporaryPassword()
    {
        // Generate a secure temporary password
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

