using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using Xunit;

namespace EquipmentBorrowing.Test.Student;

public class RegisterStudentServiceTests
{
    [Fact]
    public async Task RegisterStudentAsync_ShouldRegisterStudent_WhenValid()
    {
        // Arrange
        var repository = new FakeStudentRepository();
        var service = new RegisterStudentService(repository);

        // Act
        var student = await service.RegisterStudentAsync(
            "  2024-001  ",
            "Jade",
            "09123456789");

        // Assert
        Assert.Equal("2024-001", student.InstitutionalNumber);
        Assert.Equal("Jade", student.Name);
        Assert.Equal("09123456789", student.ContactInformation);
        Assert.True(student.IsAllowedToBorrow);
        Assert.Single(repository.Students);
    }

    [Fact]
    public async Task RegisterStudentAsync_ShouldRejectBlankInstitutionalNumber()
    {
        var service = new RegisterStudentService(
            new FakeStudentRepository());

        await Assert.ThrowsAsync<System.ArgumentException>(
            () => service.RegisterStudentAsync("", "Jade"));
    }

    [Fact]
    public async Task RegisterStudentAsync_ShouldRejectBlankName()
    {
        var service = new RegisterStudentService(
            new FakeStudentRepository());

        await Assert.ThrowsAsync<System.ArgumentException>(
            () => service.RegisterStudentAsync("2024-001", " "));
    }

    [Fact]
    public async Task RegisterStudentAsync_ShouldRejectDuplicateInstitutionalNumber()
    {
        var repository = new FakeStudentRepository();

        var existingStudent = new EquipmentBorrowing.Domain.Student(
            1,
            "Jade",
            true,
            "2024-001");

        await repository.AddAsync(existingStudent);

        var service = new RegisterStudentService(repository);

        await Assert.ThrowsAsync<System.InvalidOperationException>(
            () => service.RegisterStudentAsync("2024-001", "Another Student"));
    }

    private class FakeStudentRepository : IStudentRepository
    {
        public List<EquipmentBorrowing.Domain.Student> Students { get; }
            = new();

        public Task<EquipmentBorrowing.Domain.Student?> GetByIdAsync(int id)
        {
            return Task.FromResult(
                Students.FirstOrDefault(s => s.Id == id));
        }

        public Task<IReadOnlyList<EquipmentBorrowing.Domain.Student>> GetAllAsync()
        {
            return Task.FromResult<IReadOnlyList<EquipmentBorrowing.Domain.Student>>(
                Students);
        }

        public Task<EquipmentBorrowing.Domain.Student?> GetByInstitutionalNumberAsync(
            string institutionalNumber)
        {
            string normalized = institutionalNumber.Trim().ToUpperInvariant();

            return Task.FromResult(
                Students.FirstOrDefault(
                    s => s.InstitutionalNumber.ToUpperInvariant() == normalized));
        }
        public Task UpdateAsync(EquipmentBorrowing.Domain.Student student)
        {
            var existingStudent = Students.FirstOrDefault(
                s => s.Id == student.Id);

            if (existingStudent is null)
                throw new InvalidOperationException("Student not found.");

            existingStudent.UpdateInformation(
                student.InstitutionalNumber,
                student.Name,
                student.ContactInformation);

            return Task.CompletedTask;
        }
        public Task AddAsync(EquipmentBorrowing.Domain.Student student)
        {
            Students.Add(student);
            return Task.CompletedTask;
        }
        
    }
}