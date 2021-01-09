using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Server.Abstract;

namespace Server.Services
{
    /// <summary>
    /// gRPC Serverside streaming sample service.
    /// </summary>
    public class ServerSideStreamingService : ServerSideStreaming.ServerSideStreamingBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GetImmediateCoinData(GetImmediateCoinDataRequest request,
            IServerStreamWriter<GetImmediateCoinDataReply> responseStream,
            ServerCallContext context)
        {
            var supportedCurrencyList = new List<(string, double)>() {("EUR", 1), ("TRY", 5), ("USD", 1.5)};
            var source =
                supportedCurrencyList.FirstOrDefault(a => string.Equals(a.Item1, request.SourceCurrencyCode.ToUpper()));
            if (source.Item1 == null)
            {
                throw new RpcException(new Status(StatusCode.Unavailable,
                    "The passing currency type is invalid or not supported yet."));
            }

            var random = new Random();
            while (true)
            {
                await Task.Delay(1000);
                var replay = new GetImmediateCoinDataReply();
                replay.CoinDataList.Add(new GetImmediateCoinDataReplyDataItem()
                {
                    CoinCode = $"BTC/{request.SourceCurrencyCode.ToUpper()}",
                    Value = random.NextDouble() * source.Item2 * 10
                });
                replay.CoinDataList.Add(new GetImmediateCoinDataReplyDataItem()
                {
                    CoinCode = $"ETH/{request.SourceCurrencyCode.ToUpper()}",
                    Value = random.NextDouble() * source.Item2 * 4
                });
                replay.CoinDataList.Add(new GetImmediateCoinDataReplyDataItem()
                {
                    CoinCode = $"DASH/{request.SourceCurrencyCode.ToUpper()}",
                    Value = random.NextDouble() * source.Item2 * 2
                });
                await responseStream.WriteAsync(replay);
            }
        }
    }
}