namespace Server.Security.Jwt
{
    public interface ITokenHelper
    {
        AccessToken CreateToken(TokenUser tokenUser);
        AccessToken CreateRefreshToken();
    }
}
