using CoreLib;
using UplayKit;
using UplayKit.Connection;

namespace Dumperv2;

internal class Program
{
    static void Main(string[] args)
    {
        if (ParameterLib.HasParameter(args, "-help") || ParameterLib.HasParameter(args, "-?") || ParameterLib.HasParameter(args, "?"))
        {
            PrintHelp();
        }
        var currentDir = ParameterLib.GetParameter(args, "-dir", Environment.CurrentDirectory);
        if (ParameterLib.HasParameter(args, "-gen"))
        {
            GenerateStore.Work(ParameterLib.HasParameter(args, "-eng"));
            Environment.Exit(0);
        }
        if (ParameterLib.HasParameter(args, "-csv"))
        {
            List<Task> tasks = new();
            foreach (var file in Directory.GetFileSystemEntries(Path.Combine(currentDir, "files"), "*.manifest", SearchOption.AllDirectories))
            {
                tasks.Add(Task.Run(()=> {
                    var splitted = file.Split("_");
                    var x = splitted[0].Replace(Path.Combine(currentDir, "files"), "").Replace(Path.DirectorySeparatorChar, ' ').TrimStart();
                    var prodId = uint.Parse(x);
                    var manifestId = splitted[1].Replace(".manifest", "");
                    Dumper.Dump(Parsers.ParseManifestFile(file), file.Replace(".manifest", ".txt"));
                    Dumper.DumpAsCSV(Parsers.ParseManifestFile(file), null, file.Replace(".manifest", ""), manifestId, prodId);
                }));
            }
            Console.WriteLine("Waiting until all files are parsed to .csv");
            Task.WaitAll(tasks.ToArray());
            Environment.Exit(0);
        }
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
        Console.WriteLine("Is same Version? " + socket.VersionCheck());
        socket.PushVersion();
        bool IsAuthSuccess = socket.Authenticate(login.Ticket);
        Console.WriteLine("Is Auth Success? " + IsAuthSuccess);
        if (!IsAuthSuccess)
        {
            Console.WriteLine("Oops something is wrong!");
            Environment.Exit(1);
        }
        if (!Directory.Exists(Path.Combine(currentDir, "files")))
        {
            Directory.CreateDirectory(Path.Combine(currentDir, "files"));
        }
        OwnershipConnection ownership = new(socket, login.Ticket, login.SessionId);
        ownership.PushEvent += Ownership_PushEvent;
        var games_ = ownership.GetOwnedGames(true);
        GameLister.Work(currentDir, games_);
        ProductUbiService.Work(currentDir, games_.ToArray());
        var games = games_.Where(x => x.LatestManifest.Trim().Length > 0).ToArray();

        ListAllManifests.Work(currentDir, games);
        DownloadConnection downloadConnection = new(socket);

        if (ParameterLib.HasParameter(args, "-todl"))
        {
            ReDL.Work(currentDir, downloadConnection, ownership);
        }
        
        LatestManifest.Work(currentDir, games, downloadConnection, ownership);
        
       
        var pb = FromBranches.Work(currentDir, games);
        var games2 = games_.Where(x => x.Configuration.Length != 0).ToArray();
        ProductConfig.Work(currentDir, games2);
        if (ParameterLib.HasParameter(args, "-store"))
        {
            Console.WriteLine("STORE!!");
            StoreConnection storeConnection = new(socket);
            storeConnection.Init();
            var store = storeConnection.GetStore();
            if (store != null)
            {
                Console.WriteLine("STORE SUCCESS!!");
                StoreWork.Work(store);
                storeConnection.Close();
            }
        }
		    Console.ReadLine();
		    ownership.Close();
        downloadConnection.Close();
        socket.Disconnect();
        Console.WriteLine("Goodbye World!");
    }

    private static void PrintHelp()
    {
        HelpArgs.PrintHelp();
        Console.WriteLine();
        Console.WriteLine("\t\tWelcome to Uplay Dumper CLI!");
        Console.WriteLine();
        Console.WriteLine("\t Arguments\t\t Arguments Description");
        Console.WriteLine();
        Console.WriteLine("\t -dir\t\t\t A Path where its going to dump the files (manifest,csv,txt)");
        Console.WriteLine("\t -gen\t\t\t Generating Store data from storeref.json");
        Console.WriteLine("\t -csv\t\t\t Skipping login and start redumping from manifest files");
        Console.WriteLine("\t -todl\t\t\t Will try to download manifest from the todl.txt provided manifestIds and ProductIds");
        Console.WriteLine("\t -store\t\t\t Making storeref.json");
        Console.WriteLine();
        Environment.Exit(0);
    }

    private static void Ownership_PushEvent(object? sender, Uplay.Ownership.Push e)
    {
        Console.WriteLine("Ownership_PushEvent!" + e.ToString());
        File.AppendAllText("Ownership_PushEvent.txt", "\n" + e.ToString());
    }
}