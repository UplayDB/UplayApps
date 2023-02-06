using System.Text.RegularExpressions;

namespace Downloader
{
    class Config
    {
        public string DownloadDirectory { get; set; }
        // Made use of MManager, but warn(?) if not run as admin,
        // Default off!
        public bool InstallDependencies { get; set; } = false;
        public bool InstallAllLang { get; set; } = false;
        public bool Verify { get; set; } = false;
        public bool UsingFileList { get; set; } = false;
        public bool UsingOnlyFileList { get; set; } = false;

        public string ManifestId { get; set; }
        public uint ProductId { get; set; }
        public string ProductManifest { get; set; } = $"{Downloader.Config.ProductId}_{Downloader.Config.ManifestId}";

        public List<Uplay.Download.File> FilesToDownload { get; set; }
        public List<Regex> FilesToDownloadRegex { get; set; }

    }
}
