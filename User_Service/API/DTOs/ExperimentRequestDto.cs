// Denne klasse repræsenterer dataoverførselsobjektet (DTO) for en eksperimentanmodning.
// DTO bruges til at modtage data fra klienten, når der oprettes eller opdateres et eksperiment.
namespace SEP4_User_Service.API.DTOs.Experiment;

public class ExperimentRequestDto
{
    // Titel på eksperimentet.
    public string Title { get; set; } = string.Empty;

    // Beskrivelse af eksperimentet (valgfri).
    public string? Description { get; set; }

    // JSON-data relateret til eksperimentet (valgfri).
    public object DataJson { get; set; }
}
