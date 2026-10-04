using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Configurations;

public class BorrowingConfiguration : IEntityTypeConfiguration<Borrowing>
{
    public void Configure(EntityTypeBuilder<Borrowing> builder)
    {
        // Primary key
        builder.HasKey(b => b.BorrowingId);

        // Required dates
        builder.Property(b => b.DateBorrowed)
            .IsRequired();

        builder.Property(b => b.ExpectedReturnDate)
            .IsRequired();

        // Store enum as text
        builder.Property(b => b.Status)
            .HasConversion<string>()
            .IsRequired();

        // Student relationship
        builder.HasOne(b => b.Student)
            .WithMany()
            .HasForeignKey(b => b.StudentId)
            .IsRequired();

        // Equipment relationship
        builder.HasOne(b => b.Equipment)
            .WithMany()
            .HasForeignKey(b => b.EquipmentId)
            .IsRequired();

        // Indexes
        builder.HasIndex(b => b.StudentId);
        builder.HasIndex(b => b.EquipmentId);
    }
}