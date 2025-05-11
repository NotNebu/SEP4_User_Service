namespace SEP4_User_Service.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Birthday { get; set; } = string.Empty;

        // Adresse‐relation
        public ICollection<Location> Locations { get; set; } = new List<Location>();

        // HER skal Experiment‐navigationen stå
        public ICollection<Experiment> Experiments { get; set; } = new List<Experiment>();

        public User(string username, string password, string email, string firstname, string lastname, string birthday, ICollection<Location>? locations = null)
        {
            Id = Guid.NewGuid();
            Username = username;
            Password = password;
            Email = email;
            Firstname = firstname;
            Lastname = lastname;
            Birthday = birthday;
            Locations = locations ?? new List<Location>();
            Experiments = new List<Experiment>();
        }

        public User() { }
    }
}
