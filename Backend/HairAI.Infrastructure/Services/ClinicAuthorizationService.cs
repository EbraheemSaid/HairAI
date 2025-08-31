using HairAI.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HairAI.Domain.Entities;

namespace HairAI.Infrastructure.Services;

public class ClinicAuthorizationService : IClinicAuthorizationService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ClinicAuthorizationService(
        IApplicationDbContext context, 
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<bool> CanAccessClinicAsync(Guid clinicId, string? userId = null)
    {
        userId ??= _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId)) return false;

        // SuperAdmin can access any clinic
        if (await IsSuperAdminAsync(userId)) return true;

        // Get user's clinic
        var userClinicId = await GetUserClinicIdAsync(userId);
        return userClinicId == clinicId;
    }

    public async Task<bool> CanAccessPatientAsync(Guid patientId, string? userId = null)
    {
        userId ??= _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId)) return false;

        // SuperAdmin can access any patient
        if (await IsSuperAdminAsync(userId)) return true;

        // Get patient's clinic
        var patient = await _context.Patients
            .Where(p => p.Id == patientId)
            .Select(p => p.ClinicId)
            .FirstOrDefaultAsync();

        if (patient == Guid.Empty) return false;

        // Check if user can access the patient's clinic
        return await CanAccessClinicAsync(patient, userId);
    }

    public async Task<bool> CanAccessAnalysisSessionAsync(Guid sessionId, string? userId = null)
    {
        userId ??= _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId)) return false;

        // SuperAdmin can access any session
        if (await IsSuperAdminAsync(userId)) return true;

        // Get session's clinic through patient
        var sessionClinicId = await _context.AnalysisSessions
            .Where(s => s.Id == sessionId)
            .Include(s => s.Patient)
            .Select(s => s.Patient.ClinicId)
            .FirstOrDefaultAsync();

        if (sessionClinicId == Guid.Empty) return false;

        // Check if user can access the session's clinic
        return await CanAccessClinicAsync(sessionClinicId, userId);
    }

    public async Task<bool> CanAccessCalibrationProfileAsync(Guid profileId, string? userId = null)
    {
        userId ??= _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId)) return false;

        // SuperAdmin can access any profile
        if (await IsSuperAdminAsync(userId)) return true;

        // Get profile's clinic
        var profileClinicId = await _context.CalibrationProfiles
            .Where(cp => cp.Id == profileId)
            .Select(cp => cp.ClinicId)
            .FirstOrDefaultAsync();

        if (profileClinicId == Guid.Empty) return false;

        // Check if user can access the profile's clinic
        return await CanAccessClinicAsync(profileClinicId, userId);
    }

    public async Task<Guid?> GetUserClinicIdAsync(string? userId = null)
    {
        userId ??= _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId)) return null;

        var user = await _context.ApplicationUsers
            .Where(u => u.Id == userId)
            .Select(u => u.ClinicId)
            .FirstOrDefaultAsync();

        return user;
    }

    public async Task<bool> IsSuperAdminAsync(string? userId = null)
    {
        userId ??= _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId)) return false;

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var roles = await _userManager.GetRolesAsync(user);
        return roles.Contains("SuperAdmin");
    }

    public async Task<bool> IsClinicAdminAsync(string? userId = null)
    {
        userId ??= _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId)) return false;

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var roles = await _userManager.GetRolesAsync(user);
        return roles.Contains("ClinicAdmin") || roles.Contains("SuperAdmin");
    }

    public async Task<List<Guid>> GetUserAccessibleClinicsAsync(string? userId = null)
    {
        userId ??= _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId)) return new List<Guid>();

        // SuperAdmin can access all clinics
        if (await IsSuperAdminAsync(userId))
        {
            return await _context.Clinics
                .Select(c => c.Id)
                .ToListAsync();
        }

        // Regular users can only access their clinic
        var userClinicId = await GetUserClinicIdAsync(userId);
        return userClinicId.HasValue ? new List<Guid> { userClinicId.Value } : new List<Guid>();
    }
} 