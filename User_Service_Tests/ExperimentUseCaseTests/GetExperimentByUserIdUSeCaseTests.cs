using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Application.UseCases;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Tests.UseCaseTests
{
    public class GetExperimentByIdUseCaseTests
    {
        [Fact] // Test når eksperimentet eksistere 
        public async Task ExecuteAsync_IfExperimentExists_ReturnsExperiment()
        {
            // Arrange
            var experimentId = 1;
            var expectedExperiment = new Experiment
            {
                Id = experimentId,
                Title = "Test Experiment",  
                Description = "This is a test experiment.",
                CreatedAt = DateTime.UtcNow 
            };

            var mockRepo = new Mock<IExperimentRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(experimentId)).ReturnsAsync(expectedExperiment);

            var useCase = new GetExperimentByIdUseCase(mockRepo.Object);

            // Act
            var result = await useCase.ExecuteAsync(experimentId);

            // Assert
            Assert.NotNull(result); // Eksperimentet skal eksistere 
            Assert.Equal(expectedExperiment.Id, result?.Id); // ID er korrekt
            Assert.Equal(expectedExperiment.Title, result?.Title); // Titel er korrekt 
            Assert.Equal(expectedExperiment.Description, result?.Description); //Beskrivelse er korrekt
            Assert.Equal(expectedExperiment.CreatedAt, result?.CreatedAt); // CreatedAt er korret 

            // Verificere at repository's GetByIdAsync method var kaldt præcis en gang 
            mockRepo.Verify(r => r.GetByIdAsync(experimentId), Times.Once);
        }

        [Fact] // Test når eksperimentet ikke eksistere 
        public async Task ExecuteAsync_IfExperimentDoesNotExist_ReturnsNull()
        {
            // Arrange
            var experimentId = 1;

            var mockRepo = new Mock<IExperimentRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(experimentId)).ReturnsAsync((Experiment?)null); // Simulere at intet eksperiment er fundet 

            var useCase = new GetExperimentByIdUseCase(mockRepo.Object);

            // Act
            var result = await useCase.ExecuteAsync(experimentId);

            // Assert
            Assert.Null(result); // Eksperimentet eksistere ikke, så resultatet skal være null 
            mockRepo.Verify(r => r.GetByIdAsync(experimentId), Times.Once); // Verificere at GetByIdAsync var kaldt en gang
        }
    }
}
