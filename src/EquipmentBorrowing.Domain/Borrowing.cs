using System;

namespace EquipmentBorrowing.Domain;

public class Borrowing
{
    public Guid BorrowingId { get; private set; }

    public int StudentId { get; private set; }
    public Student Student { get; private set; }

    public Guid EquipmentId { get; private set; }
    public Equipment Equipment { get; private set; }

    public DateTime DateBorrowed { get; private set; }
    public DateTime ExpectedReturnDate { get; private set; }
    public BorrowingStatus Status { get; private set; }

    public Borrowing(
        Guid borrowingId,
        Student student,
        Equipment equipment,
        DateTime dateBorrowed,
        DateTime expectedReturnDate,
        BorrowingStatus status)
    {
        BorrowingId = borrowingId;
        Student = student;
        StudentId = student.Id;

        Equipment = equipment;
        EquipmentId = equipment.EquipmentId;

        DateBorrowed = dateBorrowed;
        ExpectedReturnDate = expectedReturnDate;
        Status = status;
    }

    private Borrowing()
    {
    }
    public void MarkAsReturned()
    {
        Status = BorrowingStatus.Returned;
    }
}