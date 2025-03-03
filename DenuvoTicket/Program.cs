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
        Console.WriteLine("Please enter your appId!");
        uint appid = uint.Parse(Console.ReadLine()!);

        var (Token, _) = ownershipConnection.GetOwnershipToken(appid);
        if (string.IsNullOrEmpty(Token))
        {
            Console.WriteLine("you not own this appid");
            Environment.Exit(1);
        }
        DenuvoConnection denuvoConnection = new(demuxSocket);
        Console.WriteLine("Please enter your denuvo ticket request!");
        char[] padding = ['='];
        string input = Console.ReadLine()!;
        string incoming = input
            .Replace('_', '/').Replace('-', '+');
        switch (input.Length % 4)
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
            Console.WriteLine(gametoken.Value.response.GameToken.ToBase64().TrimEnd(padding).Replace('+', '-').Replace('/', '_'));
        }
        else
        {
            Console.WriteLine(gametoken.Value.result);
        }
    }
}
