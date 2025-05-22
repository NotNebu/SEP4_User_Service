using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Application.UseCases;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Tests.UseCaseTests;

public class GetUserUseCaseTests
{
    [Fact] // Test af metoden: ExecuteByIdAsync
    public async Task ExecuteByIdAsync_ReturnsUserWithCorrectId()
    {
        // Arrange (forbered testdata og mocks)
        var userId = Guid.NewGuid(); // Unik ID vi vil søge efter

        var expectedUser = new User(
        username: "testUser",
        password: "password",
        email: "test@example.com",
        firstname: "Test",
        lastname: "Tester",
        birthday: "1991-01-01"
    );
        expectedUser.Id = userId;

        // Vi mocker repository og siger: "Når du kalder GetUserByIdAsync med dette ID, så returner denne bruger"
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(expectedUser);

        // Brug mocken til at oprette usecase
        var useCase = new GetUserUseCase(mockRepo.Object);

        // Act (udfør testen)
        var result = await useCase.ExecuteByIdAsync(userId);

      // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result!.Id);
        Assert.Equal(expectedUser.Username, result.Username);
        mockRepo.Verify(r => r.GetUserByIdAsync(userId), Times.Once);

        // Verificér at metoden GetUserByIdAsync blev kaldt én gang med præcis det ID
        // Dette fanger hvis metoden aldrig blev kaldt, eller blev kaldt flere gange
        mockRepo.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
    }

    [Fact] // Test af metoden: ExecuteByIdsAsync
    public async Task ExecuteByIdsAsync_ReturnsAllMatchingUsers()
    {
        // Arrange
        var user1 = new User("user1", "pass1", "user1@example.com", "First1", "Last1", "1991-01-01");
        var user2 = new User("user2", "pass2", "user2@example.com", "First2", "Last2", "1992-02-02");

        var ids = new List<Guid> { user1.Id, user2.Id }; // Vi tester at disse 2 brugere kan hentes

        var expectedUsers = new List<User> { user1, user2 };

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetUsersByIdsAsync(ids)).ReturnsAsync(expectedUsers);

        var useCase = new GetUserUseCase(mockRepo.Object);

        // Act
        var result = await useCase.ExecuteByIdsAsync(ids);

          // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, u => u.Id == user1.Id);
        Assert.Contains(result, u => u.Id == user2.Id);
        mockRepo.Verify(r => r.GetUsersByIdsAsync(ids), Times.Once);
    }
}
