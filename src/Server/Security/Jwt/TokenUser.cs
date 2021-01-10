using System.Collections.Generic;

namespace Server.Security.Jwt
{
    public class TokenUser
    {
        public string Id { get; set; }
        public string EMail { get; set; }
        public string Name { get; set; }
        public List<string> Cleims { get; set; }
    }
}
