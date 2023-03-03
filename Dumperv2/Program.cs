using CoreLib;
using UplayKit;
using UplayKit.Connection;

namespace Dumperv2
{
    internal class Program
    {
        public static string Version = "0.2";
        static void Main(string[] args)
        {
            if (!VersionCheck.Check())
            {
                Console.WriteLine("yrrz");
            }
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
                foreach (var file in Directory.GetFileSystemEntries(currentDir + "\\files", "*.manifest", SearchOption.AllDirectories))
                {
                    var splitted = file.Split("_");
                    Console.WriteLine(splitted[0]);
                    var prodId = uint.Parse(splitted[0].Replace(currentDir + "\\files\\", ""));
                    var manifestId = splitted[1].Replace(".manifest", "");
                    Dumper.Dump(Parsers.ParseManifestFile(file), file.Replace(".manifest", ".txt"));
                    Dumper.DumpAsCSV(Parsers.ParseManifestFile(file), null, file.Replace(".manifest", ""), manifestId, prodId);
                }
                Environment.Exit(0);
            }
            var login = LoginLib.TryLoginWithArgsCLI(args);
            if (login == null)
            {
                Console.WriteLine("Login was wrong :(!");
                Environment.Exit(1);
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
            if (!Directory.Exists(currentDir + "\\files"))
            {
                Directory.CreateDirectory(currentDir + "\\files");
            }
            OwnershipConnection ownership = new(socket);
            ownership.PushEvent += Ownership_PushEvent;
            DownloadConnection downloadConnection = new(socket);
            var games_ = ownership.GetOwnedGames(true);
            GameLister.Work(games_);
            ProductUbiService.Work(games_.ToArray());
            if (ParameterLib.HasParameter(args, "-todl"))
            {
                ReDL.Work(currentDir, downloadConnection, ownership);
            }

            var games = games_.Where(x => x.LatestManifest.Trim().Length > 0).ToArray();
            LatestManifest.Work(currentDir, games, downloadConnection, ownership);
            var games2 = games_.Where(x => x.Configuration.Length != 0).ToArray();
            ProductConfig.Work(currentDir, games2);
            if (ParameterLib.HasParameter(args, "-store"))
            {
                StoreConnection storeConnection = new(socket);
                storeConnection.Init();
                var store = storeConnection.GetStore();
                StoreWork.Work(store);
                storeConnection.Close();
            }

            downloadConnection.Close();
            ownership.Close();
            socket.Close();
            Console.WriteLine("Goodbye World!");
        }

        private static void PrintHelp()
        {
            HelpArgs.PrintHelp();
            Console.WriteLine("\n");
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

        }
    }
}