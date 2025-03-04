using CoreLib;
using UplayKit;
using UplayKit.Connection;

namespace AchiSync;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");
        var login = LoginLib.LoginArgs_CLI(args);

        if (login == null)
        {
            Console.WriteLine("Login was wrong :(!");
            Environment.Exit(1);
        }
        if (ParameterLib.HasParameter(args, "-debug"))
        {
            UplayKit.Logs.Log_Switch.MinimumLevel = Serilog.Events.LogEventLevel.Verbose;
        }
        DemuxSocket socket = new();
        socket.VersionCheck();
        socket.PushVersion();
        bool IsAuthSuccess = socket.Authenticate(login.Ticket);
        Console.WriteLine("Is Auth Success? " + IsAuthSuccess);
        if (!IsAuthSuccess)
        {
            Console.WriteLine("Auth was not successfull!");
            return;
        }
        OwnershipConnection ownership = new(socket, login.Ticket, login.SessionId);
        var games_ = ownership.GetOwnedGames(true);
        if (games_ == null)
        {
            Console.WriteLine("you dont have any games!");
            return;
        }

        var playgames = games_.Where(game => game.ProductType == (uint)Uplay.Ownership.OwnedGame.Types.ProductType.Game
                   && game.Owned
                   && game.State == (uint)Uplay.Ownership.OwnedGame.Types.State.Playable
                   && !game.LockedBySubscription).ToList();
        AchCon achCon = new AchCon();
        achCon.ConnectAchi(socket, "", login.UserId);
        Console.WriteLine("-1) Your Downloadable games:.");
        Console.WriteLine("----------------------");
        foreach (var game in playgames)
        {
            Console.WriteLine($"\n\t{Appname.GetAppName(game.ProductId)}");
            Console.WriteLine($"ProductId ({game.ProductId})");

            Console.WriteLine(string.Join(", ", achCon.GetProductAchievements(game.ProductId.ToString())));
        }
        return;
        Console.WriteLine("Please type the uplay Id of achi you want to gather:");

        uint pid = uint.Parse(Console.ReadLine()!);
        var ticket = ownership.GetOwnershipToken(pid);
        if (string.IsNullOrEmpty(ticket.Item1))
        {
            Console.WriteLine("cannot get ticket for this game!");
            return;
        }
        
        achCon.GetProductAchievements(pid.ToString());
    }
}