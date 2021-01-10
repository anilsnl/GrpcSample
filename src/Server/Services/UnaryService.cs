using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Server.Abstract;
using Server.Data;
using Server.Security.Jwt;

namespace Server.Services
{
    /// <summary>
    /// Unary service gRPC sample.
    /// </summary>
    [Authorize]
    public class UnaryService : Unary.UnaryBase
    {
        private readonly ITokenHelper _tokenHelper;

        public UnaryService(ITokenHelper tokenHelper)
        {
            _tokenHelper = tokenHelper;
        }

        /// <summary>
        /// Get country info by the county code.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<GetCountryByCodeReply> GetCountryByCode(GetCountryByCodeRequest request,
            ServerCallContext context)
        {
            var list = new List<GetCountryByCodeReply>()
            {
                new GetCountryByCodeReply()
                {
                    Code = "tr",
                    Name = "Turkey",
                    Population = 83000000
                },
                new GetCountryByCodeReply()
                {
                    Code = "us",
                    Name = "United Status",
                    Population = 328000000
                },
                new GetCountryByCodeReply()
                {
                    Code = "de",
                    Name = "Germany",
                    Population = 83150000
                }
            };
            
            var data = list.FirstOrDefault(a => string.Equals(a.Code, request.Code));
            return data != null
                ? Task.FromResult(data)
                : throw new RpcException(new Status(StatusCode.NotFound,"Resource could be found."), Metadata.Empty);
        }

        [AllowAnonymous]
        public override Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
        {
            if (UserMockDb.Users.Any(a=>a.Username==request.UserName&&a.Password==request.Password))
            {
                var token = _tokenHelper.CreateToken(new TokenUser()
                {
                    Id = request.UserName,
                    Name = request.UserName,
                    EMail = $"{request.UserName}@senell.net"
                });
                return Task.FromResult(new LoginReply()
                {
                    AccessToken  = token.Token,
                    ExpiryOn = Timestamp.FromDateTimeOffset(token.Expiration),
                    UserName = request.UserName
                });
            }

            throw new RpcException(new Status(StatusCode.Unauthenticated, "User or password is invalid!"));
        }

        [AllowAnonymous]
        public override Task<RegisterReply> Register(RegisterRequest request, ServerCallContext context)
        {
            if (UserMockDb.Users.Any(a=>a.Username==request.UserName))
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists,
                    "The user is already exist on the system!"));
            }
            UserMockDb.Users.Add(new User()
            {
                Password = request.Password,
                Username = request.UserName
            });
            return Task.FromResult(new RegisterReply()
            {
                IsSuccess = true
            });
        }
    }
}