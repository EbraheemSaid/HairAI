using Microsoft.EntityFrameworkCore;
using HairAI.Application.Common.Interfaces;
using HairAI.Domain.Entities;
using HairAI.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HairAI.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    private readonly ICurrentUserService? _currentUserService;
    private readonly ILogger<ApplicationDbContext>? _logger;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger<ApplicationDbContext> logger) : base(options)
    {
        _currentUserService = null;
        _logger = logger;
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService, ILogger<ApplicationDbContext> logger) 
        : base(options)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public DbSet<Clinic> Clinics => Set<Clinic>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<CalibrationProfile> CalibrationProfiles => Set<CalibrationProfile>();
    public DbSet<AnalysisSession> AnalysisSessions => Set<AnalysisSession>();
    public DbSet<AnalysisJob> AnalysisJobs => Set<AnalysisJob>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ClinicInvitation> ClinicInvitations => Set<ClinicInvitation>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentUser = _currentUserService?.UserId;
        var timestamp = DateTime.UtcNow;
        var changeCount = ChangeTracker.Entries().Count();

        // PERFORMANCE: Log large change sets that might indicate performance issues
        if (changeCount > 100)
        {
            _logger?.LogWarning("Large change set detected: {ChangeCount} entities being saved", changeCount);
        }

        // Collect audit logs to avoid modifying collection during enumeration
        var auditLogsToCreate = new List<(string actionType, object entity, object? beforeValue, object? afterValue)>();

        // Handle audit logging and timestamps
        foreach (var entry in ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity is Clinic clinic)
                    {
                        clinic.CreatedAt = timestamp;
                        clinic.UpdatedAt = timestamp;
                        auditLogsToCreate.Add(("CREATE_CLINIC", entry.Entity, null, clinic));
                    }
                    else if (entry.Entity is Patient patient)
                    {
                        patient.CreatedAt = timestamp;
                        auditLogsToCreate.Add(("CREATE_PATIENT", entry.Entity, null, patient));
                    }
                    else if (entry.Entity is CalibrationProfile profile)
                    {
                        profile.CreatedAt = timestamp;
                        auditLogsToCreate.Add(("CREATE_CALIBRATION_PROFILE", entry.Entity, null, profile));
                    }
                    else if (entry.Entity is AnalysisSession session)
                    {
                        session.CreatedAt = timestamp;
                        auditLogsToCreate.Add(("CREATE_ANALYSIS_SESSION", entry.Entity, null, session));
                    }
                    else if (entry.Entity is AnalysisJob job)
                    {
                        job.CreatedAt = timestamp;
                        auditLogsToCreate.Add(("CREATE_ANALYSIS_JOB", entry.Entity, null, job));
                    }
                    break;

                case EntityState.Modified:
                    if (entry.Entity is Clinic modifiedClinic)
                    {
                        modifiedClinic.UpdatedAt = timestamp;
                        auditLogsToCreate.Add(("UPDATE_CLINIC", entry.Entity, entry.OriginalValues.ToObject(), modifiedClinic));
                    }
                    else if (entry.Entity is Patient modifiedPatient)
                    {
                        auditLogsToCreate.Add(("UPDATE_PATIENT", entry.Entity, entry.OriginalValues.ToObject(), modifiedPatient));
                    }
                    else if (entry.Entity is AnalysisJob modifiedJob)
                    {
                        auditLogsToCreate.Add(("UPDATE_ANALYSIS_JOB", entry.Entity, entry.OriginalValues.ToObject(), modifiedJob));
                    }
                    break;

                case EntityState.Deleted:
                    auditLogsToCreate.Add(("DELETE", entry.Entity, entry.Entity, null));
                    break;
            }
        }

        // Create audit logs after enumeration to avoid collection modification during iteration
        foreach (var (actionType, entity, beforeValue, afterValue) in auditLogsToCreate)
        {
            try
            {
                await CreateAuditLogEntryAsync(actionType, entity, currentUser, beforeValue, afterValue);
            }
            catch (Exception ex)
            {
                // Don't fail the main operation if audit logging fails
                _logger?.LogWarning(ex, "Failed to create audit log for {ActionType} on {EntityType}", actionType, entity?.GetType().Name);
            }
        }

        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error saving changes to database");
            throw;
        }
    }

    private async Task CreateAuditLogEntryAsync(string actionType, object entity, string? userId, object? beforeValue, object? afterValue)
    {
        try
        {
            var entityType = entity.GetType().Name;
            var entityId = GetEntityId(entity);

            // MEMORY MANAGEMENT: Limit the size of serialized data
            string details;
            try
            {
                var auditData = new
                {
                    Before = beforeValue,
                    After = afterValue,
                    Timestamp = DateTime.UtcNow
                };
                
                details = JsonSerializer.Serialize(auditData, new JsonSerializerOptions 
                { 
                    WriteIndented = false,
                    MaxDepth = 3 // Limit depth to prevent circular references
                });
                
                // Limit size to prevent oversized audit logs
                if (details.Length > 10000)
                {
                    details = details.Substring(0, 9997) + "...";
                }
            }
            catch (Exception serializeEx)
            {
                _logger?.LogWarning(serializeEx, "Failed to serialize audit data for {EntityType} {EntityId}", entityType, entityId);
                details = $"{{\"error\": \"Failed to serialize audit data: {serializeEx.Message}\"}}";
            }

            var auditLog = new AuditLog
            {
                UserId = userId,
                TargetEntityType = entityType,
                TargetEntityId = entityId,
                ActionType = actionType,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            AuditLogs.Add(auditLog);
        }
        catch (Exception ex)
        {
            // Don't fail the main operation if audit logging fails
            _logger?.LogWarning(ex, "Failed to create audit log for {ActionType} on {EntityType}", actionType, entity?.GetType().Name);
        }
    }

    private string GetEntityId(object entity)
    {
        return entity switch
        {
            Clinic clinic => clinic.Id.ToString(),
            Patient patient => patient.Id.ToString(),
            CalibrationProfile profile => profile.Id.ToString(),
            AnalysisSession session => session.Id.ToString(),
            AnalysisJob job => job.Id.ToString(),
            SubscriptionPlan plan => plan.Id.ToString(),
            Subscription subscription => subscription.Id.ToString(),
            Payment payment => payment.Id.ToString(),
            ClinicInvitation invitation => invitation.Id.ToString(),
            ApplicationUser user => user.Id,
            _ => "unknown"
        };
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configure relationships
        modelBuilder.Entity<Clinic>()
            .HasMany(c => c.Patients)
            .WithOne(p => p.Clinic)
            .HasForeignKey(p => p.ClinicId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Clinic>()
            .HasMany(c => c.CalibrationProfiles)
            .WithOne(cp => cp.Clinic)
            .HasForeignKey(cp => cp.ClinicId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Clinic>()
            .HasOne(c => c.Subscription)
            .WithOne(s => s.Clinic)
            .HasForeignKey<Subscription>(s => s.ClinicId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Clinic>()
            .HasMany(c => c.ClinicInvitations)
            .WithOne(ci => ci.Clinic)
            .HasForeignKey(ci => ci.ClinicId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Patient>()
            .HasMany(p => p.AnalysisSessions)
            .WithOne(asession => asession.Patient)
            .HasForeignKey(asession => asession.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Patient>()
            .HasMany(p => p.AnalysisJobs)
            .WithOne(aj => aj.Patient)
            .HasForeignKey(aj => aj.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CalibrationProfile>()
            .HasMany(cp => cp.AnalysisJobs)
            .WithOne(aj => aj.CalibrationProfile)
            .HasForeignKey(aj => aj.CalibrationProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AnalysisSession>()
            .HasMany(asession => asession.AnalysisJobs)
            .WithOne(aj => aj.Session)
            .HasForeignKey(aj => aj.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SubscriptionPlan>()
            .HasMany(sp => sp.Subscriptions)
            .WithOne(s => s.Plan)
            .HasForeignKey(s => s.PlanId)
            .OnDelete(DeleteBehavior.Restrict); // SAFETY: Prevent accidental plan deletion cascading to subscriptions

        modelBuilder.Entity<Subscription>()
            .HasMany(s => s.Payments)
            .WithOne(p => p.Subscription)
            .HasForeignKey(p => p.SubscriptionId)
            .OnDelete(DeleteBehavior.Restrict); // SAFETY: Preserve payment records even if subscription is deleted

        // Configure ApplicationUser to extend IdentityUser
        modelBuilder.Entity<ApplicationUser>()
            .ToTable("AspNetUsers");

        // Add indexes for performance - CRITICAL MISSING INDEXES ADDED
        modelBuilder.Entity<Patient>()
            .HasIndex(p => new { p.ClinicId, p.LastName, p.FirstName })
            .HasDatabaseName("IX_Patients_Clinic_Name");

        modelBuilder.Entity<CalibrationProfile>()
            .HasIndex(cp => new { cp.ClinicId, cp.IsActive })
            .HasDatabaseName("IX_CalibrationProfiles_Clinic_Active");

        modelBuilder.Entity<AnalysisJob>()
            .HasIndex(aj => aj.Status)
            .HasDatabaseName("IX_AnalysisJobs_Status");

        modelBuilder.Entity<AnalysisJob>()
            .HasIndex(aj => new { aj.Status, aj.CreatedAt })
            .HasDatabaseName("IX_AnalysisJobs_Queue")
            .HasFilter("Status = 'pending'");

        // CRITICAL PERFORMANCE FIX: Add missing SessionId index for analysis jobs
        modelBuilder.Entity<AnalysisJob>()
            .HasIndex(aj => aj.SessionId)
            .HasDatabaseName("IX_AnalysisJobs_SessionId");

        // CRITICAL PERFORMANCE FIX: Add composite index for session-status queries
        modelBuilder.Entity<AnalysisJob>()
            .HasIndex(aj => new { aj.SessionId, aj.Status })
            .HasDatabaseName("IX_AnalysisJobs_Session_Status");

        // CRITICAL PERFORMANCE FIX: Add index for patient queries
        modelBuilder.Entity<AnalysisSession>()
            .HasIndex(s => new { s.PatientId, s.CreatedAt })
            .HasDatabaseName("IX_AnalysisSessions_Patient_Created");

        // CRITICAL PERFORMANCE FIX: Add index for user-based queries
        modelBuilder.Entity<AnalysisSession>()
            .HasIndex(s => s.CreatedByUserId)
            .HasDatabaseName("IX_AnalysisSessions_CreatedBy");

        // CRITICAL PERFORMANCE FIX: Add index for clinic invitation lookups
        modelBuilder.Entity<ClinicInvitation>()
            .HasIndex(i => new { i.Email, i.Status, i.ExpiresAt })
            .HasDatabaseName("IX_ClinicInvitations_Email_Status_Expires");

        // CRITICAL PERFORMANCE FIX: Add index for subscription queries
        modelBuilder.Entity<Subscription>()
            .HasIndex(s => new { s.ClinicId, s.Status })
            .HasDatabaseName("IX_Subscriptions_Clinic_Status");

        modelBuilder.Entity<AuditLog>()
            .HasIndex(al => al.UserId)
            .HasDatabaseName("IX_AuditLogs_UserId");

        modelBuilder.Entity<AuditLog>()
            .HasIndex(al => new { al.TargetEntityType, al.TargetEntityId })
            .HasDatabaseName("IX_AuditLogs_Entity");

        // CRITICAL PERFORMANCE FIX: Add timestamp index for audit log queries
        modelBuilder.Entity<AuditLog>()
            .HasIndex(al => al.Timestamp)
            .HasDatabaseName("IX_AuditLogs_Timestamp");

        base.OnModelCreating(modelBuilder);
    }
}