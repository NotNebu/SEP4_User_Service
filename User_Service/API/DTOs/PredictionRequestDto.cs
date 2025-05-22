namespace SEP4_User_Service.API.DTOs;

public class PredictionRequestDto
{
    public string Model { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public object Input { get; set; } = default!;
    public object Result { get; set; } = default!;
}