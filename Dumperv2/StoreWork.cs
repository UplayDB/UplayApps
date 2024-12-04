using Newtonsoft.Json;
using Uplay.Store;
using static Dumperv2.Jsons;

namespace Dumperv2
{
    internal class StoreWork
    {
        public static void Work(GetStoreRsp store)
        {
            List<storeconf> storelist = new();

            if (File.Exists("storeref.json"))
            {
                storelist = JsonConvert.DeserializeObject<List<storeconf>>(File.ReadAllText("storeref.json"))!;
            }
            var storersp = store.StoreProducts.OrderBy(x => x.ProductId).ToList();
            foreach (var storeprod in storersp)
            {
                if (storeprod.StoreReference.Trim().Length == 0)
                {
                    continue;
                }
                Console.WriteLine("Working on " + storeprod.StoreReference);
                storeconf storeconf_ = new()
                {
                    ProductId = storeprod.ProductId,
                    StoreRef = storeprod.StoreReference,
                    Partner = storeprod.StorePartner
                };

                if (storelist.FindAll(x => x.ProductId == storeprod.ProductId).Count <= 0)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(storeconf_));
                    storelist.Add(storeconf_);
                }

                var listdiffconf = storelist.Where(x => x.ProductId == storeprod.ProductId && x.StoreRef != storeprod.StoreReference).ToList();

                if (listdiffconf.Count > 0)
                {
                    storelist.Where(x => x.ProductId == storeprod.ProductId).First().StoreRef = storeprod.StoreReference;
                }

            }
            storelist = storelist.OrderBy(x => x.ProductId).ToList();
            File.WriteAllText("storeref.json", JsonConvert.SerializeObject(storelist, Formatting.Indented));
        }
    }
}
