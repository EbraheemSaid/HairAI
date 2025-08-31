using HairAI.Application.Common.Interfaces;
using HairAI.Application.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HairAI.Domain.Entities;

namespace HairAI.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<string> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.UserName ?? string.Empty;
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        // SECURITY FIX: Implement proper policy-based authorization
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(policyName))
        {
            return false;
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        // Define policy-to-role mappings
        return policyName.ToLowerInvariant() switch
        {
            // SuperAdmin policies
            "superadmin" => userRoles.Contains("SuperAdmin"),
            "admin.users.manage" => userRoles.Contains("SuperAdmin"),
            "admin.clinics.manage" => userRoles.Contains("SuperAdmin"),
            "admin.subscriptions.manage" => userRoles.Contains("SuperAdmin"),
            "admin.payments.manage" => userRoles.Contains("SuperAdmin"),
            "admin.system.access" => userRoles.Contains("SuperAdmin"),
            
            // ClinicAdmin policies
            "clinic.admin" => userRoles.Contains("SuperAdmin") || userRoles.Contains("ClinicAdmin"),
            "clinic.users.manage" => userRoles.Contains("SuperAdmin") || userRoles.Contains("ClinicAdmin"),
            "clinic.patients.manage" => userRoles.Contains("SuperAdmin") || userRoles.Contains("ClinicAdmin"),
            "clinic.settings.manage" => userRoles.Contains("SuperAdmin") || userRoles.Contains("ClinicAdmin"),
            
            // Doctor policies
            "patients.view" => userRoles.Contains("SuperAdmin") || userRoles.Contains("ClinicAdmin") || userRoles.Contains("Doctor"),
            "patients.create" => userRoles.Contains("SuperAdmin") || userRoles.Contains("ClinicAdmin") || userRoles.Contains("Doctor"),
            "patients.update" => userRoles.Contains("SuperAdmin") || userRoles.Contains("ClinicAdmin") || userRoles.Contains("Doctor"),
            "analysis.create" => userRoles.Contains("SuperAdmin") || userRoles.Contains("ClinicAdmin") || userRoles.Contains("Doctor"),
            "analysis.view" => userRoles.Contains("SuperAdmin") || userRoles.Contains("ClinicAdmin") || userRoles.Contains("Doctor"),
            
            // Calibration policies
            "calibration.manage" => userRoles.Contains("SuperAdmin") || userRoles.Contains("ClinicAdmin"),
            "calibration.view" => userRoles.Contains("SuperAdmin") || userRoles.Contains("ClinicAdmin") || userRoles.Contains("Doctor"),
            
            // Reports policies
            "reports.generate" => userRoles.Contains("SuperAdmin") || userRoles.Contains("ClinicAdmin") || userRoles.Contains("Doctor"),
            "reports.view" => userRoles.Contains("SuperAdmin") || userRoles.Contains("ClinicAdmin") || userRoles.Contains("Doctor"),
            
            // Invitations policies
            "invitations.create" => userRoles.Contains("SuperAdmin") || userRoles.Contains("ClinicAdmin"),
            "invitations.manage" => userRoles.Contains("SuperAdmin") || userRoles.Contains("ClinicAdmin"),
            
            // Default deny - SECURITY: Unknown policies are denied
            _ => false
        };
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string email, string password, string firstName, string lastName, Guid? clinicId = null)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            ClinicId = clinicId
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        // This method is kept for interface compatibility but redirects to the main method
        return await CreateUserAsync(userName, password, "", "", null);
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user != null)
        {
            var result = await _userManager.DeleteAsync(user);
            return result.ToApplicationResult();
        }

        return Result.Success();
    }

    public async Task<Result> AssignRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return Result.Failure(new[] { "User not found" });
        }

        var result = await _userManager.AddToRoleAsync(user, role);
        return result.ToApplicationResult();
    }

    public async Task<(bool Success, ApplicationUser? User, string Message)> AuthenticateAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return (false, null, "Invalid email or password");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (result.Succeeded)
        {
            return (true, user, "Authentication successful");
        }

        return (false, null, "Invalid email or password");
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }
}

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }
}