using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HairAI.Application.Common.Interfaces;
using HairAI.Domain.Entities;

namespace HairAI.Application.Features.Admin.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, GetAllUsersQueryResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllUsersQueryHandler(
        IApplicationDbContext context, 
        IClinicAuthorizationService authorizationService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _authorizationService = authorizationService;
        _userManager = userManager;
    }

    public async Task<GetAllUsersQueryResponse> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        // SECURITY: Only SuperAdmin can view all users
        if (!await _authorizationService.IsSuperAdminAsync())
        {
            return new GetAllUsersQueryResponse
            {
                Success = false,
                Message = "Access denied. Only SuperAdmin can view all users.",
                Errors = new List<string> { "Insufficient permissions to view users" }
            };
        }

        try
        {
            // Get all users with their clinic information
            var users = await _context.ApplicationUsers
                .Include(u => u.Clinic)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync(cancellationToken);

            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);
                var primaryRole = roles.FirstOrDefault() ?? "Unknown";

                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? string.Empty,
                    Role = primaryRole,
                    ClinicId = user.ClinicId,
                    Clinic = user.Clinic != null ? new ClinicDto
                    {
                        Id = user.Clinic.Id,
                        Name = user.Clinic.Name
                    } : null,
                    LastLoginAt = null, // Will be added later
                    IsActive = !user.LockoutEnd.HasValue || user.LockoutEnd <= DateTimeOffset.UtcNow,
                    CreatedAt = DateTime.UtcNow // Placeholder for now
                });
            }

            return new GetAllUsersQueryResponse
            {
                Success = true,
                Message = "Users retrieved successfully",
                Users = userDtos
            };
        }
        catch (Exception ex)
        {
            return new GetAllUsersQueryResponse
            {
                Success = false,
                Message = "Failed to retrieve users",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}
