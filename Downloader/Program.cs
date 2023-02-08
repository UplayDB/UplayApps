using Newtonsoft.Json;
using RestSharp;
using System.ComponentModel;
using CoreLib;
using System.Text.RegularExpressions;
using UbiServices.Records;
using UplayKit;
using UplayKit.Connection;
using static UbiServices.Public.V3;

namespace Downloader
{
    public class Program
    {
        public static string OWToken = "";
        public static ulong Exp = 0;
        public static string UbiTicket = "";
        public static string Session = "";
        public static OwnershipConnection? ownershipConnection = null;
        public static DemuxSocket? socket = null;
        public static DateTime UbiTicketExp = DateTime.MinValue;
        static void Main(string[] args)
        {
            if (ParameterLib.HasParameter(args, "-help") || ParameterLib.HasParameter(args, "-?") || ParameterLib.HasParameter(args, "?"))
            {
                PrintHelp();
            }
            #region Argument thingy
            
            bool haslocal = ParameterLib.HasParameter(args, "-local");
            Debug.isDebug = ParameterLib.HasParameter(args, "-debug");
            int waittime = ParameterLib.GetParameter(args, "-time", 5);
            Downloader.Config.ProductId = ParameterLib.GetParameter<uint>(args, "-product", 0);
            Downloader.Config.ManifestId = ParameterLib.GetParameter(args, "-manifest", "");
            string manifest_path = ParameterLib.GetParameter(args, "-manifestpath", "");
            bool hasAddons = ParameterLib.HasParameter(args, "-addons");
            string lang = ParameterLib.GetParameter(args, "-lang", "default");
            Downloader.Config.DownloadDirectory = ParameterLib.GetParameter(args, "-dir", $"{Directory.GetCurrentDirectory()}\\{Downloader.Config.ProductId}\\{Downloader.Config.ManifestId}\\");
            bool hasSkip = ParameterLib.HasParameter(args, "-skip");
            bool hasOnly = ParameterLib.HasParameter(args, "-only");
            string skipping = ParameterLib.GetParameter(args, "-skip", "skip.txt");
            string onlygetting = ParameterLib.GetParameter(args, "-only", "only.txt");
            bool hasVerify = ParameterLib.HasParameter(args, "-verify");
            bool hasSaved = ParameterLib.HasParameter(args, "-filetosaved");
            #endregion

            UbiServices.Urls.IsLocalTest = haslocal;
            #region Login
            LoginJson? login = LoginLib.TryLoginWithArgsCLI(args);
            // Last login check
            if (login == null)
            {
                Console.WriteLine("Login failed");
                Environment.Exit(1);
            }
            #endregion
            #region Starting Connections, Getting game
            UbiTicketExp = (DateTime)login.Expiration;
            
            socket = new(haslocal);
            socket.WaitInTimeMS = waittime;
            Console.WriteLine("Is same Version? " + socket.VersionCheck());
            socket.PushVersion();
            bool IsAuthSuccess = socket.Authenticate(login.Ticket);
            Console.WriteLine("Is Auth Success? " + IsAuthSuccess);
            if (!IsAuthSuccess)
            {
                Console.WriteLine("Oops something is wrong!");
                Environment.Exit(1);
            }

            ownershipConnection = new(socket);
            DownloadConnection downloadConnection = new(socket);
            var owned = ownershipConnection.GetOwnedGames(false);
            if (owned == null || owned.Count == 0)
            {
                Console.WriteLine("No games owned?!");
                Environment.Exit(1);
            }
                
            #endregion
            #region Printing games



            Uplay.Download.Manifest parsedManifest = new();
            RestClient rc = new();

            if (Downloader.Config.ProductId == 0 && Downloader.Config.ManifestId == "")
            {
                owned = owned.Where(game => game.LatestManifest.Trim().Length > 0).ToList();
                owned = owned.Where(game => game.ProductType == (uint)Uplay.Ownership.OwnedGame.Types.ProductType.Game).ToList();


                Console.WriteLine("-1) Your games:.");
                Console.WriteLine("----------------------");
                int gameIds = 0;
                foreach (var game in owned)
                {
                    Console.WriteLine($"\n({gameIds}) ProductId ({game.ProductId}) Manifest {game.LatestManifest}");
                    gameIds++;
                }
                Console.WriteLine("Please select:");
                Console.ReadLine();

                int selection = int.Parse(Console.ReadLine()!);
                bool manifestfile = false;
                if (selection == -1)
                {
                    Console.WriteLine("> Input the 20-byte long manifest identifier:");
                    Downloader.Config.ManifestId = Console.ReadLine()!.Trim();

                    if (Downloader.Config.ManifestId.Contains(".manifest")) { manifestfile = true; }

                    Console.WriteLine("> Input the productId:");
                    Downloader.Config.ProductId = uint.Parse(Console.ReadLine()!.Trim());
                }
                else if (selection <= gameIds)
                {
                    Downloader.Config.ManifestId = owned[selection].LatestManifest;
                    Downloader.Config.ProductId = owned[selection].ProductId;
                }



                if (manifestfile)
                {
                    parsedManifest = Parsers.ParseManifestFile(Downloader.Config.ManifestId);
                    var ownershipToken = ownershipConnection.GetOwnershipToken(Downloader.Config.ProductId);
                    if (ownershipConnection.isServiceSuccess == false) { throw new("Product not owned"); }
                    OWToken = ownershipToken.Item1;
                    Exp = ownershipToken.Item2;
                    Console.WriteLine($"Expires in {GetTimeFromEpoc(Exp)}");
                    downloadConnection.InitDownloadToken(OWToken);
                }
                else
                {
                    var ownershipToken = ownershipConnection.GetOwnershipToken(Downloader.Config.ProductId);
                    if (ownershipConnection.isServiceSuccess == false) { throw new("Product not owned"); }
                    OWToken = ownershipToken.Item1;
                    Exp = ownershipToken.Item2;
                    Console.WriteLine($"Expires in {GetTimeFromEpoc(Exp)}");
                    downloadConnection.InitDownloadToken(OWToken);
                    string manifestUrl = downloadConnection.GetUrl(Downloader.Config.ManifestId, Downloader.Config.ProductId);

                    var manifestBytes = rc.DownloadData(new(manifestUrl));
                    if (manifestBytes == null)
                        throw new("Manifest not found?");

                    File.WriteAllBytes(Downloader.Config.ProductManifest + ".manifest", manifestBytes);
                    parsedManifest = Parsers.ParseManifestFile(Downloader.Config.ProductManifest + ".manifest");
                }

            }
            else
            {
                var ownershipToken = ownershipConnection.GetOwnershipToken(Downloader.Config.ProductId);
                if (ownershipConnection.isServiceSuccess == false) { throw new("Product not owned"); }
                OWToken = ownershipToken.Item1;
                Exp = ownershipToken.Item2;
                Console.WriteLine($"Expires in {GetTimeFromEpoc(Exp)}");
                downloadConnection.InitDownloadToken(OWToken);

                if (manifest_path != "")
                {
                    parsedManifest = Parsers.ParseManifestFile(manifest_path);
                }
                else
                {
                    string manifestUrl = downloadConnection.GetUrl(Downloader.Config.ManifestId, Downloader.Config.ProductId);

                    var manifestBytes = rc.DownloadData(new(manifestUrl));
                    if (manifestBytes == null)
                        throw new("Manifest not found?");

                    File.WriteAllBytes(Downloader.Config.ProductManifest + ".manifest", manifestBytes);
                    parsedManifest = Parsers.ParseManifestFile(Downloader.Config.ProductManifest + ".manifest");
                }
            }

            if (hasAddons)
            {
                string LicenseURL = downloadConnection.GetUrl(Downloader.Config.ManifestId, Downloader.Config.ProductId, "license");
                var License = rc.DownloadData(new(LicenseURL));
                if (License == null)
                    throw new("License not found?");
                File.WriteAllBytes(Downloader.Config.ProductManifest + ".license", License);

                string MetadataURL = downloadConnection.GetUrl(Downloader.Config.ManifestId, Downloader.Config.ProductId, "metadata");
                var Metadata = rc.DownloadData(new(MetadataURL));
                if (Metadata == null)
                    throw new("Metadata not found?");
                File.WriteAllBytes(Downloader.Config.ProductManifest + ".metadata", Metadata);
            }
            rc.Dispose();
            #endregion
            #region Compression Print
            Console.WriteLine($"\nDownloaded and parsed manifest successfully:");
            Console.WriteLine($"Compression Method: {parsedManifest.CompressionMethod} IsCompressed? {parsedManifest.IsCompressed} Version {parsedManifest.Version}");
            #endregion
            #region Lang Chunks
            List<Uplay.Download.File> files = new();

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
            #endregion
            #region Skipping files from chunk
            List<string> skip_files = new();

            if (hasSkip)
            {
                if (File.Exists(skipping))
                {
                    var lines = File.ReadAllLines(skipping);
                    skip_files.AddRange(lines);
                    Console.WriteLine("Skipping files Added");
                }
                files = ChunkManager.RemoveSkipFiles(files, skip_files);
            }
            if (hasOnly)
            {
                if (File.Exists(onlygetting))
                {
                    var lines = File.ReadAllLines(onlygetting);
                    skip_files.AddRange(lines);
                    Console.WriteLine("Download only Added");
                }
                files = ChunkManager.AddDLOnlyFiles(files, skip_files);
            }
            #endregion
            #region Get Path and Create
            Console.WriteLine("\tFiles Ready to work\n");     
            if (!Directory.Exists(Downloader.Config.DownloadDirectory))
            {
                Directory.CreateDirectory(Downloader.Config.DownloadDirectory);
            }
            #endregion
            #region Saving
            Saving.Root saving = new();
            Downloader.Config.SavedDirectory = Path.Combine(Downloader.Config.DownloadDirectory, ".UD\\saved.bin");
            Directory.CreateDirectory(Path.GetDirectoryName(Downloader.Config.SavedDirectory));
            if (File.Exists(Downloader.Config.SavedDirectory))
            {
                var readedBin = Saving.Read();
                if (readedBin == null)
                {
                    saving = Saving.MakeNew(Downloader.Config.ProductId, Downloader.Config.ManifestId, parsedManifest);
                }
                else
                {
                    saving = readedBin;
                }
            }
            else
            {
                saving = Saving.MakeNew(Downloader.Config.ProductId, Downloader.Config.ManifestId, parsedManifest);
            }
            if (hasSaved)
            {
                File.WriteAllText(Downloader.Config.SavedDirectory + ".json", JsonConvert.SerializeObject(saving));
                Console.ReadLine();
            }
            Saving.Save(saving);
            #endregion
            #region Verify + Downloading
            if (hasVerify)
            {
                files = Verifier.Verify(files, saving);
            }
            Console.ReadLine();
            Downloader.DownloadWorker(files, downloadConnection, saving);
            #endregion
            #region Closing and GoodBye
            File.Copy(Downloader.Config.ProductManifest + ".manifest", Downloader.Config.DownloadDirectory + "uplay_install.manifest");
            Console.WriteLine("Goodbye!");
            Console.ReadLine();
            downloadConnection.Close();
            ownershipConnection.Close();
            socket.Close();
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
            Console.WriteLine("\t -product\t\t -");
            Console.WriteLine("\t -manifest\t\t -");
            Console.WriteLine("\t -manifestpath\t\t -");
            Console.WriteLine("\t -lang\t\t\t -");
            Console.WriteLine("\t -skip\t\t\t -");
            Console.WriteLine("\t -only\t\t\t -");
            Console.WriteLine("\t -dir\t\t\t -");
            Console.WriteLine("\t -filetosaved\t\t -");
            Console.WriteLine("\t -verify\t\t -");
            Console.WriteLine();
            Environment.Exit(0);
        }

        public static void UbiTicketReNew()
        {
            if (ToUnixTime(UbiTicketExp) <= GetEpocTime())
            {
                Console.WriteLine("UbiTicketReNew " + ToUnixTime(UbiTicketExp) + "<=" + GetEpocTime());
                
                var renewed = LoginRenew(UbiTicket, Session);

                if (renewed.Ticket != null)
                {
                    UbiTicket = renewed.Ticket;
                    Session = renewed.SessionId;
                    UbiTicketExp = (DateTime)renewed.Expiration;
                    bool authed = socket.Authenticate(renewed.Ticket);
                    Console.WriteLine("Renewed and Authed? " + authed);
                }
            }
        }

        static DateTime GetTimeFromEpoc(ulong epoc)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddSeconds(epoc);
        }

        static ulong GetEpocTime()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return (ulong)t.TotalSeconds;

        }
        public static ulong ToUnixTime(DateTime dateTime)
        {
            DateTimeOffset dto = new DateTimeOffset(dateTime);
            return (ulong)dto.ToUnixTimeSeconds();
        }

        public static void CheckOW(uint ProdId)
        {
            if (Exp <= GetEpocTime())
            {
                Console.WriteLine("CheckOW " + Exp + "<=" + GetEpocTime());
                Console.WriteLine("Your token has no more valid, getting new!");
                if (ownershipConnection != null && !ownershipConnection.isConnectionClosed)
                {
                    var token = ownershipConnection.GetOwnershipToken(ProdId);
                    Console.WriteLine("Is Token get success? " + ownershipConnection.isServiceSuccess);
                    Exp = token.Item2;
                    Console.WriteLine("New exp: " + Exp);
                    OWToken = token.Item1;
                }
            }
        }
        #endregion
    }
}