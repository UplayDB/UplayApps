using Uplay.Download;
using UplayKit;

namespace Downloader.Managers;

internal class ManifestManager
{
    public static Manifest ParsedManifest = new();
    public static List<Uplay.Download.File> ToDownloadFiles = new();
    public static void Init()
    {
        System.IO.File.Copy(Config.ManifestPath, Path.Combine(Config.DownloadDirectory, "uplay_install.manifest"), true);
        ParsedManifest = Parsers.ParseManifestFile(Config.ManifestPath);
        Console.WriteLine($"\nDownloaded and parsed manifest successfully:");
        Console.WriteLine($"Compression Method: {ParsedManifest.CompressionMethod} IsCompressed? {ParsedManifest.IsCompressed} Version {ParsedManifest.Version}");
        ParseLang();
        ParseConfigFileLists();
    }

    public static void ParseLang()
    {
        // Todo Make this better
        if (ParsedManifest.Languages.ToList().Count == 0)
        {
            ToDownloadFiles = ChunkManager.AllFiles(ParsedManifest);
            return;
        }

        if (Config.LangToDownload == "all")
        {
            ToDownloadFiles = ChunkManager.AllFiles(ParsedManifest);
        }
        else if (Config.LangToDownload == "default")
        {
            Console.WriteLine("Languages to use (just press enter to choose nothing, and all for all chunks)");
            ParsedManifest.Languages.ToList().ForEach(x => Console.WriteLine(x.Code));

            var langchoosed = Console.ReadLine();

            if (string.IsNullOrEmpty(langchoosed))
            {
                ToDownloadFiles.AddRange(ChunkManager.RemoveNonEnglish(ParsedManifest));
                return;
            }
            if (langchoosed == "all")
            {
                ToDownloadFiles = ChunkManager.AllFiles(ParsedManifest);
            }
            else
            {
                ToDownloadFiles.AddRange(ChunkManager.RemoveNonEnglish(ParsedManifest));
                Config.LangToDownload = langchoosed;
                ToDownloadFiles.AddRange(ChunkManager.AddLanguage(ParsedManifest, Config.LangToDownload));
            }
        }
        else
        {
            ToDownloadFiles.AddRange(ChunkManager.RemoveNonEnglish(ParsedManifest));
            ToDownloadFiles.AddRange(ChunkManager.AddLanguage(ParsedManifest, Config.LangToDownload));
        }
    }

    public static void ParseConfigFileLists()
    {
        ToDownloadFiles = DLFile.FileNormalizer(ToDownloadFiles);
        List<string> skip_files = new();
        if (Config.UsingFileList)
        {
            if (System.IO.File.Exists(Config.SkipFilesPath))
            {
                var lines = System.IO.File.ReadAllLines(Config.SkipFilesPath);
                skip_files.AddRange(lines);
                Console.WriteLine("Skipping files Added");
            }
            ChunkManager.RemoveSkipFiles(skip_files);
        }
        if (Config.UsingOnlyFileList)
        {
            if (System.IO.File.Exists(Config.OnlyDownloadFilesPath))
            {
                var lines = System.IO.File.ReadAllLines(Config.OnlyDownloadFilesPath);
                skip_files.AddRange(lines);
                Console.WriteLine("Download only Added");
            }
            ToDownloadFiles = ChunkManager.AddDLOnlyFiles(skip_files);
        }
        Console.WriteLine("\tFiles Ready to work\n");
    }
}
