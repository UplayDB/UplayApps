

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
            string manifestlist_path = Path.Combine(currentDir, "manifestlist.json");
            if (File.Exists(manifestlist_path))
            {
                var manifestListfile = JsonConvert.DeserializeObject<List<prodmanifests>>(File.ReadAllText(manifestlist_path));
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
                if (!manifestList.Contains(prodmanifest))
                    manifestList.Add(prodmanifest);
            }
            manifestList = manifestList.OrderBy(x => x.ProductId).ToList();
            File.WriteAllText(manifestlist_path, JsonConvert.SerializeObject(manifestList, Formatting.Indented));
        }
    }
}
