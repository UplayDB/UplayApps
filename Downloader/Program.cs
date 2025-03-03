using CoreLib;
using Downloader.Managers;
using Downloader.Tools;

namespace Downloader;

public class Program
{
    internal static object GlobalLock = new();

    static void Main(string[] args)
    {
        ArgsReader.ReadArgs(args);
        UbiServices.Urls.IsLocalTest = Config.UseLocal;
        var login = LoginLib.LoginArgs_CLI(args);
        if (login == null)
        {
            Console.WriteLine("Login was wrong! :(");
            Environment.Exit(1);
        }
        SocketManager.Login(login);
        if (SocketManager.Ownership == null)
        {
            Console.WriteLine("Ownnership is still null after login :(");
            Environment.Exit(1);
        }
        var ownedGames = SocketManager.Ownership.GetOwnedGames();
        if (ownedGames == null || ownedGames.Count == 0)
        {
            Console.WriteLine("No games owned?!");
            Environment.Exit(1);
        }
        if (Config.ProductId == 0 && string.IsNullOrEmpty(Config.ManifestId))
            GameSelector.Select(ownedGames);
        else
            SocketManager.GetOwnership();
        if (!Directory.Exists(Config.DownloadDirectory))
            Directory.CreateDirectory(Config.DownloadDirectory);

        if (!SocketManager.InitDownload())
        {
            Console.WriteLine("Initializing download failed!");
            Environment.Exit(1);
        }
        if (Config.DownloadAddons)
            ManifestAddonDownloader.DownloadAddons();
        if (string.IsNullOrEmpty(Config.ManifestPath))
        {
            ManifestAddonDownloader.DownloadManifest();
        }
        Console.WriteLine(Config.ManifestPath);
        ManifestManager.Init();
        VerifyManager.Init();
        DLWorker.DownloadWorker();
        SocketManager.Quit();
    }
}
