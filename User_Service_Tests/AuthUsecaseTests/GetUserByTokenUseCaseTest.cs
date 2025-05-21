using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Application.UseCases;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Tests.UseCaseTests
{
    public class GetUserByTokenUseCaseTests
    {
        [Fact] // Test når token er valid og brugeren er fundet 
        public async Task ExecuteAsync_IfTokenIsValid_ReturnsUser()
        {
            // Arrange
            var token = "valid.jwt.token";
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testUser",
                Email = "test@example.com",
                Firstname = "Test",
                Lastname = "User",
                Birthday = "1990-01-01"
            };

            // Mock AuthService til at retunere en user når token er valid
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(auth => auth.GetUserByTokenAsync(token)).ReturnsAsync(user);

            var useCase = new GetUserByTokenUseCase(mockAuthService.Object);

            // Act
            var result = await useCase.ExecuteAsync(token);

            // Assert
            Assert.NotNull(result); // Brugeren skal være fundet
            Assert.Equal(user.Id, result?.Id); // ID skal match
            Assert.Equal(user.Username, result?.Username); // Username skal match
            Assert.Equal(user.Email, result?.Email); // Email skal match
            Assert.Equal(user.Firstname, result?.Firstname); // Firstname skal match
            Assert.Equal(user.Lastname, result?.Lastname); // Lastname skal match

            // Verificere at GetUserByTokenAsync var kaldt en gang
            mockAuthService.Verify(auth => auth.GetUserByTokenAsync(token), Times.Once);
        }

        [Fact] // Test når token er invalid og brugeren ikke er fundet
        public async Task ExecuteAsync_IfTokenIsInvalid_ReturnsNull()
        {
            // Arrange
            var token = "invalid.jwt.token";

            // Mock AuthService til at retunere null når token er invalid 
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(auth => auth.GetUserByTokenAsync(token)).ReturnsAsync((User?)null);

            var useCase = new GetUserByTokenUseCase(mockAuthService.Object);

            // Act
            var result = await useCase.ExecuteAsync(token);

            // Assert
            Assert.Null(result); // Brugeren skal ikke være fundet
            mockAuthService.Verify(auth => auth.GetUserByTokenAsync(token), Times.Once);
        }

        // Test når token fører til en exception
        [Fact] // Test når token er udløbet elelr invalid og smider en exception
        public async Task ExecuteAsync_IfTokenIsInvalid_ThrowsException()
        {
            // Arrange
            var token = "expired.jwt.token";
            
            // Mock AuthService til at smide en exception for en invalid token
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(auth => auth.GetUserByTokenAsync(token))
                           .ThrowsAsync(new UnauthorizedAccessException("Token is expired"));

            var useCase = new GetUserByTokenUseCase(mockAuthService.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecuteAsync(token));
            Assert.Equal("Token is expired", exception.Message); // Tjek exception besked

            mockAuthService.Verify(auth => auth.GetUserByTokenAsync(token), Times.Once);
        }
    }
}
