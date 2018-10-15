namespace Server.Entities
{
    public class User
    {
        public string Username { get; }
        public string Password { get; }
        public Document Document { get; set; }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public override string ToString()
        {
            return $"{Username},{Password}";
        }
    }
}