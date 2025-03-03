using System.Diagnostics.CodeAnalysis;
using UbiServices.Records;
using UplayKit;
using UplayKit.Connection;

namespace Downloader.Managers;

public class SocketManager
{
    public static string OWToken { get; protected set; } = string.Empty;
    public static ulong Exp { get; protected set; } = 0;
    public static DemuxSocket Socket { get; protected set; } = CreateNew();
    public static OwnershipConnection? Ownership { get; protected set; } = null;
    public static DownloadConnection? Download { get; protected set; } = null;
    public static DemuxSocket CreateNew()
    {
        Socket = new()
        {
            WaitInTimeMS = Config.WaitTimeSocket
        };
        if (Socket.IsConnected)
        {
            Socket.VersionCheck();
            Socket.PushVersion();
        }
        return Socket;
    }

    public static void Login([NotNull] LoginJson loginJson)
    {
        bool IsAuthSuccess = Socket.Authenticate(loginJson.Ticket);
        if (!IsAuthSuccess)
        {
            Console.WriteLine("Auth was not success!");
            Environment.Exit(1);
        }
        Ownership = new(Socket, loginJson.Ticket, loginJson.SessionId);
        Download = new(Socket);
    }


    static DateTime GetTimeFromEpoc(ulong epoc)
    {
        DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return dateTime.AddSeconds(epoc);
    }

    static ulong GetEpocTime()
    {
        TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return (ulong)t.TotalSeconds;

    }

    public static void GetOwnership()
    {
        if (Ownership == null)
        {
            Console.WriteLine("Ownership is null!");
            return;
        }
        var (Token, Expiration) = Ownership.GetOwnershipToken(Config.ProductId);
        if (Ownership.IsConnectionClosed == true || string.IsNullOrEmpty(Token))
            throw new("Product not owned");
        OWToken = Token;
        Exp = Expiration;
        Console.WriteLine($"Expires in {GetTimeFromEpoc(Exp)}");
    }

    public static bool InitDownload()
    {
        if (Download == null)
        {
            Console.WriteLine("Download is null!");
            return false;
        }
        return Download.InitDownloadToken(OWToken);
    }

    public static void Quit()
    {
        Ownership?.Close();
        Ownership = null;
        Download?.Close();
        Download = null;
        Socket.Disconnect();
        Socket.Dispose();
    }

    public static void CheckOW(uint ProdId)
    {
        if (Exp > GetEpocTime())
            return;
        Console.WriteLine("Your token has no more valid, getting new!");
        if (Ownership != null && !Ownership.IsConnectionClosed)
        {
            (OWToken, Exp) = Ownership.GetOwnershipToken(ProdId);
            Console.WriteLine("Is Token get success? " + Ownership.IsConnectionClosed + " " + (Exp != 0));
        }
    }

}
