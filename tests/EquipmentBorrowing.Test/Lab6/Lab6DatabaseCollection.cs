using Xunit;

namespace EquipmentBorrowing.Tests.Lab6;

[CollectionDefinition("Lab 6 PostgreSQL database", DisableParallelization = true)]
public sealed class Lab6DatabaseCollection : ICollectionFixture<Lab6DatabaseFixture>
{
}
