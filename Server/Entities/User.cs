namespace Server.Entities
{
    public class User
    {
        public string Username { get; }
        public string Password { get; }
        public Document Document { get; set; } = new Document();

        /// <summary>
        /// A User has an username and a password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
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