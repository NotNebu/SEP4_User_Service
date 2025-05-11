using Grpc.Core;
using Protos.Grpc.Experiment;
using SEP4_User_Service.Application.UseCases;
using SEP4_User_Service.Domain.Entities;
using SEP4_User_Service.Application.Interfaces;
using Google.Protobuf.WellKnownTypes;

namespace SEP4_User_Service.API.Services;

// Denne klasse implementerer gRPC-tjenesten for eksperimenter.
// Den bruger forskellige UseCases til at udføre CRUD-operationer på eksperimenter.
public class GrpcExperimentService : ExperimentService.ExperimentServiceBase
{
    private readonly CreateExperimentUseCase _create;
    private readonly GetExperimentByIdUseCase _getById;
    private readonly DeleteExperimentUseCase _delete;
    private readonly UpdateExperimentUseCase _update;
    private readonly IExperimentRepository _repository;

    // Constructoren initialiserer afhængighederne, som bruges til at udføre operationer.
    public GrpcExperimentService(
        CreateExperimentUseCase create,
        GetExperimentByIdUseCase getById,
        DeleteExperimentUseCase delete,
        UpdateExperimentUseCase update,
        IExperimentRepository repository)
    {
        _create = create;
        _getById = getById;
        _delete = delete;
        _update = update;
        _repository = repository;
    }

    // Henter et eksperiment baseret på dets ID.
    // Returnerer en gRPC-besked, hvis eksperimentet findes, ellers kastes en NotFound-fejl.
    public override async Task<ExperimentMessage> GetById(GetExperimentByIdRequest request, ServerCallContext context)
    {
        var experiment = await _getById.ExecuteAsync(request.Id);
        if (experiment == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Eksperiment ikke fundet"));

        return MapToMessage(experiment);
    }

    // Henter alle eksperimenter for en given bruger baseret på brugerens ID.
    // Returnerer en liste af eksperimenter som gRPC-beskeder.
    public override async Task<ExperimentListResponse> GetByUserId(GetExperimentsByUserIdRequest request, ServerCallContext context)
    {
        var userId = Guid.Parse(request.UserId);
        var experiments = await _repository.GetByUserIdAsync(userId);

        var response = new ExperimentListResponse();
        response.Experiments.AddRange(experiments.Select(MapToMessage));
        return response;
    }

    // Opretter et nyt eksperiment baseret på data fra gRPC-requesten.
    // Returnerer en gRPC-besked med det oprettede eksperiment.
    public override async Task<ExperimentMessage> Create(CreateExperimentRequest request, ServerCallContext context)
    {
        var experiment = new Experiment
        {
            Title = request.Title,
            Description = request.Description,
            DataJson = request.DataJson,
            UserId = Guid.Parse(request.UserId)
        };

        await _create.ExecuteAsync(experiment);
        return MapToMessage(experiment);
    }

    // Opdaterer et eksisterende eksperiment baseret på data fra gRPC-requesten.
    // Returnerer en tom gRPC-besked, hvis opdateringen lykkes.
    public override async Task<Empty> Update(UpdateExperimentRequest request, ServerCallContext context)
    {
        var experiment = await _getById.ExecuteAsync(request.Id);
        if (experiment == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Eksperiment ikke fundet"));

        experiment.Title = request.Title;
        experiment.Description = request.Description;
        experiment.DataJson = request.DataJson;

        await _update.ExecuteAsync(experiment);
        return new Empty();
    }

    // Sletter et eksperiment baseret på dets ID.
    // Returnerer en gRPC-besked med en bool, der angiver, om sletningen lykkedes.
    public override async Task<DeleteExperimentResponse> Delete(DeleteExperimentRequest request, ServerCallContext context)
    {
        var success = await _delete.ExecuteAsync(request.Id);
        return new DeleteExperimentResponse { Success = success };
    }

    // Mapper en Experiment-entitet til en gRPC-besked.
    private ExperimentMessage MapToMessage(Experiment experiment)
    {
        return new ExperimentMessage
        {
            Id = experiment.Id,
            Title = experiment.Title,
            Description = experiment.Description ?? "",
            DataJson = experiment.DataJson ?? "",
            CreatedAt = experiment.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            UserId = experiment.UserId.ToString()
        };
    }
}
