namespace Domain.Entities;

/// <summary>
/// Repræsenterer en bruger i systemet, herunder identitet og legitimationsoplysninger.
/// </summary>
public class User
{
    /// <summary>
    /// Unik identifikator for brugeren.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Brugerens emailadresse, som også anvendes til login.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Brugerens synlige brugernavn.
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// Brugerens adgangskode i hashed form (ikke rå tekst).
    /// </summary>
    public string Password { get; set; } = null!;
}
