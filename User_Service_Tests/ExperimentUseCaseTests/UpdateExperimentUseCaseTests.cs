using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Application.UseCases;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Tests.UseCaseTests
{
    public class UpdateExperimentUseCaseTests
    {
        [Fact] // Test når eksperiment er opdater succesfuldt 
        public async Task ExecuteAsync_UpdatesExperimentSuccessfully()
        {
            // Arrange
            var experiment = new Experiment
            {
                Id = 1,
                Title = "Old Experiment Title",  
                Description = "Old experiment description", 
                CreatedAt = DateTime.UtcNow 
            };

            // Nyt data for experimentet 
            var updatedExperiment = new Experiment
            {
                Id = experiment.Id,
                Title = "Updated Experiment Title",  
                Description = "Updated experiment description",  
                CreatedAt = experiment.CreatedAt 
            };

            var mockRepo = new Mock<IExperimentRepository>();
            mockRepo.Setup(r => r.UpdateAsync(updatedExperiment)).Returns(Task.CompletedTask);

            var useCase = new UpdateExperimentUseCase(mockRepo.Object);

            // Act
            await useCase.ExecuteAsync(updatedExperiment);

            // Assert
            // Verificere at UpdateAsync var kaldt med det korrekte opdatere eksperiment
            mockRepo.Verify(r => r.UpdateAsync(It.Is<Experiment>(e =>
                e.Title == updatedExperiment.Title &&
                e.Description == updatedExperiment.Description &&
                e.CreatedAt == updatedExperiment.CreatedAt // Validere at CreatedAt er uændret
            )), Times.Once);
        }
    }
}
