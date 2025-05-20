using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Application.UseCases;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Tests.UseCaseTests
{
    public class CreateExperimentUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_CallsRepositoryToCreateExperiment()
        {
            // Arrange
            var experiment = new Experiment
            {
                Id = 1, // Sætter Id til en integer
                Title = "Test Experiment", // Bruger Title property
                Description = "This is a test experiment.",
                CreatedAt = DateTime.Now, // Bruger CreatedAt 
                UserId = Guid.NewGuid() // en valid UserId af typen Guid
            };

            var mockRepo = new Mock<IExperimentRepository>();
            mockRepo.Setup(repo => repo.CreateAsync(experiment)).Returns(Task.CompletedTask);

            var useCase = new CreateExperimentUseCase(mockRepo.Object);

            // Act
            await useCase.ExecuteAsync(experiment);

            // Assert
            mockRepo.Verify(repo => repo.CreateAsync(experiment), Times.Once);
        }
    }
}
