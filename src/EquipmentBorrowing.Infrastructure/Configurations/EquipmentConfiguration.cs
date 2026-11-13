using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Configurations;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        // Primary key
        builder.HasKey(e => e.EquipmentId);

        // Required equipment name
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Required description
        builder.Property(e => e.Description)
            .IsRequired(false)
            .HasMaxLength(500);

        // Required availability status
        builder.Property(e => e.IsAvailable)
            .IsRequired();

        // Equipment names are not required to be unique
        // Required equipment identifier
        builder.Property(e => e.EquipmentIdentifier)
            .IsRequired()
            .HasMaxLength(50);

        // Unique equipment identifier
        builder.HasIndex(e => e.EquipmentIdentifier)
            .IsUnique();
    }
}