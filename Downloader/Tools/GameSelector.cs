using CoreLib;
using Downloader.Managers;
using System.Text.Json;

namespace Downloader.Tools;

internal class GameSelector
{
    public static void Select(List<Uplay.Ownership.OwnedGame> games)
    {
        File.WriteAllText("games_full.json", JsonSerializer.Serialize(games.Where(x => x.ProductType == (uint)Uplay.Ownership.OwnedGame.Types.ProductType.Game), new JsonSerializerOptions() { WriteIndented = true }));
        var owned = games.Where(game => game.LatestManifest.Trim().Length > 0
                   && game.ProductType == (uint)Uplay.Ownership.OwnedGame.Types.ProductType.Game
                   && game.Owned
                   && game.State == (uint)Uplay.Ownership.OwnedGame.Types.State.Playable
                   && !game.LockedBySubscription
                   ).ToList();

        File.WriteAllText("games.json", JsonSerializer.Serialize(owned, new JsonSerializerOptions() { WriteIndented = true }));

        Console.WriteLine("-1) Your Downloadable games:.");
        Console.WriteLine("----------------------");
        int gameIds = 0;
        foreach (var game in owned)
        {
            Console.WriteLine($"\n\t{Appname.GetAppName(game.ProductId)}");
            Console.WriteLine($"({gameIds}) ProductId ({game.ProductId}) Manifest {game.LatestManifest}");
            gameIds++;
        }
        Console.WriteLine("Please select:");
        Console.ReadLine();

        int selection = int.Parse(Console.ReadLine()!);
        if (selection == -1)
        {
            Console.WriteLine("> Input the 20-byte long manifest identifier:");
            Config.ManifestId = Console.ReadLine()!.Trim();

            Console.WriteLine("> Input the productId:");
            Config.ProductId = uint.Parse(Console.ReadLine()!.Trim());
        }
        else if (selection <= gameIds)
        {
            Config.ProductId = owned[selection].ProductId;
            Config.ManifestId = owned[selection].LatestManifest;
        }
        Config.ProductManifest = $"{Config.ProductId}_{Config.ManifestId}";
        Config.DownloadDirectory = string.IsNullOrEmpty(Config.DownloadDirectory) ?  Path.Combine(Directory.GetCurrentDirectory(), Config.ProductId.ToString(), Config.ManifestId) : Config.DownloadDirectory; 
        SocketManager.GetOwnership();
    }
}
