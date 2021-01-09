using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Server.Abstract;

namespace Server.Services
{
    /// <summary>
    /// gRPC Serverside streaming sample service.
    /// </summary>
    public class ClientSideStreamingService : ClientSideStreaming.ClientSideStreamingBase
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Init new instance of <see cref="ClientSideStreamingService"/>
        /// </summary>
        /// <param name="logger"></param>
        public ClientSideStreamingService(ILogger<ClientSideStreamingService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Use to send log records.
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<SendLogReply> SendLog(IAsyncStreamReader<SendLogRequest> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var message = requestStream.Current.LogData;
                switch (requestStream.Current.LogType)
                {
                    case LogType.Info:
                        _logger.LogInformation(message);
                        break;
                    case LogType.Critical:
                        _logger.LogCritical(message);
                        break;
                    case LogType.Warming:
                        _logger.LogWarning(message);
                        break;
                    case LogType.Error:
                        _logger.LogError(message);
                        break;
                    default:
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid log type."));
                }
            }

            return new SendLogReply()
            {
                IsDelivered = true
            };
        }
    }
}