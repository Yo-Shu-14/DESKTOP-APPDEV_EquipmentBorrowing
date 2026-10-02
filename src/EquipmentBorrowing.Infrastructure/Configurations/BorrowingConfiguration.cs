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

        // Store BorrowingStatus as text in SQLite
        builder.Property(b => b.Status)
            .HasConversion<string>()
            .IsRequired();

        // Relationship with Student
        builder.HasOne(b => b.Student)
            .WithMany()
            .IsRequired();

        // Relationship with Equipment
        builder.HasOne(b => b.Equipment)
            .WithMany()
            .IsRequired();

        // Indexes for foreign-key lookups
        builder.HasIndex(b => b.StudentId);

        builder.HasIndex(b => b.EquipmentId);
    }
}