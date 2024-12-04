using Newtonsoft.Json;
using static Dumperv2.Jsons;

namespace Dumperv2
{
    internal class ProductUbiService
    {
        public static void Work(string curDir, Uplay.Ownership.OwnedGame[] games)
        {
            List<prodserv> listconf = new();
            string productservice_path = Path.Combine(curDir, "productservice.json");
            if (File.Exists(productservice_path))
            {
                listconf = JsonConvert.DeserializeObject<List<prodserv>>(File.ReadAllText(productservice_path))!;
                Console.WriteLine($"ProductUbiService list item number is {listconf.Count}");
            }

            foreach (var g in games)
            {
                if (!g.HasUbiservicesAppId || !UbiServices.Validations.IdValidation(g.UbiservicesAppId))
                    continue;

                if (!g.HasUbiservicesSpaceId || !UbiServices.Validations.IdValidation(g.UbiservicesSpaceId))
                    continue;

                prodserv prodconf = new()
                {
                    ProductId = g.ProductId,
                    AppId = g.UbiservicesAppId,
                    SpaceId = g.UbiservicesSpaceId
                };

                if (listconf.FindAll(x => x.ProductId == g.ProductId).Count <= 0)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(prodconf));
                    listconf.Add(prodconf);
                }
                var listdiffconf = listconf.Where(x => x.ProductId == g.ProductId && (x.AppId != g.UbiservicesAppId || x.SpaceId != g.UbiservicesSpaceId)).ToList();

                if (listdiffconf.Count > 0)
                {
                    Console.WriteLine(listdiffconf.Count);
                    var x = listconf.Where(x => x.ProductId == g.ProductId).First();
                    x.SpaceId = g.UbiservicesSpaceId;
                    x.AppId = g.UbiservicesAppId;
                    Console.WriteLine(JsonConvert.SerializeObject(x));
                }
            }
            listconf = listconf.OrderBy(x => x.ProductId).ToList();
            File.WriteAllText(productservice_path, JsonConvert.SerializeObject(listconf, Formatting.Indented));
        }
    }
}
