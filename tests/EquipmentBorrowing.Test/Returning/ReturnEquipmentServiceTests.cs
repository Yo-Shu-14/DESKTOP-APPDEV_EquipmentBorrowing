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
            borrowingRepository,
            equipmentRepository);

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
            borrowingRepository,
            equipmentRepository);

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
            borrowingRepository,
            equipmentRepository);

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

        public Task UpdateAsync(Equipment equipment)
        {
            return Task.CompletedTask;
        }
    }
}