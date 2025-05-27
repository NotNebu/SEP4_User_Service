namespace SEP4_User_Service.Domain.Entities
{
    public class Prediction
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Model { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string Input { get; set; } = null!;
        public string Result { get; set; } = null!;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public User User { get; set; } = null!;
    }
}
