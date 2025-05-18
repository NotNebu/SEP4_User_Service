using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP4_User_Service.Domain.Entities;
using SEP4_User_Service.API.DTOs;
using System.Security.Claims;
using SEP4_User_Service.Application.Interfaces;

namespace SEP4_User_Service.API.Controllers;

// Denne controller håndterer brugerrelaterede operationer som ændring af kodeord og profilopdatering.
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IChangePasswordUseCase _changePasswordUseCase;
    private readonly IUserRepository _userRepository;

    // Constructor til at injicere afhængigheder.
    public AccountController(IChangePasswordUseCase changePasswordUseCase, IUserRepository userRepository)
    {
        _changePasswordUseCase = changePasswordUseCase;
        _userRepository = userRepository;
    }

    // Endpoint til at ændre brugerens kodeord.
    // Kræver, at brugeren er autoriseret.
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

    // Endpoint til at hente brugerens profil.
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userRepository.GetUserByIdAsync(Guid.Parse(userId));
        if (user == null)
            return NotFound();

        var location = user.Locations.FirstOrDefault();

        return Ok(new UserProfileDto
        {
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Username = user.Username,
            Email = user.Email,
            Birthday = user.Birthday,
            Country = location?.Country ?? "",
            Street = location?.Street ?? "",
            HouseNumber = location?.HouseNumber ?? "",
            City = location?.City ?? ""
        });
    }

    // Endpoint til at opdatere brugerens profil.
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UserProfileUpdateDto dto)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userRepository.GetUserByIdAsync(Guid.Parse(userId));
        if (user == null)
            return NotFound();

        user.Firstname = dto.Firstname;
        user.Lastname = dto.Lastname;
        user.Username = dto.Username;
        user.Email = dto.Email;
        user.Birthday = dto.Birthday;

        var location = user.Locations.FirstOrDefault();
        if (location == null)
        {
            location = new Location
            {
                Street = dto.Street,
                HouseNumber = dto.HouseNumber,
                City = dto.City,
                Country = dto.Country,
                UserID = user.Id
            };
            user.Locations.Add(location);
        }
        else
        {
            location.Street = dto.Street;
            location.HouseNumber = dto.HouseNumber;
            location.City = dto.City;
            location.Country = dto.Country;
        }

        await _userRepository.UpdateUserAsync(user);
        return Ok(new { message = "Profil opdateret." });
    }
   [HttpPost("upload-profile-image")]
[Authorize]
public async Task<IActionResult> UploadProfileImage([FromForm] ProfileImageUploadDto dto)
{
    var userId = User.FindFirst("UserId")?.Value;
    if (string.IsNullOrEmpty(userId))
        return Unauthorized();

    var file = dto.File;

    if (file == null || file.Length == 0)
        return BadRequest("Ingen fil modtaget.");

    var user = await _userRepository.GetUserByIdAsync(Guid.Parse(userId));
    if (user == null)
        return NotFound("Bruger ikke fundet.");

    // Gem billede i wwwroot/uploads
    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
    Directory.CreateDirectory(uploadsFolder);

    var filename = $"{Guid.NewGuid()}_{file.FileName}";
    var filePath = Path.Combine(uploadsFolder, filename);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    user.ProfileImageUrl = $"/uploads/{filename}";
    await _userRepository.UpdateUserAsync(user);

    return Ok(new { message = "Profilbillede uploadet." });
}
}

