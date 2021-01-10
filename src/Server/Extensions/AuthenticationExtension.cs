using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Server.Security.Encyption;
using Server.Security.Jwt;

namespace Server.Extensions
{
    /// <summary>
    /// This class manages the Authentication configuration of the api.
    /// </summary>
    public static class AuthenticationExtension
    {
        /// <summary>
        /// Configure the api Authentication.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        public static void ConfigureAppAuthentication(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddOptions();
            services.Configure<TokenOptions>(Configuration.GetSection("TokenOptions"));
            
            var tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
            services.AddAuthorization();
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = tokenOptions.Issuer,
                        ValidAudience = tokenOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
                    };
                });

        }
    }
}