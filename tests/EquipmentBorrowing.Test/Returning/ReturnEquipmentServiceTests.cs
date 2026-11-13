using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EquipmentBorrowing.Tests.Returning;

public class ReturnEquipmentServiceTests
{
    [Fact]
    public async Task ReturnEquipmentAsync_ShouldReturnEquipment_WhenBorrowingIsActive()
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

        var borrowing = new EquipmentBorrowing.Domain.Borrowing(
            Guid.NewGuid(),
            student,
            equipment,
            DateTime.Now.AddDays(-3),
            DateTime.Now.AddDays(4),
            BorrowingStatus.Active);

        var borrowingRepository = new FakeBorrowingRepository();
        var equipmentRepository = new FakeEquipmentRepository(equipment);

        borrowingRepository.Borrowings.Add(borrowing);

        var service = new ReturnEquipmentService(
            borrowingRepository);

        // Act
        await service.ReturnEquipmentAsync(borrowing.BorrowingId);

        // Assert
        Assert.Equal(
            BorrowingStatus.Returned,
            borrowing.Status);

        Assert.True(equipment.IsAvailable);
    }


    [Fact]
    public async Task ReturnEquipmentAsync_ShouldThrowException_WhenBorrowingDoesNotExist()
    {
        // Arrange
        var borrowingRepository = new FakeBorrowingRepository();
        var equipmentRepository = new FakeEquipmentRepository(null);

        var service = new ReturnEquipmentService(
            borrowingRepository);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ReturnEquipmentAsync(Guid.NewGuid()));
    }


    [Fact]
    public async Task ReturnEquipmentAsync_ShouldThrowException_WhenBorrowingIsAlreadyReturned()
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

        var borrowing = new EquipmentBorrowing.Domain.Borrowing(
            Guid.NewGuid(),
            student,
            equipment,
            DateTime.Now.AddDays(-3),
            DateTime.Now.AddDays(4),
            BorrowingStatus.Returned);

        var borrowingRepository = new FakeBorrowingRepository();
        var equipmentRepository = new FakeEquipmentRepository(equipment);

        borrowingRepository.Borrowings.Add(borrowing);

        var service = new ReturnEquipmentService(
            borrowingRepository);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ReturnEquipmentAsync(borrowing.BorrowingId));
    }


    // --------------------------------------------------
    // Fake Borrowing Repository
    // --------------------------------------------------

    private class FakeBorrowingRepository : IBorrowingRepository
    {
        public List<EquipmentBorrowing.Domain.Borrowing> Borrowings { get; } = new();

        public Task<IReadOnlyList<EquipmentBorrowing.Domain.Borrowing>> GetActiveByStudentIdAsync(int studentId)
        {
            var activeBorrowings = Borrowings
                .Where(b => b.Student.Id == studentId &&
                            b.Status == BorrowingStatus.Active)
                .ToList();

            return Task.FromResult<IReadOnlyList<EquipmentBorrowing.Domain.Borrowing>>(
                activeBorrowings);
        }

        public Task<IReadOnlyList<EquipmentBorrowing.Domain.Borrowing>> GetAllActiveAsync()
        {
            var active = Borrowings.Where(b => b.Status == BorrowingStatus.Active).ToList();
            return Task.FromResult<IReadOnlyList<EquipmentBorrowing.Domain.Borrowing>>(active);
        }
        public Task<IReadOnlyList<EquipmentBorrowing.Domain.Borrowing>> GetAllAsync()
        {
            IReadOnlyList<EquipmentBorrowing.Domain.Borrowing> result =
                Borrowings
                    .OrderByDescending(b => b.DateBorrowed)
                    .ToList();

            return Task.FromResult(result);
        }
        public Task<EquipmentBorrowing.Domain.Borrowing?> GetByIdAsync(Guid borrowingId)
        {
            var borrowing = Borrowings
                .FirstOrDefault(b => b.BorrowingId == borrowingId);

            return Task.FromResult(borrowing);
        }

        public Task AddAsync(EquipmentBorrowing.Domain.Borrowing borrowing)
        {
            Borrowings.Add(borrowing);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(EquipmentBorrowing.Domain.Borrowing borrowing)
        {
            return Task.CompletedTask;
        }
        public Task AddBorrowingWithEquipmentUpdateAsync(
            EquipmentBorrowing.Domain.Borrowing borrowing,
            Equipment equipment)
        {
            Borrowings.Add(borrowing);
            return Task.CompletedTask;
        }
        public Task ReturnBorrowingWithEquipmentUpdateAsync(
            EquipmentBorrowing.Domain.Borrowing borrowing,
            Equipment equipment)
                {
                    borrowing.MarkAsReturned();
                    equipment.MarkAsAvailable();

                    return Task.CompletedTask;
                }
        public Task AddBorrowingsWithEquipmentUpdatesAsync(
            IReadOnlyList<EquipmentBorrowing.Domain.Borrowing> borrowings,
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
        public Task<IReadOnlyList<EquipmentBorrowing.Domain.Borrowing>>
            GetAllByStudentIdAsync(int studentId)
            {
                IReadOnlyList<EquipmentBorrowing.Domain.Borrowing> result =
                    Borrowings
                        .Where(b => b.Student.Id == studentId)
                        .ToList();

                return Task.FromResult(result);
            }
        public Task<IReadOnlyList<EquipmentBorrowing.Domain.Borrowing>>
            GetAllByEquipmentIdAsync(Guid equipmentId)
            {
                IReadOnlyList<EquipmentBorrowing.Domain.Borrowing> result =
                    Borrowings
                        .Where(b => b.Equipment.EquipmentId == equipmentId)
                        .OrderByDescending(b => b.DateBorrowed)
                        .ToList();

                return Task.FromResult(result);
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

            return Task.FromResult<IReadOnlyList<Equipment>>(
                equipmentList);
        }

        public Task<IReadOnlyList<Equipment>> GetAvailableAsync()
        {
            var equipmentList = _equipment is not null && _equipment.IsAvailable
                ? new List<Equipment> { _equipment }
                : new List<Equipment>();

            return Task.FromResult<IReadOnlyList<Equipment>>(
                equipmentList);
        }

        public Task UpdateAsync(Equipment equipment)
        {
            return Task.CompletedTask;
        }

    }
}