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

                if (File.Exists(Path.Combine(currentDir, "files", prod + "_" + manifest + ".manifest")))
                {
                    Console.WriteLine("Skipped!");
                }
                else
                {
                    uint prodId = uint.Parse(prod);
                    string ownershipToken_1 = ownership.GetOwnershipToken(prodId).Item1;
                    bool IsSuccess = downloadConnection.InitDownloadToken(ownershipToken_1);
                    if (IsSuccess != false)
                    {
                        var manifestUrls = downloadConnection.GetUrl(manifest, prodId);
                        foreach (var url in manifestUrls)
                        {
                            if (string.IsNullOrEmpty(url))
                                continue;
                            Console.WriteLine("Manifest dl!");
                            var rc1 = new RestClient();
                            var data = rc1.DownloadData(new(url));
                            if (data == null)
                            {
                                Console.WriteLine("Manifest dl failed! Url return nothing. (If we have more we try to download from the other!)\nURL: " + url);
                                continue;
                            }
                            else
                            {
                                File.WriteAllBytes(Path.Combine(currentDir, "files", prod + "_" + manifest + ".manifest"), data);
                                break;
                            } 
                        }
                    }
                    Console.WriteLine(manifest + " " + prod + " " + i);
                }

            }
        }
    }
}
