using CoreLib;

namespace Downloader.Tools;

public class ArgsReader
{

    public static void ReadArgs(string[] args)
    {
        if (ParameterLib.HasParameter(args, "-help") || ParameterLib.HasParameter(args, "-?") || ParameterLib.HasParameter(args, "?"))
        {
            PrintHelp();
        }
        Config.UseLocal = ParameterLib.HasParameter(args, "-local");
        UbiServices.Urls.IsLocalTest = Config.UseLocal;
        if (ParameterLib.HasParameter(args, "-debug"))
        {
            UplayKit.Logs.Log_Switch.MinimumLevel = Serilog.Events.LogEventLevel.Verbose;
        } 
        Config.WaitTimeSocket = ParameterLib.GetParameter(args, "-time", 5);
        Config.ProductId = ParameterLib.GetParameter<uint>(args, "-product", 0);
        Config.ManifestId = ParameterLib.GetParameter(args, "-manifest", string.Empty);
        Config.ManifestPath = ParameterLib.GetParameter(args, "-manifestpath", string.Empty);
        Config.DownloadAddons = ParameterLib.HasParameter(args, "-addons");
        Config.LangToDownload = ParameterLib.GetParameter(args, "-lang", "default");
        Config.DownloadDirectory = ParameterLib.GetParameter(args, "-dir", string.Empty);
        Config.UsingFileList = ParameterLib.HasParameter(args, "-skip");
        Config.UsingOnlyFileList = ParameterLib.HasParameter(args, "-only");
        Config.SkipFilesPath = ParameterLib.GetParameter(args, "-skip", string.Empty);
        Config.OnlyDownloadFilesPath = ParameterLib.GetParameter(args, "-only", string.Empty);
        Config.Verify = ParameterLib.GetParameter(args, "-verify", true);
        Config.ParseVerifyBin = ParameterLib.HasParameter(args, "-vp");
        Config.DownloadAsChunks = ParameterLib.HasParameter(args, "-onlychunk");
        Config.MaxParallel = ParameterLib.GetParameter(args, "-parallel", Environment.ProcessorCount);
        Config.ParallelOptions.MaxDegreeOfParallelism = Config.MaxParallel;

        if (Config.UsingFileList && Config.UsingOnlyFileList)
        {
            Console.WriteLine("-skip and -only cannot be used in same time!");
            Environment.Exit(1);
        }

        Console.WriteLine($"Parallel option is now: {Config.MaxParallel}");
    }

    public static void PrintHelp()
    {
        HelpArgs.PrintHelp();
        Console.WriteLine("\n");
        Console.WriteLine("\t\tWelcome to Uplay Downloader CLI!");
        Console.WriteLine();
        Console.WriteLine("\t Arguments\t\t Arguments Description");
        Console.WriteLine();
        Console.WriteLine("\t -debug\t\t\t Debugging every request/response");
        Console.WriteLine("\t -time\t\t\t Using that as a wait time (5 is default [Low is better])");
        Console.WriteLine("\t -product\t\t Id of the Product");
        Console.WriteLine("\t -manifest\t\t Manifest of the Product");
        Console.WriteLine("\t -manifestpath\t\t Path to Manifest file");
        Console.WriteLine("\t -lang\t\t\t Download selected lang if available");
        Console.WriteLine("\t -skip\t\t\t Skip files from downloading");
        Console.WriteLine("\t -only\t\t\t Downloading only selected files from txt");
        Console.WriteLine("\t -dir\t\t\t A Path where to download the files");
        Console.WriteLine("\t -vp\t\t\t Make a json from verify.bin");
        Console.WriteLine("\t -verify\t\t Verifying files before downloading");
        Console.WriteLine("\t -onlychunk\t\t\t Downloading only the Uncompressed Chunks");
        Console.WriteLine("\t -parallel\t\t\t Downloading more files paralell, Default is Env.ProcessorCount");
        Console.WriteLine();
        Environment.Exit(0);
    }
}
