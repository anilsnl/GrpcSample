using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Server.Helpers;
using Server.Security.Encyption;

namespace Server.Security.Jwt
{
    public class TokenHelper : ITokenHelper
    {
        private TokenOptions _tokenOptions;
        private DateTimeOffset _accessTokenExpiration;
        public TokenHelper(IOptions<TokenOptions> tokenOptions)
        {
            _tokenOptions = tokenOptions.Value;
        }
        /// <summary>
        /// Creates token and returns the created token with expire date.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public AccessToken CreateToken(TokenUser user)
        {
            _accessTokenExpiration = DateTimeOffset.UtcNow.AddMinutes(_tokenOptions.AccessTokenExpiration);
            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
            var jwt = CreateJwtSecurityToken(_tokenOptions, user, signingCredentials);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwt);

            return new AccessToken
            {
                Token = token,
                Expiration = _accessTokenExpiration
            };

        }

        public AccessToken CreateRefreshToken()
        {
            return new AccessToken()
            {
                Expiration = DateTimeOffset.UtcNow.AddMinutes(_tokenOptions.RefreshTokenExpiration),
                Token = RandomGeneratorHelper.GenerateRandomString(RandomGeneratorType.Any, 64)
            };
        }

        private JwtSecurityToken CreateJwtSecurityToken(TokenOptions tokenOptions,
            TokenUser user,
            SigningCredentials signingCredentials)
        {
            var jwt = new JwtSecurityToken(
                issuer: tokenOptions.Issuer,
                audience: tokenOptions.Audience,
                expires: _accessTokenExpiration.UtcDateTime,
                notBefore: DateTime.Now,
                claims: SetClaims(user),
                signingCredentials: signingCredentials
            );
            return jwt;
        }

        private IEnumerable<Claim> SetClaims(TokenUser tokenUser)
        {
            var claims = new List<Claim>();
            claims.AddNameIdentifier(tokenUser.Id);
            claims.AddEmail(tokenUser.EMail);
            claims.AddName(tokenUser.Name);
            if (tokenUser.Cleims != null && tokenUser.Cleims.Any())
                claims.AddRoles(tokenUser.Cleims.ToArray());

            return claims;
        }
    }
}
