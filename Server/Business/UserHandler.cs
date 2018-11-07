using System.Collections.Generic;
using System.IO;
using Server.Entities;

namespace Server.Business
{
    public class UserHandler
    {
        public List<User> Users { get; }

        private readonly string _accountFilePath;

        public UserHandler(string accountFilePath)
        {
            _accountFilePath = accountFilePath;
            Users = LoadUsersFromFile();
        }

        /// <summary>
        /// Loads all users from a file
        /// </summary>
        /// <returns></returns>
        private List<User> LoadUsersFromFile()
        {
            List<User> users = new List<User>();
            string[] usersFromFile = File.ReadAllLines(_accountFilePath);
            foreach (var user in usersFromFile)
            {
                string[] credentials = user.Split(',');
                users.Add(new User(credentials[0], credentials[1]));
            }

            return users;
        }

        /// <summary>
        /// Adds a new user to the user file.
        /// </summary>
        /// <param name="user"></param>
        public void AddNewUser(User user)
        {
            Users.Add(user);
            File.AppendAllText(_accountFilePath, $"\r\n{user.ToString()}");
        }
    }
}