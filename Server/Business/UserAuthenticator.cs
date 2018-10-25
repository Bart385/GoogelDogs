using Server.Entities;

namespace Server.Business
{
    public static class UserAuthenticator
    {
        /// <summary>
        /// Authenticates a user trying to log in.
        /// </summary>
        /// <param name="username">Username from the user</param>
        /// <param name="password">Password from the user</param>
        /// <param name="userHandler">An UserHandler that has a list of all available users</param>
        /// <returns></returns>
        public static User Authenticate(string username, string password, UserHandler userHandler)
        {
            foreach (var user in userHandler.Users)
            {
                if (user.Username == username && user.Password == password)
                    return user;
  

            }

            return null;
        }
    }
}