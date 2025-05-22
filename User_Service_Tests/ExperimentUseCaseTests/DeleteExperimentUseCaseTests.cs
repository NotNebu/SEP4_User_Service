using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Application.UseCases;

namespace SEP4_User_Service.Tests.UseCaseTests
{
    public class DeleteExperimentUseCaseTests
    {
        [Fact] // Test når eksperimentet er slettet ordentligt 
        public async Task ExecuteAsync_IfExperimentExists_ReturnsTrue()
        {
            // Arrange
            var experimentId = 1; // Experiment ID til at slette 

            var mockRepo = new Mock<IExperimentRepository>();
            mockRepo.Setup(r => r.DeleteAsync(experimentId)).ReturnsAsync(true); // Mock DeleteAsync til retunere true

            var useCase = new DeleteExperimentUseCase(mockRepo.Object);

            // Act
            var result = await useCase.ExecuteAsync(experimentId);

            // Assert
            Assert.True(result); // Assert at sletningen var succesfuld
            mockRepo.Verify(r => r.DeleteAsync(experimentId), Times.Once); // Verificere at DeleteAsync var kaldt en gang
        }

        [Fact] // Test når eksperimentet ikke eksistere eller sletning fejler 
        public async Task ExecuteAsync_IfExperimentDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var experimentId = 1; // Experiment ID at slette 

            var mockRepo = new Mock<IExperimentRepository>();
            mockRepo.Setup(r => r.DeleteAsync(experimentId)).ReturnsAsync(false); // Mock DeleteAsync til at retunere false

            var useCase = new DeleteExperimentUseCase(mockRepo.Object);

            // Act
            var result = await useCase.ExecuteAsync(experimentId);

            // Assert
            Assert.False(result); // Assert at sletningen fejler
            mockRepo.Verify(r => r.DeleteAsync(experimentId), Times.Once); // Verificere at DeleteAsync var kaldt en gang
        }
    }
}
