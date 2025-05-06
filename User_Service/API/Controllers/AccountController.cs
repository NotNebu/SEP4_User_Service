using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP4_User_Service.Application.UseCases;
using SEP4_User_Service.API.DTOs;
using System.Security.Claims;
using SEP4_User_Service.Application.Interfaces;

namespace SEP4_User_Service.API.Controllers;

// Angiver, at denne klasse er en API-controller og definerer dens routing.
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    // Dependency Injection: Interface til håndtering af ændring af kodeord.
    private readonly IChangePasswordUseCase _changePasswordUseCase;

    // Constructor til at injicere afhængigheder.
    public AccountController(IChangePasswordUseCase changePasswordUseCase)
    {
        _changePasswordUseCase = changePasswordUseCase;
    }

    // Endpoint til at ændre brugerens kodeord.
    // Kræver, at brugeren er autoriseret.
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        // Henter brugerens ID fra claims i den autoriserede bruger.
        var userId = User.FindFirst("UserId")?.Value;

        // Hvis bruger-ID'et ikke findes, returneres en Unauthorized-respons.
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Ugyldig bruger.");

        // Kalder use case for at ændre kodeordet og sender nødvendige parametre.
        var success = await _changePasswordUseCase.ExecuteAsync(
            Guid.Parse(userId), // Konverterer bruger-ID til en Guid.
            request.OldPassword, // Det gamle kodeord.
            request.NewPassword  // Det nye kodeord.
        );

        // Hvis ændringen ikke lykkes, returneres en BadRequest-respons med en fejlmeddelelse.
        if (!success)
            return BadRequest(new { message = "Gammelt kodeord er forkert eller bruger ikke fundet." });

        // Hvis ændringen lykkes, returneres en OK-respons med en succesmeddelelse.
        return Ok(new { message = "Kodeord opdateret." });
    }
}

