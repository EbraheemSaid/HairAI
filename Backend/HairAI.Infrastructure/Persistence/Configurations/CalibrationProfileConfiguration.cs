using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HairAI.Domain.Entities;

namespace HairAI.Infrastructure.Persistence.Configurations;

public class CalibrationProfileConfiguration : IEntityTypeConfiguration<CalibrationProfile>
{
    public void Configure(EntityTypeBuilder<CalibrationProfile> builder)
    {
        builder.Property(e => e.ProfileName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Version)
            .HasDefaultValue(1);

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd();

        builder.HasIndex(e => new { e.ClinicId, e.ProfileName, e.Version })
            .IsUnique();
    }
}