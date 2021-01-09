using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Server.Abstract;

namespace Server.Services
{
    /// <summary>
    /// Unary service gRPC sample.
    /// </summary>
    public class UnaryService : Unary.UnaryBase
    {
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
    }
}