using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Infrastructure;
using Xunit;
using DomainEquipment = EquipmentBorrowing.Domain.Equipment;

namespace EquipmentBorrowing.Test.Equipment
{
    public class CheckAvailableEquipmentServiceTests
    {
        [Fact]
        public async Task Should_Return_Available_Equipment()
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
    }
}