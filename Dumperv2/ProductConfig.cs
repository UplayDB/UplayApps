using Newtonsoft.Json;
using static Dumperv2.Jsons;

namespace Dumperv2
{
    internal class ProductConfig
    {
        public static void Work(string currentDir, Uplay.Ownership.OwnedGame[] games)
        {
            List<prodconf> listconf = new();
            string productconfig_path = Path.Combine(currentDir, "productconfig.json");
            if (File.Exists(productconfig_path))
            {
                listconf = JsonConvert.DeserializeObject<List<prodconf>>(File.ReadAllText(productconfig_path))!;
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
                    Console.WriteLine(JsonConvert.SerializeObject(listconf.Where(x => x.ProductId == g.ProductId).First()));
                }
            }
            listconf = listconf.OrderBy(x => x.ProductId).ToList();
            File.WriteAllText(productconfig_path, JsonConvert.SerializeObject(listconf, Formatting.Indented));
        }
    }
}
