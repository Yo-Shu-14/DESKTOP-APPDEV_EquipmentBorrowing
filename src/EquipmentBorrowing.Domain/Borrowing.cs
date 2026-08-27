using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentBorrowing.Domain;
public class Borrowing
{
    public Guid BorrowingId { get; private set; }
    public Student Student { get; private set; }
    public Equipment Equipment { get; private set; }

    public Borrowing(Guid borrowingId, Student student, Equipment equipment)
    {
        BorrowingId = borrowingId;
        Student = student;
        Equipment = equipment;
    }
}

