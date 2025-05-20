using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Application.UseCases;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Tests.UseCaseTests
{
    public class LoginUseCaseTests
    {
        [Fact] // Test når login er successfuld og en JWT token er retuneret 
        public async Task ExecuteAsync_IfLoginIsSuccessful_ReturnsToken()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password123";
            var expectedToken = "valid.jwt.token"; // mock valid JWT token string

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(auth => auth.LoginAsync(email, password)).ReturnsAsync(expectedToken);

            var useCase = new LoginUseCase(mockAuthService.Object);

            // Act
            var result = await useCase.ExecuteAsync(email, password);

            // Assert
            Assert.Equal(expectedToken, result); // Token skal match den forventet token
            mockAuthService.Verify(auth => auth.LoginAsync(email, password), Times.Once);
        }

        [Fact] // Test når login fejler og en UnauthorizedAccessException bruges 
        public async Task ExecuteAsync_IfLoginFails_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var email = "test@example.com";
            var password = "wrongpassword";

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(auth => auth.LoginAsync(email, password))
                           .ThrowsAsync(new UnauthorizedAccessException("Invalid login credentials"));

            var useCase = new LoginUseCase(mockAuthService.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecuteAsync(email, password));
            Assert.Equal("Invalid login credentials", exception.Message); // Error message skal match
            mockAuthService.Verify(auth => auth.LoginAsync(email, password), Times.Once);
        }

        [Fact] // Test når login fejler og en null eller tom token er retuneret 
        public async Task ExecuteAsync_IfLoginFails_ReturnsNullToken()
        {
            // Arrange
            var email = "test@example.com";
            var password = "incorrectpassword";
            string? expectedToken = null;

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(auth => auth.LoginAsync(email, password)).ReturnsAsync(expectedToken);

            var useCase = new LoginUseCase(mockAuthService.Object);

            // Act
            var result = await useCase.ExecuteAsync(email, password);

            // Assert
            Assert.Null(result); // Null token retuneres når login fejler 
            mockAuthService.Verify(auth => auth.LoginAsync(email, password), Times.Once);
        }
    }
}
