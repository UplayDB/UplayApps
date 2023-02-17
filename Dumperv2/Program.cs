using UplayKit;
using UplayKit.Connection;
using CoreLib;
using RestSharp;

//  This will replace the first dumper, cus that slow and should be redone from the ground

namespace Dumperv2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
            var AllText = "";

            foreach (var files in Directory.GetFiles("from"))
            {
                AllText += File.ReadAllText(files);
            }
            File.WriteAllText("allfiles.txt", AllText);
            */
            var todl = File.ReadAllLines("todl.txt");
            
            var login = LoginLib.TryLoginWithArgsCLI(args);

            if (login == null)
            {
                Console.WriteLine("Login was wrong :(!");
                Environment.Exit(1);
            }
            var currentDir = ParameterLib.GetParameter(args, "-dir", Environment.CurrentDirectory);
            

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
            var games = games_.Where(x => x.LatestManifest.Trim().Length > 0).ToArray();
            
            for (int i = 0; i < todl.Length; i++)
            {
                var splitted = todl[i].Split("=");
                var manifest = splitted[0];
                var prod = splitted[1];

                if (File.Exists(currentDir + "\\files\\" + prod + "_" + manifest + ".manifest"))
                {
                    Console.WriteLine("Skipped!");
                }
                else
                {
                    uint prodId = uint.Parse(prod);
                    string ownershipToken_1 = ownership.GetOwnershipToken(prodId).Item1;
                    downloadConnection.InitDownloadToken(ownershipToken_1);
                    if (downloadConnection.isServiceSuccess != false)
                    {
                        string manifestUrl_1 = downloadConnection.GetUrl(manifest, prodId);
                        if (manifestUrl_1 != "")
                        {
                            Console.WriteLine("Manifest dl!");
                            var rc1 = new RestClient();
                            File.WriteAllBytes(currentDir + "\\files\\" + prod + "_" + manifest + ".manifest", rc1.DownloadData(new(manifestUrl_1)));
                        }
                    }
                    Console.WriteLine(manifest + " " + prod + " " + i);
                }

            }



            Console.WriteLine("Goodbye World!");
        }

        private static void Ownership_PushEvent(object? sender, Uplay.Ownership.Push e)
        {
            
        }
    }
}