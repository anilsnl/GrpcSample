using System.Collections.Generic;

namespace Server.Data
{
    public static class UserMockDb
    {
        public static List<User> Users { get; }

        static UserMockDb()
        {
            Users = new List<User>()
            {
                new User() {Username = "anil", Password = "anil"}
            };
        }
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}