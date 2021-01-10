using System.Collections.Concurrent;
using System.Threading.Tasks;
using Grpc.Core;
using Server.Abstract;

namespace Server.Data
{
    public class ChatHandler : IChatHandler
    {
        private readonly ConcurrentDictionary<string, IServerStreamWriter<StartChantReply>> _roomList;

        public ChatHandler()
        {
            _roomList = new ConcurrentDictionary<string, IServerStreamWriter<StartChantReply>>();
        }

        public void Subscribe(string username, IServerStreamWriter<StartChantReply> responseStream)
        {
            _roomList.TryAdd(username, responseStream);
        }

        public Task PublishAsync(StartChantReply reply)
        {
            return Task.Run(() => { Parallel.ForEach(_roomList, async a => { await a.Value.WriteAsync(reply); }); });
        }
    }
}