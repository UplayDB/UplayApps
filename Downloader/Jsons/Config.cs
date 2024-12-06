namespace Downloader;

static class Config
{
    public static ParallelOptions ParallelOptions = new()
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount,
    };
    public static bool UseLocal { get; set; } = false;
    public static bool DownloadAddons { get; set; } = false;
    public static bool ParseVerifyBin { get; set; } = false;
    public static bool InstallDependencies { get; set; } = false;
    public static bool Verify { get; set; } = true;
    public static bool UsingFileList { get; set; } = false;
    public static bool UsingOnlyFileList { get; set; } = false;
    public static bool DownloadAsChunks { get; set; } = false;
    public static int MaxParallel { get; set; } = Environment.ProcessorCount;
    public static int WaitTimeSocket { get; set; } = 5;
    public static uint ProductId { get; set; } = 0;
    public static string ManifestPath { get; set; } = string.Empty;
    public static string SkipFilesPath { get; set; } = string.Empty;
    public static string OnlyDownloadFilesPath { get; set; } = string.Empty;
    public static string LangToDownload { get; set; } = "default";
    public static string DownloadDirectory { get; set; } = string.Empty;
    public static string VerifyBinPath { get; set; } = string.Empty;
    public static string ManifestId { get; set; } = string.Empty;
    public static string ProductManifest { get; set; } = string.Empty;

}
