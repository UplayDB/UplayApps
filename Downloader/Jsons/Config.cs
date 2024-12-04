namespace Downloader;

static class Config
{
    public static string DownloadDirectory { get; set; } = Directory.GetCurrentDirectory();
    public static string VerifyBinPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), ".UD\\verify.bin");
    // Made use of MManager, but warn(?) if not run as admin,
    // Default off!
    public static bool InstallDependencies { get; set; } = false;
    public static bool Verify { get; set; } = true;
    public static bool UsingFileList { get; set; } = false;
    public static bool UsingOnlyFileList { get; set; } = false;
    public static string? ManifestId { get; set; }
    public static uint ProductId { get; set; }
    public static string? ProductManifest { get; set; }
    public static bool DownloadAsChunks { get; set; } = false;
    public static List<Uplay.Download.File> FilesToDownload { get; set; } = new();

}
