using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class EfStudentRepository : IStudentRepository
{
    private readonly EquipmentBorrowingDbContext _db;

    public EfStudentRepository(EquipmentBorrowingDbContext db)
    {
        _db = db;
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _db.Students
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IReadOnlyList<Student>> GetAllAsync()
    {
        return await _db.Students
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<Student?> GetByInstitutionalNumberAsync(
    string institutionalNumber)
    {
        string normalizedNumber = institutionalNumber.Trim().ToUpper();

        return await _db.Students.FirstOrDefaultAsync(s =>
            s.InstitutionalNumber.ToUpper() == normalizedNumber);
    }

    public async Task AddAsync(Student student)
    {
        try
        {
            await _db.Students.AddAsync(student);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException postgresException
                  && postgresException.SqlState == "23505")
        {
            throw new InvalidOperationException(
                "A student with this institutional number already exists.");
        }
    }
}