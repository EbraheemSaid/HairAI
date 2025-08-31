using HairAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HairAI.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Clinic> Clinics { get; }
    DbSet<Patient> Patients { get; }
    DbSet<CalibrationProfile> CalibrationProfiles { get; }
    DbSet<AnalysisSession> AnalysisSessions { get; }
    DbSet<AnalysisJob> AnalysisJobs { get; }
    DbSet<SubscriptionPlan> SubscriptionPlans { get; }
    DbSet<Subscription> Subscriptions { get; }
    DbSet<Payment> Payments { get; }
    DbSet<ClinicInvitation> ClinicInvitations { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<ApplicationUser> ApplicationUsers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}