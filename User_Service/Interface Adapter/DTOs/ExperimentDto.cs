// Denne klasse repræsenterer dataoverførselsobjektet (DTO) for et eksperiment.
// DTO bruges til at sende data mellem API'et og klienten.
public class ExperimentDto
{
    // Eksperimentets unikke ID.
    public int Id { get; set; }

    // Titel på eksperimentet.
    public string Title { get; set; } = string.Empty;

    // Beskrivelse af eksperimentet (valgfri).
    public string? Description { get; set; }

    // JSON-data relateret til eksperimentet (valgfri).
    public object DataJson { get; set; }

    // Tidsstempel for, hvornår eksperimentet blev oprettet.
    public string CreatedAt { get; set; } = string.Empty;
}
