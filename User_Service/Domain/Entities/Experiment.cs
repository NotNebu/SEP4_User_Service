namespace SEP4_User_Service.Domain.Entities;

public class Experiment
{
    public int Id { get; set; } // Eksperimentets unikke ID
    public string Title { get; set; } = string.Empty; // Titel på eksperimentet
    public string? Description { get; set; } // Beskrivelse af eksperimentet
    public string? DataJson { get; set; } // JSON-repræsentation af eksperimentdata
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Oprettelsesdato og -tidspunkt

    // Relationsfelt til User
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
