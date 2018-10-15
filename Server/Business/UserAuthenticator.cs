using Server.Entities;

namespace Server.Business
{
    public static class UserAuthenticator
    {
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