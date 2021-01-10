using System;

namespace Server.Security.Jwt
{
    public class AccessToken
    {
        public string Token { get; set; }
        public DateTimeOffset Expiration { get; set; }

    }
}
