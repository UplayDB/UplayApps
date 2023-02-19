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
            foreach (var game in games)
            {
                Console.WriteLine($"{game.ProductId}={game.LatestManifest}");
                strlist.Add($"{game.ProductId}={game.LatestManifest}");
                prodIdList.Add(game.ProductId);

                string ownershipToken_1 = ownership.GetOwnershipToken(game.ProductId).Item1;
                downloadConnection.InitDownloadToken(ownershipToken_1);
                if (downloadConnection.isServiceSuccess != false)
                {
                    string manifestUrl_1 = downloadConnection.GetUrl(game.LatestManifest, game.ProductId);
                    var rc1 = new RestClient();
                    File.WriteAllBytes(currentDir + "\\files\\" + game.ProductId + "_" + game.LatestManifest + ".manifest", rc1.DownloadData(new(manifestUrl_1)));

                    var gm = $"{currentDir}\\files\\{game.ProductId}_{game.LatestManifest}.manifest";
                    Dumper.Dump(Parsers.ParseManifestFile(gm), gm.Replace(".manifest", ".txt"));
                    Dumper.DumpAsCSV(Parsers.ParseManifestFile(gm), null, gm.Replace(".manifest", ""), game.LatestManifest, game.ProductId);
                }
            }
            if (File.Exists(currentDir + "\\latest_manifests.txt"))
            {
                Console.WriteLine("file are here");
                var latest_manifests = File.ReadAllLines(currentDir + "\\latest_manifests.txt");
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
            File.WriteAllText(currentDir + "\\latest_manifests.txt", write_out);
        }
    }
}
