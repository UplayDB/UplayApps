using CoreLib;
using Google.Protobuf;
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
        string incoming = denuvoToken.Replace('_', '/').Replace('-', '+');
        switch (denuvoToken.Length % 4)
        {
            case 2: incoming += "=="; break;
            case 3: incoming += "="; break;
        }
        var base64token = ByteString.FromBase64(incoming);
        var gametoken = denuvoConnection.GetGameToken(Token, base64token);

        if (!gametoken.HasValue)
            Environment.Exit(1);

        if (gametoken.Value.result == Uplay.DenuvoService.Rsp.Types.Result.Success && gametoken.Value.response != null)
        {
            Console.WriteLine(gametoken.Value.response.GameToken.ToBase64().TrimEnd(['=']).Replace('+', '-').Replace('/', '_'));
        }
        else
        {
            Console.WriteLine(gametoken.Value.result);
        }
    }
}
