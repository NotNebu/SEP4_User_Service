using Microsoft.AspNetCore.Mvc;
using SEP4_User_Service.API.DTOs;
using SEP4_User_Service.Application.Interfaces;

namespace SEP4_User_Service.API.Controllers;

// Controller til autentificeringsrelaterede operationer
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILoginUseCase _loginUseCase;
    private readonly IRegisterUseCase _registerUseCase;
    private readonly IGetUserByTokenUseCase _getUserByTokenUseCase;

    // Initialiserer controlleren med nødvendige use cases
    public AuthController(
        ILoginUseCase loginUseCase,
        IRegisterUseCase registerUseCase,
        IGetUserByTokenUseCase getUserByTokenUseCase)
    {
        _loginUseCase = loginUseCase;
        _registerUseCase = registerUseCase;
        _getUserByTokenUseCase = getUserByTokenUseCase;
    }

    // Endpoint til login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var token = await _loginUseCase.ExecuteAsync(request.Email, request.Password);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return Ok(new { Message = "Login successful" });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Ugyldige loginoplysninger.");
        }
    }

    // Endpoint til registrering
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

    // Endpoint til at hente brugeroplysninger
    [HttpGet("me")]
    public async Task<IActionResult> GetUser()
    {
        var token = Request.Cookies["jwt"];

        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized("Token mangler.");

        var user = await _getUserByTokenUseCase.ExecuteAsync(token);
        if (user == null)
            return Unauthorized("Ugyldigt token.");

        return Ok(new { user.Email, user.Username });
    }

    // Endpoint til logout
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Append("jwt", "", new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(-1),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return Ok(new { Message = "Logout successful" });
    }
}