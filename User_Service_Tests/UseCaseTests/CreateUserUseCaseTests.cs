using Xunit;
using Moq;
using System.Threading.Tasks;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Application.UseCases;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Tests.UseCaseTests;

public class CreateUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_CallsRepositoryAndReturnsSameUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testUser",
            Password = "password",
            Email = "test@example.com",
            Firstname = "Test",
            Lastname = "Tester",
            Birthday = "1991-01-01"
        };

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(repo => repo.CreateUserAsync(user)).Returns(Task.CompletedTask);

        var useCase = new CreateUserUseCase(mockRepo.Object);

        // Act
        var result = await useCase.ExecuteAsync(user);

        // Assert
        mockRepo.Verify(repo => repo.CreateUserAsync(user), Times.Once);
        Assert.Equal(user, result);
    }
}
