using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentBorrowing.Domain;

public class Student
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public bool IsAllowedToBorrow { get; private set; }


    //constructor
    public Student(int Id, string Name, bool IsAllowedToBorrow)
    {
        this.Id = Id;
        this.Name = Name;
        this.IsAllowedToBorrow = IsAllowedToBorrow;
    }
}
