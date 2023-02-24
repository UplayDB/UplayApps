using Newtonsoft.Json;
using static Dumperv2.Jsons;

namespace Dumperv2
{
    internal class ProductConfig
    {
        public static void Work(string currentDir, Uplay.Ownership.OwnedGame[] games)
        {
            List<prodconf> listconf = new();

            if (File.Exists(currentDir + "\\productconfig.json"))
            {
                listconf = JsonConvert.DeserializeObject<List<prodconf>>(File.ReadAllText(currentDir + "\\productconfig.json"));
            }

            foreach (var g in games)
            {
                prodconf prodconf = new()
                {
                    ProductId = g.ProductId,
                    Configuration = g.Configuration
                };

                if (listconf.FindAll(x => x.ProductId == g.ProductId).Count <= 0)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(prodconf));
                    listconf.Add(prodconf);
                }
                var listdiffconf = listconf.Where(x => x.ProductId == g.ProductId && x.Configuration != g.Configuration).ToList();

                if (listdiffconf.Count > 0)
                {
                    Console.WriteLine(listdiffconf.Count);
                    listconf.Where(x => x.ProductId == g.ProductId).First().Configuration = g.Configuration;
                }
            }
            File.WriteAllText(currentDir + "\\productconfig.json", JsonConvert.SerializeObject(listconf, Formatting.Indented));
        }
    }
}
