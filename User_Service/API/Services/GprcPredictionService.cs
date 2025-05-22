using Grpc.Core;
using Protos.Grpc;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;
using Google.Protobuf.WellKnownTypes;

namespace SEP4_User_Service.API.Services;

public class GrpcPredictionService : PredictionService.PredictionServiceBase
{
    private readonly IPredictionRepository _repository;

    public GrpcPredictionService(IPredictionRepository repository)
    {
        _repository = repository;
    }

    public override async Task<PredictionMessage> CreatePrediction(CreatePredictionRequest request, ServerCallContext context)
    {
        
        var prediction = new Prediction
        {
            UserId = Guid.Parse(request.UserId),
            Model = request.ModelType,
            FileName = request.ModelFile,
            Input = request.DataNumeric != null
                ? System.Text.Json.JsonSerializer.Serialize(request.DataNumeric)
                : System.Text.Json.JsonSerializer.Serialize(request.DataCategorical),
            Result = "pending" 
        };

        await _repository.AddAsync(prediction);

        return new PredictionMessage
        {
            Id = prediction.Id.ToString(),
            UserId = prediction.UserId.ToString(),
            ModelType = prediction.Model,
            ModelFile = prediction.FileName,
            InputJson = prediction.Input,
            Timestamp = prediction.Timestamp.ToString("o")
        };
    }

    public override async Task<PredictionListResponse> GetPredictionsByUser(GetPredictionsByUserRequest request, ServerCallContext context)
    {
        var userId = Guid.Parse(request.UserId);
        var predictions = await _repository.GetByUserIdAsync(userId);

        var response = new PredictionListResponse();
        response.Predictions.AddRange(predictions.Select(p => new PredictionMessage
        {
            Id = p.Id.ToString(),
            UserId = p.UserId.ToString(),
            ModelType = p.Model,
            ModelFile = p.FileName,
            InputJson = p.Input,
            Timestamp = p.Timestamp.ToString("o")
        }));

        return response;
    }
}
