using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Server.Abstract;
using Server.Data;

namespace Server.Services
{
    /// <summary>
    /// gRPC Bidirectional sample service.
    /// </summary>
    [Authorize]
    public class BidirectionalService : BidirectionalStreaming.BidirectionalStreamingBase
    {
        private readonly IChatHandler _chatHandler;

        public BidirectionalService(IChatHandler chatHandler)
        {
            _chatHandler = chatHandler;
        }

        public override async Task StartChant(IAsyncStreamReader<StartChantRequest> requestStream,
            IServerStreamWriter<StartChantReply> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var userName = context.GetHttpContext().User.Identity?.Name;
                _chatHandler.Subscribe(userName, responseStream);
                await _chatHandler.PublishAsync(new StartChantReply()
                {
                    Message = requestStream.Current.Message,
                    UserName = userName
                });
            }
        }
    }
}