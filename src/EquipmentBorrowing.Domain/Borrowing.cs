using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentBorrowing.Domain;
public class Borrowing
{
    public Guid BorrowingId { get; private set; }
    public Student Student { get; private set; }
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
        Equipment = equipment;
        DateBorrowed = dateBorrowed;
        ExpectedReturnDate = expectedReturnDate;
        Status = status;
    }
    public void MarkAsReturned()
    {
        Status = BorrowingStatus.Returned;
    }
}

