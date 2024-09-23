

using Newtonsoft.Json;
using static Dumperv2.Jsons;

namespace Dumperv2
{
    internal class ListAllManifests
    {
        public static void Work(string currentDir, Uplay.Ownership.OwnedGame[]? games)
        {
            if (games == null)
                return;
            List<prodmanifests> manifestList = new();
            if (File.Exists($"{currentDir}/manifestlist.json"))
            {
                var manifestListfile = JsonConvert.DeserializeObject<List<prodmanifests>>(File.ReadAllText($"{currentDir}/manifestlist.json"));
                if (manifestListfile != null)
                    manifestList = manifestListfile;
            }
            foreach (var item in games)
            {
                if (item == null)
                    continue;
                if (string.IsNullOrEmpty(item.LatestManifest.Trim()))
                    continue;
                var prodmanifest = manifestList.FirstOrDefault(x => x.ProductId == item.ProductId);
                if (prodmanifest == null)
                {
                    prodmanifest = new()
                    {
                        ProductId = item.ProductId,
                        Manifest = new()
                    };
                }
                if (!prodmanifest.Manifest.Contains(item.LatestManifest))
                    prodmanifest.Manifest.Add(item.LatestManifest);
                manifestList.Add(prodmanifest);
            }
            manifestList = manifestList.OrderBy(x => x.ProductId).ToList();
            File.WriteAllText($"{currentDir}/manifestlist.json", JsonConvert.SerializeObject(manifestList, Formatting.Indented));
        }
    }
}
