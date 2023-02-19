using RestSharp;
using UplayKit.Connection;

namespace Dumperv2
{
    internal class ReDL
    {
        public static void Work(string currentDir, DownloadConnection downloadConnection, OwnershipConnection ownership)
        {
            if (!File.Exists("todl.txt"))
                return;

            //manifestId=productId

            var todl = File.ReadAllLines("todl.txt");

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
        }
    }
}
