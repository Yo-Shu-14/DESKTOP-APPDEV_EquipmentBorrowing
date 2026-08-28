using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;

using DomainBorrowing = EquipmentBorrowing.Domain.Borrowing;
namespace EquipmentBorrowing.Tests.Borrowing;

public class BorrowEquipmentServiceTests
{
    [Fact]
    public async Task BorrowEquipmentAsync_ShouldCreateBorrowing_WhenValid()
    {
        // Arrange
        var student = new Student(
            1,
            "Jade",
            true);

        var equipment = new Equipment(
            Guid.NewGuid(),
            "Laptop",
            "Dell Laptop",
            true);

        var studentRepository = new FakeStudentRepository(student);
        var equipmentRepository = new FakeEquipmentRepository(equipment);
        var borrowingRepository = new FakeBorrowingRepository();

        var service = new BorrowEquipmentService(
            studentRepository,
            equipmentRepository,
            borrowingRepository);

        // Act
        await service.BorrowEquipmentAsync(
            1,
            equipment.EquipmentId,
            3,
            DateTime.Now.AddDays(7));

        // Assert
        Assert.Single(borrowingRepository.Borrowings);

        Assert.Equal(
            BorrowingStatus.Active,
            borrowingRepository.Borrowings[0].Status);
    }


    [Fact]
    public async Task BorrowEquipmentAsync_ShouldThrowException_WhenStudentDoesNotExist()
    {
        // Arrange
        var equipment = new Equipment(
            Guid.NewGuid(),
            "Laptop",
            "Dell Laptop",
            true);

        var studentRepository = new FakeStudentRepository(null);
        var equipmentRepository = new FakeEquipmentRepository(equipment);
        var borrowingRepository = new FakeBorrowingRepository();

        var service = new BorrowEquipmentService(
            studentRepository,
            equipmentRepository,
            borrowingRepository);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.BorrowEquipmentAsync(
                1,
                equipment.EquipmentId,
                3,
                DateTime.Now.AddDays(7)));
    }


    [Fact]
    public async Task BorrowEquipmentAsync_ShouldThrowException_WhenStudentIsNotAllowed()
    {
        // Arrange
        var student = new Student(
            1,
            "Jade",
            false);

        var equipment = new Equipment(
            Guid.NewGuid(),
            "Laptop",
            "Dell Laptop",
            true);

        var studentRepository = new FakeStudentRepository(student);
        var equipmentRepository = new FakeEquipmentRepository(equipment);
        var borrowingRepository = new FakeBorrowingRepository();

        var service = new BorrowEquipmentService(
            studentRepository,
            equipmentRepository,
            borrowingRepository);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.BorrowEquipmentAsync(
                1,
                equipment.EquipmentId,
                3,
                DateTime.Now.AddDays(7)));
    }


    [Fact]
    public async Task BorrowEquipmentAsync_ShouldThrowException_WhenEquipmentDoesNotExist()
    {
        // Arrange
        var student = new Student(
            1,
            "Jade",
            true);

        var studentRepository = new FakeStudentRepository(student);
        var equipmentRepository = new FakeEquipmentRepository(null);
        var borrowingRepository = new FakeBorrowingRepository();

        var service = new BorrowEquipmentService(
            studentRepository,
            equipmentRepository,
            borrowingRepository);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.BorrowEquipmentAsync(
                1,
                Guid.NewGuid(),
                3,
                DateTime.Now.AddDays(7)));
    }


    [Fact]
    public async Task BorrowEquipmentAsync_ShouldThrowException_WhenEquipmentIsNotAvailable()
    {
        // Arrange
        var student = new Student(
            1,
            "Jade",
            true);

        var equipment = new Equipment(
            Guid.NewGuid(),
            "Laptop",
            "Dell Laptop",
            false);

        var studentRepository = new FakeStudentRepository(student);
        var equipmentRepository = new FakeEquipmentRepository(equipment);
        var borrowingRepository = new FakeBorrowingRepository();

        var service = new BorrowEquipmentService(
            studentRepository,
            equipmentRepository,
            borrowingRepository);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.BorrowEquipmentAsync(
                1,
                equipment.EquipmentId,
                3,
                DateTime.Now.AddDays(7)));
    }


    [Fact]
    public async Task BorrowEquipmentAsync_ShouldThrowException_WhenMaximumBorrowingsReached()
    {
        // Arrange
        var student = new Student(
            1,
            "Jade",
            true);

        var equipment = new Equipment(
            Guid.NewGuid(),
            "Laptop",
            "Dell Laptop",
            true);

        var studentRepository = new FakeStudentRepository(student);
        var equipmentRepository = new FakeEquipmentRepository(equipment);

        var borrowingRepository = new FakeBorrowingRepository();

        // Create 3 existing active borrowings
        for (int i = 0; i < 3; i++)
        {
            var existingEquipment = new Equipment(
                Guid.NewGuid(),
                $"Equipment {i}",
                "Test Equipment",
                false);

            var borrowing = new DomainBorrowing(
                Guid.NewGuid(),
                student,
                existingEquipment,
                DateTime.Now,
                DateTime.Now.AddDays(7),
                BorrowingStatus.Active);

            borrowingRepository.Borrowings.Add(borrowing);
        }

        var service = new BorrowEquipmentService(
            studentRepository,
            equipmentRepository,
            borrowingRepository);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.BorrowEquipmentAsync(
                1,
                equipment.EquipmentId,
                3,
                DateTime.Now.AddDays(7)));
    }


    // --------------------------------------------------
    // Fake Student Repository
    // --------------------------------------------------

    private class FakeStudentRepository : IStudentRepository
    {
        private readonly Student? _student;

        public FakeStudentRepository(Student? student)
        {
            _student = student;
        }

        public Task<Student?> GetByIdAsync(int id)
        {
            return Task.FromResult(_student);
        }
    }


    // --------------------------------------------------
    // Fake Equipment Repository
    // --------------------------------------------------

    private class FakeEquipmentRepository : IEquipmentRepository
    {
        private readonly Equipment? _equipment;

        public FakeEquipmentRepository(Equipment? equipment)
        {
            _equipment = equipment;
        }

        public Task<Equipment?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_equipment);
        }

        public Task<IReadOnlyList<Equipment>> GetAllAsync()
        {
            var equipmentList = _equipment is null
                ? new List<Equipment>()
                : new List<Equipment> { _equipment };

            return Task.FromResult<IReadOnlyList<Equipment>>(equipmentList);
        }

        public Task UpdateAsync(Equipment equipment)
        {
            return Task.CompletedTask;
        }
    }


    // --------------------------------------------------
    // Fake Borrowing Repository
    // --------------------------------------------------

    private class FakeBorrowingRepository : IBorrowingRepository
    {
        public List<DomainBorrowing> Borrowings { get; } = new();

        public Task<IReadOnlyList<DomainBorrowing>> GetActiveByStudentIdAsync(int studentId)
        {
            var activeBorrowings = Borrowings
                .Where(b => b.Student.Id == studentId &&
                            b.Status == BorrowingStatus.Active)
                .ToList();

            return Task.FromResult<IReadOnlyList<DomainBorrowing>>(activeBorrowings);
        }

        public Task<DomainBorrowing?> GetByIdAsync(Guid borrowingId)
        {
            var borrowing = Borrowings
                .FirstOrDefault(b => b.BorrowingId == borrowingId);

            return Task.FromResult(borrowing);
        }

        public Task AddAsync(DomainBorrowing borrowing)
        {
            Borrowings.Add(borrowing);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(DomainBorrowing borrowing)
        {
            return Task.CompletedTask;
        }
    }
}