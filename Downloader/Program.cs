using CoreLib;
using Newtonsoft.Json;
using RestSharp;
using System.Text;
using UbiServices.Records;
using UplayKit;
using UplayKit.Connection;

namespace Downloader
{
    public class Program
    {
        public static string OWToken = "";
        public static ulong Exp = 0;
        public static OwnershipConnection? ownershipConnection = null;
        public static DemuxSocket? socket = null;
        static void Main(string[] args)
        {
            if (ParameterLib.HasParameter(args, "-help") || ParameterLib.HasParameter(args, "-?") || ParameterLib.HasParameter(args, "?"))
            {
                PrintHelp();
            }
            #region Argument thingy
            DLWorker.CreateNew();
            bool haslocal = ParameterLib.HasParameter(args, "-local");
            Debug.isDebug = ParameterLib.HasParameter(args, "-debug");
            int WaitTime = ParameterLib.GetParameter(args, "-time", 5);
            DLWorker.Config.ProductId = ParameterLib.GetParameter<uint>(args, "-product", 0);
            DLWorker.Config.ManifestId = ParameterLib.GetParameter(args, "-manifest", "");
            string manifest_path = ParameterLib.GetParameter(args, "-manifestpath", "");
            bool hasAddons = ParameterLib.HasParameter(args, "-addons");
            string lang = ParameterLib.GetParameter(args, "-lang", "default");
            DLWorker.Config.DownloadDirectory = ParameterLib.GetParameter(args, "-dir", $"{Directory.GetCurrentDirectory()}\\{DLWorker.Config.ProductId}\\{DLWorker.Config.ManifestId}\\");
            DLWorker.Config.UsingFileList = ParameterLib.HasParameter(args, "-skip");
            DLWorker.Config.UsingOnlyFileList = ParameterLib.HasParameter(args, "-only");
            string skipping = ParameterLib.GetParameter(args, "-skip", "skip.txt");
            string onlygetting = ParameterLib.GetParameter(args, "-only", "only.txt");
            DLWorker.Config.Verify = ParameterLib.GetParameter(args, "-verify", true);
            bool hasVerifyPrint = ParameterLib.HasParameter(args, "-vp");
            DLWorker.Config.DownloadAsChunks = ParameterLib.HasParameter(args, "-onlychunk");

            if (DLWorker.Config.UsingFileList && DLWorker.Config.UsingOnlyFileList)
            {
                Console.WriteLine("-skip and -only cannot be used in same time!");
                Environment.Exit(1);
            }
            #endregion

            UbiServices.Urls.IsLocalTest = haslocal;
            #region Login
            int indx = ParameterLib.GetParameter(args, "-lindx", 0);
            var login = LoginLib.LoginFromStore(args, indx);

            if (login == null)
            {
                Console.WriteLine("Login was wrong :(!");
                Environment.Exit(1);
            }

            #endregion
            #region Starting Connections, Getting game
            socket = new();
            socket.WaitInTimeMS = WaitTime;
            Console.WriteLine("Is same Version? " + socket.VersionCheck());
            socket.PushVersion();
            bool IsAuthSuccess = socket.Authenticate(login.Ticket);
            Console.WriteLine("Is Auth Success? " + IsAuthSuccess);
            if (!IsAuthSuccess)
            {
                Console.WriteLine("Oops something is wrong!");
                Environment.Exit(1);
            }
            ownershipConnection = new(socket, login.Ticket, login.SessionId);
            DownloadConnection downloadConnection = new(socket);
            var owned = ownershipConnection.GetOwnedGames(false);
            if (owned == null || owned.Count == 0)
            {
                Console.WriteLine("No games owned?!");
                Environment.Exit(1);
            }
            #endregion
            #region Game printing & Argument Check
            Uplay.Download.Manifest parsedManifest = new();
            RestClient rc = new();

            if (DLWorker.Config.ProductId == 0 && DLWorker.Config.ManifestId == "")
            {
                owned = owned.Where(game => game.LatestManifest.Trim().Length > 0 
                && game.ProductType == (uint)Uplay.Ownership.OwnedGame.Types.ProductType.Game
                && game.Owned
                && !game.LockedBySubscription
                ).ToList();

                Console.WriteLine("-1) Your games:.");
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
                    DLWorker.Config.ManifestId = Console.ReadLine()!.Trim();

                    Console.WriteLine("> Input the productId:");
                    DLWorker.Config.ProductId = uint.Parse(Console.ReadLine()!.Trim());
                }
                else if (selection <= gameIds)
                {
                    DLWorker.Config.ManifestId = owned[selection].LatestManifest;
                    DLWorker.Config.ProductId = owned[selection].ProductId;
                    Console.WriteLine(DLWorker.Config.ManifestId + " " + DLWorker.Config.ProductId);
                }

                DLWorker.Config.DownloadDirectory = ParameterLib.GetParameter(args, "-dir", $"{Directory.GetCurrentDirectory()}\\{DLWorker.Config.ProductId}\\{DLWorker.Config.ManifestId}\\");
                DLWorker.Config.ProductManifest = $"{DLWorker.Config.ProductId}_{DLWorker.Config.ManifestId}";

                if (!Directory.Exists(DLWorker.Config.DownloadDirectory))
                {
                    Directory.CreateDirectory(DLWorker.Config.DownloadDirectory);
                }

                // Getting ownership token
                var ownershipToken = ownershipConnection.GetOwnershipToken(DLWorker.Config.ProductId);
                if (ownershipConnection.IsConnectionClosed == false || string.IsNullOrEmpty(ownershipToken.Item1)) { throw new("Product not owned"); }
                OWToken = ownershipToken.Item1;
                Exp = ownershipToken.Item2;
                Console.WriteLine($"Expires in {GetTimeFromEpoc(Exp)}");
                downloadConnection.InitDownloadToken(OWToken);

                if (manifest_path != "")
                {
                    File.Copy(manifest_path, DLWorker.Config.DownloadDirectory + "/uplay_install.manifest", true);
                    parsedManifest = Parsers.ParseManifestFile(manifest_path);
                }
                else
                {
                    string manifestUrl = downloadConnection.GetUrl(DLWorker.Config.ManifestId, DLWorker.Config.ProductId);

                    var manifestBytes = rc.DownloadData(new(manifestUrl));
                    if (manifestBytes == null)
                        throw new("Manifest not found?");

                    File.WriteAllBytes(DLWorker.Config.ProductManifest + ".manifest", manifestBytes);
                    parsedManifest = Parsers.ParseManifestFile(DLWorker.Config.ProductManifest + ".manifest");
                }
            }
            #endregion
            #region Game from Argument
            else
            {
                if (!Directory.Exists(DLWorker.Config.DownloadDirectory))
                {
                    Directory.CreateDirectory(DLWorker.Config.DownloadDirectory);
                }
                var ownershipToken = ownershipConnection.GetOwnershipToken(DLWorker.Config.ProductId);
                if (ownershipConnection.IsConnectionClosed == false || string.IsNullOrEmpty(ownershipToken.Item1)) { throw new("Product not owned"); }
                OWToken = ownershipToken.Item1;
                Exp = ownershipToken.Item2;
                Console.WriteLine($"Expires in {GetTimeFromEpoc(Exp)}");
                downloadConnection.InitDownloadToken(OWToken);
                if (manifest_path != "")
                {
                    File.Copy(manifest_path, DLWorker.Config.DownloadDirectory + "/uplay_install.manifest", true);
                    parsedManifest = Parsers.ParseManifestFile(manifest_path);
                }
                else
                {
                    string manifestUrl = downloadConnection.GetUrl(DLWorker.Config.ManifestId, DLWorker.Config.ProductId);

                    var manifestBytes = rc.DownloadData(new(manifestUrl));
                    if (manifestBytes == null)
                        throw new("Manifest not found?");

                    File.WriteAllBytes(DLWorker.Config.ProductManifest + ".manifest", manifestBytes);
                    File.Copy(DLWorker.Config.ProductManifest + ".manifest", DLWorker.Config.DownloadDirectory + "/uplay_install.manifest", true);
                    parsedManifest = Parsers.ParseManifestFile(DLWorker.Config.ProductManifest + ".manifest");
                }
            }
            #endregion
            #region Addons check
            if (hasAddons)
            {
                string LicenseURL = downloadConnection.GetUrl(DLWorker.Config.ManifestId, DLWorker.Config.ProductId, "license");
                var License = rc.DownloadData(new(LicenseURL));
                if (License == null)
                    throw new("License not found?");
                File.WriteAllBytes(DLWorker.Config.ProductManifest + ".license", License);

                string MetadataURL = downloadConnection.GetUrl(DLWorker.Config.ManifestId, DLWorker.Config.ProductId, "metadata");
                var Metadata = rc.DownloadData(new(MetadataURL));
                if (Metadata == null)
                    throw new("Metadata not found?");
                File.WriteAllBytes(DLWorker.Config.ProductManifest + ".metadata", Metadata);
            }
            rc.Dispose();
            #endregion
            #region Compression Print
            Console.WriteLine($"\nDownloaded and parsed manifest successfully:");
            Console.WriteLine($"Compression Method: {parsedManifest.CompressionMethod} IsCompressed? {parsedManifest.IsCompressed} Version {parsedManifest.Version}");
            #endregion
            #region Lang Chunks
            List<Uplay.Download.File> files = new();

            if (parsedManifest.Languages.ToList().Count > 0)
            {
                if (lang == "default")
                {
                    Console.WriteLine("Languages to use (just press enter to choose nothing, and all for all chunks)");
                    parsedManifest.Languages.ToList().ForEach(x => Console.WriteLine(x.Code));

                    var langchoosed = Console.ReadLine();

                    if (!string.IsNullOrEmpty(langchoosed))
                    {
                        if (langchoosed == "all")
                        {
                            files = ChunkManager.AllFiles(parsedManifest);
                        }
                        else
                        {
                            files.AddRange(ChunkManager.RemoveNonEnglish(parsedManifest));
                            lang = langchoosed;
                            files.AddRange(ChunkManager.AddLanguage(parsedManifest, lang));
                        }
                    }
                    else
                    {
                        files.AddRange(ChunkManager.RemoveNonEnglish(parsedManifest));

                    }
                }
                else if (lang == "all")
                {
                    files = ChunkManager.AllFiles(parsedManifest);
                }
                else
                {
                    files.AddRange(ChunkManager.RemoveNonEnglish(parsedManifest));
                    files.AddRange(ChunkManager.AddLanguage(parsedManifest, lang));
                }
            }
            else
            {
                files = ChunkManager.AllFiles(parsedManifest);
            }
            #endregion
            #region Skipping files from chunk
            DLWorker.Config.FilesToDownload = DLFile.FileNormalizer(files);
            List<string> skip_files = new();
            if (DLWorker.Config.UsingFileList)
            {
                if (File.Exists(skipping))
                {
                    var lines = File.ReadAllLines(skipping);
                    skip_files.AddRange(lines);
                    Console.WriteLine("Skipping files Added");
                }
                ChunkManager.RemoveSkipFiles(skip_files);
            }
            if (DLWorker.Config.UsingOnlyFileList)
            {
                if (File.Exists(onlygetting))
                {
                    var lines = File.ReadAllLines(onlygetting);
                    skip_files.AddRange(lines);
                    Console.WriteLine("Download only Added");
                }
                DLWorker.Config.FilesToDownload = ChunkManager.AddDLOnlyFiles(skip_files);
            }
            Console.WriteLine("\tFiles Ready to work\n");
            #endregion
            #region Saving
            Saving.Root saving = new();
            DLWorker.Config.VerifyBinPath = Path.Combine(DLWorker.Config.DownloadDirectory, ".UD\\verify.bin");
            Directory.CreateDirectory(Path.GetDirectoryName(DLWorker.Config.VerifyBinPath));
            if (File.Exists(Path.Combine(DLWorker.Config.DownloadDirectory, ".UD\\verify.bin.json")))
            {
                saving = JsonConvert.DeserializeObject<Saving.Root>(File.ReadAllText(Path.Combine(DLWorker.Config.DownloadDirectory, ".UD\\verify.bin.json")));
            }
            else if (File.Exists(DLWorker.Config.VerifyBinPath))
            {
                var readedBin = Saving.Read();
                if (readedBin == null)
                {
                    saving = Saving.MakeNew(DLWorker.Config.ProductId, DLWorker.Config.ManifestId, parsedManifest);
                }
                else
                {
                    saving = readedBin;
                }
            }
            else
            {
                saving = Saving.MakeNew(DLWorker.Config.ProductId, DLWorker.Config.ManifestId, parsedManifest);
            }
            if (hasVerifyPrint)
            {
                File.WriteAllText(DLWorker.Config.VerifyBinPath + ".json", JsonConvert.SerializeObject(saving));
                Console.ReadLine();
            }
            Saving.Save(saving);
            #endregion
            #region Verify + Downloading
            if (DLWorker.Config.Verify && !DLWorker.Config.DownloadAsChunks)
            {
                Verifier.Verify();
            }
            /*
            var resRoot = AutoRes.MakeNew(DLWorker.Config.ProductId, DLWorker.Config.ManifestId, DLWorker.Config.DownloadDirectory, DLWorker.Config.VerifyBinPath, Path.Combine(DLWorker.Config.DownloadDirectory, "uplay_install.manifest"));
            AutoRes.Save(resRoot);
            */
            DLWorker.DownloadWorker(downloadConnection);
            #endregion
            #region Closing and GoodBye
            Console.WriteLine("Goodbye!");
            Console.ReadLine();
            downloadConnection.Close();
            ownershipConnection.Close();
            socket.Disconnect();
            #endregion
        }
        #region Other Functions

        static void PrintHelp()
        {
            CoreLib.HelpArgs.PrintHelp();
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
            Console.WriteLine();
            Environment.Exit(0);
        }

        static DateTime GetTimeFromEpoc(ulong epoc)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddSeconds(epoc);
        }

        static ulong GetEpocTime()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (ulong)t.TotalSeconds;

        }

        public static void CheckOW(uint ProdId)
        {
            if (Exp <= GetEpocTime())
            {
                Console.WriteLine("Your token has no more valid, getting new!");
                if (ownershipConnection != null && !ownershipConnection.IsConnectionClosed)
                {
                    var token = ownershipConnection.GetOwnershipToken(ProdId);
                    Exp = token.Item2;
                    OWToken = token.Item1;
                    Console.WriteLine("Is Token get success? " + ownershipConnection.IsConnectionClosed + " " + (Exp != 0));

                }
            }
        }
        #endregion
    }
}
