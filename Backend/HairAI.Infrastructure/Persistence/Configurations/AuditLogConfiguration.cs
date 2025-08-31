using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HairAI.Domain.Entities;

namespace HairAI.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.Property(e => e.ActionType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Timestamp)
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd();

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => new { e.TargetEntityType, e.TargetEntityId });
    }
}