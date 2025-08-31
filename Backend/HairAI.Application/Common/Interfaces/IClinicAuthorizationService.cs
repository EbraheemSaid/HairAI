using HairAI.Domain.Entities;

namespace HairAI.Application.Common.Interfaces;

public interface IClinicAuthorizationService
{
    Task<bool> CanAccessClinicAsync(Guid clinicId, string? userId = null);
    Task<bool> CanAccessPatientAsync(Guid patientId, string? userId = null);
    Task<bool> CanAccessAnalysisSessionAsync(Guid sessionId, string? userId = null);
    Task<bool> CanAccessCalibrationProfileAsync(Guid profileId, string? userId = null);
    Task<Guid?> GetUserClinicIdAsync(string? userId = null);
    Task<bool> IsSuperAdminAsync(string? userId = null);
    Task<bool> IsClinicAdminAsync(string? userId = null);
    Task<List<Guid>> GetUserAccessibleClinicsAsync(string? userId = null);
} 