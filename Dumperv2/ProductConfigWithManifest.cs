using Newtonsoft.Json;
using static Dumperv2.Jsons;

namespace Dumperv2
{
    internal class ProductConfigWithManifest
    {
        public static void Work(string currentDir, Uplay.Ownership.OwnedGame[] games)
        {
            List<prodconfm> listconf = new();

            if (File.Exists(currentDir + "\\productconfigmanifest.json"))
            {
                listconf = JsonConvert.DeserializeObject<List<prodconfm>>(File.ReadAllText(currentDir + "\\productconfigmanifest.json"));
            }

            foreach (var g in games)
            {

                prodconfm prodconf = new()
                {
                    ProductId = g.ProductId,
                    Configuration = g.Configuration,
                    Manifest = g.LatestManifest
                };

                if (listconf.FindAll(x => x.ProductId == g.ProductId).Count <= 0)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(prodconf));
                    listconf.Add(prodconf);
                }
                var listdiffconf = listconf.Where(x => x.ProductId == g.ProductId &&
                (x.Configuration != g.Configuration ||
                 x.Manifest != g.LatestManifest
                )).ToList();

                if (listdiffconf.Count > 0)
                {
                    Console.WriteLine(listdiffconf.Count);
                    listconf.Where(x => x.ProductId == g.ProductId).First().Configuration = g.Configuration;
                }
            }
            listconf = listconf.OrderBy(x => x.ProductId).ToList();
            File.WriteAllText(currentDir + "\\productconfigmanifest.json", JsonConvert.SerializeObject(listconf, Formatting.Indented));
        }
    }
}
