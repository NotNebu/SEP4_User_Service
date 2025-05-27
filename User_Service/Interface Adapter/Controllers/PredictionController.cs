using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP4_User_Service.API.DTOs;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PredictionController : ControllerBase
{
    private readonly IPredictionRepository _repository;

    public PredictionController(IPredictionRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> SavePrediction([FromBody] PredictionRequestDto dto)
    {
        var userId = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID mangler i token");

        var prediction = new Prediction
        {
            UserId = Guid.Parse(userId),
            Model = dto.Model,
            FileName = dto.FileName,
            Input = JsonSerializer.Serialize(dto.Input),
            Result = JsonSerializer.Serialize(dto.Result),
            Timestamp = DateTime.UtcNow,
        };

        await _repository.AddAsync(prediction);

        return Ok(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetUserPredictions()
    {
        var userId = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID mangler i token");

        var predictions = await _repository.GetByUserIdAsync(Guid.Parse(userId));
        return Ok(predictions);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePrediction(int id)
    {
        var userId = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID mangler i token");

        var prediction = await _repository.GetByIdAsync(id);

        if (prediction == null || prediction.UserId != Guid.Parse(userId))
            return NotFound("Forudsigelse ikke fundet eller ikke tilladt.");

        await _repository.DeleteAsync(prediction);
        return NoContent();
    }
}
