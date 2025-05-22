using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Application.UseCases;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Tests.UseCaseTests;

// Tester UpdateUserUseCase-klassen
public class UpdateUserUseCaseTests
{
    [Fact] // Test at metoden kalder UpdateUserAsync korrekt
    public async Task ExecuteAsync_CallsRepositoryWithCorrectUser()
    {
        // Arrange: Opret en testbruger
        var user = new User(
            username: "updateUser",
            password: "newpassword",
            email: "update@example.com",
            firstname: "Update",
            lastname: "Tester",
            birthday: "1995-05-05"
        );

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.UpdateUserAsync(user)).Returns(Task.CompletedTask);

        var useCase = new UpdateUserUseCase(mockRepo.Object);

        // Act: Kald usecasen med brugeren
        await useCase.ExecuteAsync(user);

        // Assert: Verificér at repository-metoden blev kaldt én gang med korrekt data
        mockRepo.Verify(r => r.UpdateUserAsync(user), Times.Once);
    }
}
