using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleTables;
using Grpc.Core;
using Grpc.Net.Client;
using Server.Abstract;

namespace Client
{
    /// <summary>
    /// The main class runs the app.
    /// </summary>
    public class Program
    {
        private static readonly string _url = "https://localhost:5001";
        private static LoginReply _loginReply;

        /// <summary>
        /// The main methods runs the app.
        /// </summary>
        /// <param name="args"></param>
        public static async Task Main(string[] args)
        {
            try
            {
                while (true)
                {
                    GetAuthorizedGrpcChannel();
                    Console.WriteLine("Select the connection method type!");
                    Console.WriteLine("1. Unary\n2. Serverside Streaming\n3. Clientside Streaming\n3. Bidirectional");
                    switch (Console.ReadLine())
                    {
                        case "1":
                            await UnaryCallAsync();
                            break;
                        case "2":
                            await ServersideStreamingCallAsync();
                            break;
                        case "3":
                            await ClientsideStreamingCallAsync();
                            break;
                        default:
                            Console.WriteLine("Invalid operation!\nRetry, press Ctrl+C to exist.");
                            break;
                    }
                }
            }
            catch (RpcException e)
            {
                Console.WriteLine($"Status: {e.Status}");
                Console.WriteLine("Retry, press Ctrl+C to Exit!");
                await Main(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task UnaryCallAsync()
        {
            Console.WriteLine("Enter two digit country code to execute GetCountryByCodeAsync gRPC method: ");
            var countryCode = Console.ReadLine();
            using var channel = GetAuthorizedGrpcChannel();
            var unaryClient = new Unary.UnaryClient(channel);
            var response = await unaryClient.GetCountryByCodeAsync(new GetCountryByCodeRequest()
            {
                Code = countryCode
            });
            Console.WriteLine($"The population of {response.Name} is {response.Population:N0}");
        }

        private static async Task ServersideStreamingCallAsync()
        {
            Console.WriteLine("Enter source currency code to execute GetCountryByCodeAsync gRPC method: ");
            var currencyCode = Console.ReadLine();
            using var channel = GetAuthorizedGrpcChannel();
            var serverSideStreamingClient = new ServerSideStreaming.ServerSideStreamingClient(channel);
            var responseStream = serverSideStreamingClient.GetImmediateCoinData(new GetImmediateCoinDataRequest()
            {
                SourceCurrencyCode = currencyCode
            });
            while (await responseStream.ResponseStream.MoveNext())
            {
                Console.Clear();
                ConsoleTable.From(responseStream.ResponseStream.Current.CoinDataList
                        .Select(a => new
                        {
                            Coin = a.CoinCode,
                            Value = a.Value.ToString("N3")
                        }).ToList())
                    .Write(Format.Default);
            }
        }

        private static async Task ClientsideStreamingCallAsync()
        {
            Console.WriteLine("Connecting the log server to be able test  SendLog method...");
            using var channel = GetAuthorizedGrpcChannel();
            var clientSideStreamingClient = new ClientSideStreaming.ClientSideStreamingClient(channel);
            var streamingCall = clientSideStreamingClient.SendLog(Metadata.Empty);
            await streamingCall.RequestStream.WriteAsync(new SendLogRequest()
                {LogType = LogType.Critical, LogData = "Service down!!!"});
            await streamingCall.RequestStream.WriteAsync(new SendLogRequest()
                {LogType = LogType.Error, LogData = "Some missing data while getting stream!!!"});
            await streamingCall.RequestStream.WriteAsync(new SendLogRequest()
                {LogType = LogType.Info, LogData = "A new client joined to the stream!!!"});
            await streamingCall.RequestStream.WriteAsync(new SendLogRequest()
                {LogType = LogType.Warming, LogData = "Unauthorized request exist!!!"});
            await streamingCall.RequestStream.CompleteAsync();
            var response = await streamingCall.ResponseAsync;
            Console.WriteLine($"The logging server response is {response.IsDelivered}");
        }

        private static GrpcChannel GetAuthorizedGrpcChannel()
        {
            while (_loginReply == null || _loginReply.ExpiryOn.ToDateTimeOffset() <= DateTimeOffset.Now)
            {
                var authService = new Unary.UnaryClient(GrpcChannel.ForAddress(_url));
                string username, password;
                Console.Clear();
                Console.WriteLine(
                    "You've to login to be able continue using the app!! \n1. Login 2. Register\nType the operation code: ");
                switch (Console.ReadLine())
                {
                    case "1":
                        Console.WriteLine("Login to the app...\nUsername:");
                        username = Console.ReadLine();
                        Console.WriteLine("Password:");
                        password = Console.ReadLine();
                        _loginReply = authService.Login(new LoginRequest()
                        {
                            UserName = username,
                            Password = password
                        });
                        break;
                    case "2":
                        Console.WriteLine("Register the app...\nUsername: ");
                        username = Console.ReadLine();
                        Console.WriteLine("Password:");
                        password = Console.ReadLine();
                        authService.Register(new RegisterRequest()
                        {
                            UserName = username,
                            Password = password
                        });
                        _loginReply = authService.Login(new LoginRequest()
                        {
                            UserName = username,
                            Password = password
                        });
                        break;
                    default:
                        Console.WriteLine("Invalid option selected!");
                        Task.Delay(500);
                        break;
                }
            }

            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                if (!string.IsNullOrEmpty(_loginReply.AccessToken))
                {
                    metadata.Add("Authorization", $"Bearer {_loginReply.AccessToken}");
                }

                return Task.CompletedTask;
            });

            // SslCredentials is used here because this channel is using TLS.
            // CallCredentials can't be used with ChannelCredentials.Insecure on non-TLS channels.
            var channel = GrpcChannel.ForAddress(_url, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            });
            return channel;
        }
    }
}