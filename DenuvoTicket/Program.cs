using CoreLib;
using Google.Protobuf;
using System.Text;
using UplayKit;
using UplayKit.Connection;

namespace DenuvoTicket;

internal class Program
{
    static void Main(string[] args)
    {
        var login = LoginLib.LoginArgs_CLI(args);
        if (login == null)
        {
            Console.WriteLine("Login was wrong! :(");
            Environment.Exit(1);
        }
        /*
        Logs.Log_Switch.MinimumLevel = Serilog.Events.LogEventLevel.Verbose;
        Serilog.Log.Logger = Logs.CreateFileLog();
        */
        DemuxSocket demuxSocket = new();
        demuxSocket.VersionCheck();
        demuxSocket.PushVersion();
        if (!demuxSocket.Authenticate(login.Ticket))
        {
            Console.WriteLine("Authenticate false");
            Environment.Exit(1);
        }
        OwnershipConnection ownershipConnection = new(demuxSocket, login.Ticket, login.SessionId);
        ownershipConnection.Initialize();
        DenuvoConnection denuvoConnection = new(demuxSocket);
        string tokenFile = ParameterLib.GetParameter<string>(args, "-tokenfile", string.Empty);
        uint appId = 0;
        string denuvoToken = string.Empty;
        if (!string.IsNullOrEmpty(tokenFile))
        {
            var token_readed = File.ReadAllText(tokenFile).Split("|");
            denuvoToken = token_readed[0];
            appId = uint.Parse(token_readed[1]);
        }

        if (appId == 0)
        {
            Console.WriteLine("Please enter your appId!");
            appId = uint.Parse(Console.ReadLine()!);
        }

        var (Token, _) = ownershipConnection.GetOwnershipToken(appId);
        if (string.IsNullOrEmpty(Token))
        {
            Console.WriteLine("you not own this appid");
            Environment.Exit(1);
        }
        if (string.IsNullOrEmpty(denuvoToken))
        {
            Console.WriteLine("Please enter your denuvo ticket request!");
            
            denuvoToken = Console.ReadLine()!;
        }
        var base64token = ByteString.FromBase64(Convert.ToBase64String(Encoding.UTF8.GetBytes(denuvoToken)));
        var gametoken = denuvoConnection.GetGameToken(Token, base64token);

        if (!gametoken.HasValue)
            Environment.Exit(1);

        if (gametoken.Value.result == Uplay.DenuvoService.Rsp.Types.Result.Success && gametoken.Value.response != null)
        {
            Console.WriteLine("Your Response Token:\n");
            Console.WriteLine(Encoding.UTF8.GetString(Convert.FromBase64String(gametoken.Value.response.GameToken.ToBase64())));
        }
        else
        {
            Console.WriteLine(gametoken.Value);
        }
        Console.ReadLine(); // users who doesn't use console to start it.
    }
}
