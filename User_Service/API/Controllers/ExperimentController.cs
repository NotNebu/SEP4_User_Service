using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP4_User_Service.Application.UseCases;
using SEP4_User_Service.Domain.Entities;
using SEP4_User_Service.API.DTOs.Experiment;
using SEP4_User_Service.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Text.Json;


namespace SEP4_User_Service.API.Controllers;

// Denne controller håndterer CRUD-operationer for eksperimenter.
[ApiController]
[Route("api/[controller]")]
public class ExperimentController : ControllerBase
{
    private readonly CreateExperimentUseCase _createUseCase;
    private readonly UpdateExperimentUseCase _updateUseCase;
    private readonly DeleteExperimentUseCase _deleteUseCase;
    private readonly GetExperimentByIdUseCase _getByIdUseCase;
    private readonly IExperimentRepository _experimentRepository;

    // Constructor til at injicere afhængigheder.
    public ExperimentController(
        CreateExperimentUseCase createUseCase,
        UpdateExperimentUseCase updateUseCase,
        DeleteExperimentUseCase deleteUseCase,
        GetExperimentByIdUseCase getByIdUseCase,
        IExperimentRepository experimentRepository)
    {
        _createUseCase = createUseCase;
        _updateUseCase = updateUseCase;
        _deleteUseCase = deleteUseCase;
        _getByIdUseCase = getByIdUseCase;
        _experimentRepository = experimentRepository;
    }

    // Endpoint til at oprette et nyt eksperiment.
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ExperimentRequestDto dto)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var experiment = new Experiment
        {
            Title = dto.Title,
            Description = dto.Description,
            DataJson = JsonSerializer.Serialize(dto.DataJson),
            UserId = Guid.Parse(userId)
        };

        await _createUseCase.ExecuteAsync(experiment);

        return Ok(new { message = "Eksperiment oprettet." });
    }

    // Endpoint til at hente et eksperiment baseret på dets ID.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var experiment = await _getByIdUseCase.ExecuteAsync(id);
        if (experiment == null)
            return NotFound();

        return Ok(new ExperimentDto
        {
            Id = experiment.Id,
            Title = experiment.Title,
            Description = experiment.Description,
            DataJson = experiment.DataJson,
            CreatedAt = experiment.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        });
    }

    // Endpoint til at hente alle eksperimenter for den autoriserede bruger.
    [HttpGet("my-experiments")]
    [Authorize]
    public async Task<IActionResult> GetByUser()
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var experiments = await _experimentRepository.GetByUserIdAsync(Guid.Parse(userId));

        var result = experiments.Select(e => new ExperimentDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            DataJson = e.DataJson,
            CreatedAt = e.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        });

        return Ok(result);
    }

    // Endpoint til at opdatere et eksperiment.
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] ExperimentRequestDto dto)
    {
        var experiment = await _getByIdUseCase.ExecuteAsync(id);
        if (experiment == null)
            return NotFound();

        var userId = User.FindFirst("UserId")?.Value;
        if (experiment.UserId.ToString() != userId)
            return Forbid();

        experiment.Title = dto.Title;
        experiment.Description = dto.Description;
        experiment.DataJson = JsonSerializer.Serialize(dto.DataJson);


        await _updateUseCase.ExecuteAsync(experiment);
        return Ok(new { message = "Eksperiment opdateret." });
    }

    // Endpoint til at slette et eksperiment.
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var experiment = await _getByIdUseCase.ExecuteAsync(id);
        if (experiment == null)
            return NotFound();

        var userId = User.FindFirst("UserId")?.Value;
        if (experiment.UserId.ToString() != userId)
            return Forbid();

        await _deleteUseCase.ExecuteAsync(id);
        return Ok(new { message = "Eksperiment slettet." });
    }
}
