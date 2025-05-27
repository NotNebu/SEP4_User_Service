using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SEP4_User_Service.API.DTOs;
using SEP4_User_Service.Application.Interfaces;

namespace SEP4_User_Service.API.Controllers;

// Denne controller håndterer autentificeringsrelaterede operationer som login, registrering og logout.
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILoginUseCase _loginUseCase;
    private readonly IRegisterUseCase _registerUseCase;
    private readonly IGetUserByTokenUseCase _getUserByTokenUseCase;

    // Constructor til at injicere afhængigheder.
    public AuthController(
        ILoginUseCase loginUseCase,
        IRegisterUseCase registerUseCase,
        IGetUserByTokenUseCase getUserByTokenUseCase
    )
    {
        _loginUseCase = loginUseCase;
        _registerUseCase = registerUseCase;
        _getUserByTokenUseCase = getUserByTokenUseCase;
    }

    // Endpoint til login.
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var token = await _loginUseCase.ExecuteAsync(request.Email, request.Password);

            Response.Cookies.Append(
                "jwt",
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // vigtigt for HTTPS
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddHours(1),
                }
            );

            return Ok(new { Message = "Login successful" });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Ugyldige loginoplysninger.");
        }
    }

    // Endpoint til registrering.
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var success = await _registerUseCase.ExecuteAsync(
            request.Email,
            request.Password,
            request.Username
        );

        if (!success)
            return Conflict("Bruger med denne email findes allerede.");

        return Ok(new { Success = true });
    }

    // Endpoint til at hente brugeroplysninger.
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetUser()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var userId = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userId))
            return Unauthorized("Claims mangler i token.");

        return Ok(new { Email = email, UserId = userId });
    }

    // Endpoint til logout.
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Append(
            "jwt",
            "",
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            }
        );

        return Ok(new { Message = "Logout successful" });
    }
}
