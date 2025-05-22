using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Application.UseCases;

namespace SEP4_User_Service.Tests.UseCaseTests
{
    public class RegisterUseCaseTests
    {
        [Fact] // Test nåt registration er successfuld
        public async Task ExecuteAsync_IfRegistrationIsSuccessful_ReturnsTrue()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password123";
            var username = "testUser";

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(auth => auth.RegisterAsync(email, password, username)).ReturnsAsync(true);

            var useCase = new RegisterUseCase(mockAuthService.Object);

            // Act
            var result = await useCase.ExecuteAsync(email, password, username);

            // Assert
            Assert.True(result); // Registration skal være successfuld
            mockAuthService.Verify(auth => auth.RegisterAsync(email, password, username), Times.Once);
        }

        [Fact] // Test når registration fejler(brugeren findes allerede)
        public async Task ExecuteAsync_IfRegistrationFails_ReturnsFalse()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password123";
            var username = "testUser";

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(auth => auth.RegisterAsync(email, password, username)).ReturnsAsync(false);

            var useCase = new RegisterUseCase(mockAuthService.Object);

            // Act
            var result = await useCase.ExecuteAsync(email, password, username);

            // Assert
            Assert.False(result); // Registration skal fejle
            mockAuthService.Verify(auth => auth.RegisterAsync(email, password, username), Times.Once);
        }

        [Fact] // Test når registration smider en exception (feks., database issue)
        public async Task ExecuteAsync_IfRegistrationThrowsException_ThrowsException()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password123";
            var username = "testUser";

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(auth => auth.RegisterAsync(email, password, username))
                .ThrowsAsync(new Exception("Database connection failed"));

            var useCase = new RegisterUseCase(mockAuthService.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => useCase.ExecuteAsync(email, password, username));
            Assert.Equal("Database connection failed", exception.Message); // exception message matcher
            mockAuthService.Verify(auth => auth.RegisterAsync(email, password, username), Times.Once);
        }

        [Fact] // Test når email format er invalid
        public async Task ExecuteAsync_IfEmailFormatIsInvalid_ReturnsFalse()
        {
            // Arrange
            var email = "invalid-email"; // Invalid email format
            var password = "password123";
            var username = "testUser";

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(auth => auth.RegisterAsync(email, password, username)).ReturnsAsync(false);

            var useCase = new RegisterUseCase(mockAuthService.Object);

            // Act
            var result = await useCase.ExecuteAsync(email, password, username);

            // Assert
            Assert.False(result); // Registration skal fejler pga invalid email format
            mockAuthService.Verify(auth => auth.RegisterAsync(email, password, username), Times.Once);
        }
    }
}
