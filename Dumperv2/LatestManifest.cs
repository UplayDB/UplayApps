using RestSharp;
using UplayKit;
using UplayKit.Connection;

namespace Dumperv2
{
    internal class LatestManifest
    {
        public static void Work(string currentDir, Uplay.Ownership.OwnedGame[]? games, DownloadConnection downloadConnection, OwnershipConnection ownership)
        {
            List<string> strlist = new();
            List<uint> prodIdList = new();
            if (games == null)
                return;
            foreach (var game in games)
            {
                Console.WriteLine($"{game.ProductId}={game.LatestManifest}");
                strlist.Add($"{game.ProductId}={game.LatestManifest}");
                prodIdList.Add(game.ProductId);
                string ownershipToken_1 = ownership.GetOwnershipToken(game.ProductId).Item1;
                bool initDone = downloadConnection.InitDownloadToken(ownershipToken_1);
                if (initDone)
                {
                    Console.WriteLine("can download game");
                    var rc1 = new RestClient();
                    var urls = downloadConnection.GetUrlList(game.ProductId, new() { $"manifests/{game.LatestManifest}.manifest" });
                    foreach (var item in urls)
                    {
                        foreach (var item1 in item.Urls)
                        {
                            if (string.IsNullOrEmpty(item1))
                                continue;
                            Console.WriteLine(item1);
                            var data = rc1.DownloadData(new RestRequest(item1));
                            if (data == null)
                            {
                                Console.WriteLine("data is null! (if we have more we try again)");
                            }
                            if (data != null)
                            {
                                var gm = Path.Combine(currentDir, "files", game.ProductId + "_" + game.LatestManifest + ".manifest");
                                File.WriteAllBytes(gm, data);
                                Dumper.Dump(Parsers.ParseManifestFile(gm), gm.Replace(".manifest", ".txt"));
                                Dumper.DumpAsCSV(Parsers.ParseManifestFile(gm), null, gm.Replace(".manifest", ""), game.LatestManifest, game.ProductId);
                                break;
                            }
                        }  
                    }
                }
                Thread.Sleep(100);
            }
            if (File.Exists(Path.Combine(currentDir, "latest_manifests.txt")))
            {
                var latest_manifests = File.ReadAllLines(Path.Combine(currentDir, "latest_manifests.txt"));
                foreach (var latest_manifest in latest_manifests)
                {
                    if (!strlist.Contains(latest_manifest))
                    {
                        if (!prodIdList.Contains(uint.Parse(latest_manifest.Split("=")[0])))
                        {
                            strlist.Add(latest_manifest);
                        }
                        else
                        {
                            Console.WriteLine(latest_manifest + " Probably new manifest!");
                        }

                    }
                }
            }
            strlist.Sort();
            string write_out = "";
            foreach (var st in strlist)
            {
                write_out += st + "\n";
            }
            File.WriteAllText(Path.Combine(currentDir, "latest_manifests.txt"), write_out);
        }
    }
}
