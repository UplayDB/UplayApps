using Newtonsoft.Json;
using static Dumperv2.Jsons;

namespace Dumperv2
{
    internal class GameLister
    {
        public static void Work(string curDir, List<Uplay.Ownership.OwnedGame>? games)
        {
            if (games == null)
                return;

            List<OW> owList = new();
            if (File.Exists(Path.Combine(curDir, "gamelist.json")))
            {
                owList = JsonConvert.DeserializeObject<List<OW>>(File.ReadAllText(Path.Combine(curDir, "gamelist.json")))!;
            }
            foreach (var game in games)
            {
                OW ow = new();
                ow.ProductId = game.ProductId;
                ow.ProductType = ((Uplay.Ownership.OwnedGame.Types.ProductType)game.ProductType).ToString();
                ow.ProductAssociations = game.ProductAssociations.ToList();

                if (owList.FindAll(x => x.ProductId == game.ProductId).Count <= 0)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(ow));
                    owList.Add(ow);
                }

                var listdiffconf = owList.Where(x => x.ProductId == game.ProductId
                && (x.ProductType != ((Uplay.Ownership.OwnedGame.Types.ProductType)game.ProductType).ToString()
                || x.ProductAssociations != game.ProductAssociations.ToList()
                )).ToList();

                if (listdiffconf.Count > 0)
                {
                    var ows = owList.Where(x => x.ProductId == game.ProductId).First();
                    ows.ProductType = ((Uplay.Ownership.OwnedGame.Types.ProductType)game.ProductType).ToString();
                    ows.ProductAssociations = game.ProductAssociations.ToList();
                }
            }


            File.WriteAllText(Path.Combine(curDir, "gamelist.json"), JsonConvert.SerializeObject(owList, Formatting.Indented));
        }
    }
}
