using CodeSample.Application.Services;
using CodeSample.Domain.Entities;
using FluentAssertions;

namespace CodeSample.Tests
{
    public class PersonServiceTests
    {
        [Fact]
        public void CreatePerson_Should_Add_To_List()
        {
            // Arrange
            var service = new PersonService();

            // Act
            var result = service.Create("Ali", "Taheri", "1234567890", new DateTime(1995, 5, 20));

            // Assert
            result.Should().NotBeNull();
            result.FirstName.Should().Be("Ali");
            result.NationalCode.Should().Be("1234567890");
        }

        [Fact]
        public void GetPerson_Should_Return_Same_Object()
        {
            // Arrange
            var service = new PersonService();
            var created = service.Create("Sara", "Moradi", "1111111111", new DateTime(2000, 1, 1));

            // Act
            var fetched = service.Get(created.Id);

            // Assert
            fetched.Should().NotBeNull();
            fetched!.Id.Should().Be(created.Id);
            fetched.FirstName.Should().Be("Sara");
        }

        [Fact]
        public void GetPerson_Should_Return_Null_If_Not_Exists()
        {
            // Arrange
            var service = new PersonService();

            // Act
            var result = service.Get(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }
    }
}
