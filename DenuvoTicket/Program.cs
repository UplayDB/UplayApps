using CoreLib;
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

        var token = ownershipConnection.GetOwnershipToken(appid);
        if (string.IsNullOrEmpty(token.Item1))
        {
            Console.WriteLine("you not own this appid");
            Environment.Exit(1);
        }
        DenuvoConnection denuvoConnection = new(demuxSocket);
        Console.WriteLine("Please enter your denuvo ticket request!");
        var gametoken = denuvoConnection.GetGameToken(token.Item1, Console.ReadLine()!);

        if (!gametoken.HasValue)
            Environment.Exit(1);

        if (gametoken.Value.result == Uplay.DenuvoService.Rsp.Types.Result.Success && gametoken.Value.response != null)
        {
            Console.WriteLine(gametoken.Value.response.GameToken.ToBase64());
        }
        else
        {
            Console.WriteLine(gametoken.Value.result);
        }
    }
}
