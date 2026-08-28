using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Infrastructure;
using Xunit;
using DomainEquipment = EquipmentBorrowing.Domain.Equipment;

namespace EquipmentBorrowing.Test.Equipment
{
    public class CheckAvailableEquipmentServiceTests
    {
        [Fact]
        public async Task Return_Available_Equipment()
        {
            
            var equipment = new DomainEquipment(
                Guid.NewGuid(),
                "Laptop",
                "Laboratory Laptop",
                true
            );

            var repository = new InMemoryEquipmentRepository(
                new[] { equipment }
            );

            var service = new CheckAvailableEquipmentService(repository);

            
            var result = await service.CheckAvailableEquipmentAsync();

            
            Assert.Single(result);
            Assert.Equal("Laptop", result[0].Name);
        }


        [Fact]
        public async Task Return_Empty_When_No_Equipment_Is_Available()
        {
            var laptop = new DomainEquipment(
                Guid.NewGuid(),
                "Laptop",
                "Laboratory Laptop",
                false
            );

            var projector = new DomainEquipment(
                Guid.NewGuid(),
                "Projector",
                "Laboratory Projector",
                false
            );

            var repository = new InMemoryEquipmentRepository(
                new[] { laptop, projector }
            );

            var service = new CheckAvailableEquipmentService(repository);

            var result = await service.CheckAvailableEquipmentAsync();

            Assert.Empty(result);
        }
    }
}