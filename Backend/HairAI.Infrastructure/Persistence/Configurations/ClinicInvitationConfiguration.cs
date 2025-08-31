using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HairAI.Domain.Entities;

namespace HairAI.Infrastructure.Persistence.Configurations;

public class ClinicInvitationConfiguration : IEntityTypeConfiguration<ClinicInvitation>
{
    public void Configure(EntityTypeBuilder<ClinicInvitation> builder)
    {
        builder.Property(e => e.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.Role)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Token)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.InvitedByUserId)
            .IsRequired();

        builder.HasIndex(e => e.Token)
            .IsUnique();

        builder.HasIndex(e => e.Email);
    }
}