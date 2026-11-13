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

        Assert.False(equipment.IsAvailable);
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

        Assert.Empty(borrowingRepository.Borrowings);
        Assert.True(equipment.IsAvailable);
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

        Assert.True(equipment.IsAvailable);
        Assert.Equal(3, borrowingRepository.Borrowings.Count);
    }

    [Fact]
    public async Task BorrowEquipmentAsync_ShouldNotCreateBorrowing_WhenExpectedReturnDateIsInThePast()
    {
        // Arrange
        var student = new Student(1, "Jade", true);

        var equipment = new Equipment(
            Guid.NewGuid(),
            "Laptop",
            "Dell Laptop",
            true);

        var borrowingRepository = new FakeBorrowingRepository();

        var service = new BorrowEquipmentService(
            new FakeStudentRepository(student),
            new FakeEquipmentRepository(equipment),
            borrowingRepository);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.BorrowEquipmentAsync(
                1,
                equipment.EquipmentId,
                3,
                DateTime.Today.AddDays(-1)));

        Assert.Empty(borrowingRepository.Borrowings);
        Assert.True(equipment.IsAvailable);
    }
    [Fact]
    public async Task BorrowEquipmentAsync_ShouldNotCreateBorrowing_WhenEquipmentIsUnavailable()
    {
        // Arrange
        var student = new Student(1, "Jade", true);

        var equipment = new Equipment(
            Guid.NewGuid(),
            "Laptop",
            "Dell Laptop",
            false);

        var borrowingRepository = new FakeBorrowingRepository();

        var service = new BorrowEquipmentService(
            new FakeStudentRepository(student),
            new FakeEquipmentRepository(equipment),
            borrowingRepository);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.BorrowEquipmentAsync(
                1,
                equipment.EquipmentId,
                3,
                DateTime.Today.AddDays(7)));

        Assert.Empty(borrowingRepository.Borrowings);
        Assert.False(equipment.IsAvailable);
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

        public Task<IReadOnlyList<Student>> GetAllAsync()
        {
            var students = _student is null
                ? new List<Student>()
                : new List<Student> { _student };

            return Task.FromResult<IReadOnlyList<Student>>(students);
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

        public Task<IReadOnlyList<Equipment>> GetAvailableAsync()
        {
            var equipmentList = _equipment is not null && _equipment.IsAvailable
                ? new List<Equipment> { _equipment }
                : new List<Equipment>();

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

    public class FakeBorrowingRepository : IBorrowingRepository
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

        public Task<IReadOnlyList<DomainBorrowing>> GetAllActiveAsync()
        {
            var active = Borrowings.Where(b => b.Status == BorrowingStatus.Active).ToList();
            return Task.FromResult<IReadOnlyList<DomainBorrowing>>(active);
        }
        public Task<IReadOnlyList<DomainBorrowing>> GetAllAsync()
        {
            IReadOnlyList<DomainBorrowing> result = Borrowings
                .OrderByDescending(b => b.DateBorrowed)
                .ToList();

            return Task.FromResult(result);
        }
        public Task AddBorrowingWithEquipmentUpdateAsync(
            EquipmentBorrowing.Domain.Borrowing borrowing,
            Equipment equipment)
        {
            equipment.MarkAsUnavailable();
            Borrowings.Add(borrowing);
            return Task.CompletedTask;
        }
        [Fact]
        public async Task BorrowEquipmentAsync_ShouldThrowException_WhenExpectedReturnDateIsInThePast()
        {
            // Arrange
            var student = new Student(1, "Jade", true);

            var equipment = new Equipment(
                Guid.NewGuid(),
                "Laptop",
                "Dell Laptop",
                true);

            var service = new BorrowEquipmentService(
                new FakeStudentRepository(student),
                new FakeEquipmentRepository(equipment),
                new FakeBorrowingRepository());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.BorrowEquipmentAsync(
                    1,
                    equipment.EquipmentId,
                    3,
                    DateTime.Today.AddDays(-1)));
        }
        public Task ReturnBorrowingWithEquipmentUpdateAsync(
            DomainBorrowing borrowing,
            Equipment equipment)
                {
                    borrowing.MarkAsReturned();
                    equipment.MarkAsAvailable();

                    return Task.CompletedTask;
        }
        public Task AddBorrowingsWithEquipmentUpdatesAsync(
            IReadOnlyList<DomainBorrowing> borrowings,
            IReadOnlyList<Equipment> equipment)
                {
                    foreach (var item in equipment)
                    {
                        item.MarkAsUnavailable();
                    }

                    foreach (var borrowing in borrowings)
                    {
                        Borrowings.Add(borrowing);
                    }

                    return Task.CompletedTask;
        }
        public Task<IReadOnlyList<DomainBorrowing>> GetAllByStudentIdAsync(
            int studentId)
                {
                    IReadOnlyList<DomainBorrowing> result = Borrowings
                        .Where(b => b.Student.Id == studentId)
                        .ToList();

                    return Task.FromResult(result);
        }
        public Task<IReadOnlyList<DomainBorrowing>> GetAllByEquipmentIdAsync(
            Guid equipmentId)
                {
                    IReadOnlyList<DomainBorrowing> result = Borrowings
                        .Where(b => b.Equipment.EquipmentId == equipmentId)
                        .OrderByDescending(b => b.DateBorrowed)
                        .ToList();

                    return Task.FromResult(result);
        }
    }
    
}