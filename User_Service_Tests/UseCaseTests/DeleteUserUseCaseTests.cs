using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Application.UseCases;

namespace SEP4_User_Service.Tests.UseCaseTests;

// Tester DeleteUserUseCase-klassen
public class DeleteUserUseCaseTests
{
    [Fact] // Test når bruger slettes korrekt
    public async Task ExecuteAsync_IfUserExists_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.DeleteUserAsync(userId)).ReturnsAsync(true);

        var useCase = new DeleteUserUseCase(mockRepo.Object);

        // Act
        var result = await useCase.ExecuteAsync(userId);

        // Assert
        Assert.True(result); // Brugeren blev slettet
        mockRepo.Verify(r => r.DeleteUserAsync(userId), Times.Once);
    }

    [Fact] // Test når bruger ikke findes / sletning fejler
    public async Task ExecuteAsync_IfUserDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.DeleteUserAsync(userId)).ReturnsAsync(false);

        var useCase = new DeleteUserUseCase(mockRepo.Object);

        // Act
        var result = await useCase.ExecuteAsync(userId);

        // Assert
        Assert.False(result); // Brugeren blev ikke slettet
        mockRepo.Verify(r => r.DeleteUserAsync(userId), Times.Once);
    }
}
