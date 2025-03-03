using Downloader.Managers;

namespace Downloader.Tools;

internal class ManifestAddonDownloader
{
    public static void DownloadManifest()
    {
        if (SocketManager.Download == null)
        {
            Console.WriteLine("Download is null!");
            return;
        }

        if (string.IsNullOrEmpty(Config.ManifestId))
        {
            Console.WriteLine("ManifestId is null!");
            return;
        }
        var manifestUrls = SocketManager.Download.GetUrl(Config.ManifestId, Config.ProductId);
        string prod_manifest = $"{Config.ProductManifest}.manifest";
        foreach (var url in manifestUrls)
        {
            var manifestBytes = FileGetter.DownloadFromURL(url);
            if (manifestBytes == null)
                continue;
            File.WriteAllBytes(prod_manifest, manifestBytes);
            Config.ManifestPath = Path.Combine(Directory.GetCurrentDirectory(), prod_manifest);
            break;
        }
    }
    public static void DownloadAddons()
    {
        if (SocketManager.Download == null)
        {
            Console.WriteLine("Download is null!");
            return;
        }

        if (string.IsNullOrEmpty(Config.ManifestId))
        {
            Console.WriteLine("ManifestId is null!");
            return;
        }
        var LicenseURLs = SocketManager.Download.GetUrl(Config.ManifestId, Config.ProductId, "license");
        foreach (var url in LicenseURLs)
        {
            var bytes = FileGetter.DownloadFromURL(url);
            if (bytes == null)
                continue;
            File.WriteAllBytes(Config.ProductManifest + ".license", bytes);
            break;
        }

        var MetadataURLs = SocketManager.Download.GetUrl(Config.ManifestId, Config.ProductId, "metadata");
        foreach (var url in MetadataURLs)
        {
            var bytes = FileGetter.DownloadFromURL(url);
            if (bytes == null)
                continue;
            File.WriteAllBytes(Config.ProductManifest + ".metadata", bytes);
            break;
        }
    }
}
