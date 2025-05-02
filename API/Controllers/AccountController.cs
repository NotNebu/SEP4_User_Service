using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP4_User_Service.Application.UseCases;
using SEP4_User_Service.API.DTOs;
using System.Security.Claims;
using SEP4_User_Service.Application.Interfaces;
namespace SEP4_User_Service.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IChangePasswordUseCase _changePasswordUseCase;

    public AccountController(IChangePasswordUseCase changePasswordUseCase)
    {
        _changePasswordUseCase = changePasswordUseCase;
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var userId = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Ugyldig bruger.");

        var success = await _changePasswordUseCase.ExecuteAsync(
            Guid.Parse(userId),
            request.OldPassword,
            request.NewPassword
        );

        if (!success)
            return BadRequest(new { message = "Gammelt kodeord er forkert eller bruger ikke fundet." });

        return Ok(new { message = "Kodeord opdateret." });
    }
}

