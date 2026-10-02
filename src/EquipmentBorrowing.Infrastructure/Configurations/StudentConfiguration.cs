using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        // Primary key
        builder.HasKey(s => s.Id);

        // Required name
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Required borrowing permission
        builder.Property(s => s.IsAllowedToBorrow)
            .IsRequired();

        // Student names do not need to be unique
    }
}