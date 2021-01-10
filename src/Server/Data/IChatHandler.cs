using System.Threading.Tasks;
using Grpc.Core;
using Server.Abstract;

namespace Server.Data
{
    public interface IChatHandler
    {
        void Subscribe(string username, IServerStreamWriter<StartChantReply> responseStream);
        Task PublishAsync(StartChantReply reply);
    }
}