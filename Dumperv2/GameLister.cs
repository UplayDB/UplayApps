using Newtonsoft.Json;
using static Dumperv2.Jsons;

namespace Dumperv2
{
    internal class GameLister
    {
        public static void Work(List<Uplay.Ownership.OwnedGame>? games)
        {
            if (games == null)
                return;

            List<OW> owList = new();
            if (File.Exists("gamelist.json"))
            {
                owList = JsonConvert.DeserializeObject<List<OW>>(File.ReadAllText("gamelist.json"));
                Console.WriteLine(owList.Count);
            }
            foreach (var game in games)
            {
                OW ow = new();
                ow.ProductId = game.ProductId;
                ow.ProductType = ((Uplay.Ownership.OwnedGame.Types.ProductType)game.ProductType).ToString();
                ow.State = ((Uplay.Ownership.OwnedGame.Types.State)game.State).ToString();
                ow.TargetPartner = game.TargetPartner.ToString();
                ow.ProductAssociations = game.ProductAssociations.ToList();
                ow.ActivationIds = game.ActivationIds.ToList();

                if (owList.FindAll(x => x.ProductId == game.ProductId).Count <= 0)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(ow));
                    owList.Add(ow);
                }

                var listdiffconf = owList.Where(x => x.ProductId == game.ProductId
                && x.ProductType != ((Uplay.Ownership.OwnedGame.Types.ProductType)game.ProductType).ToString()
                && x.State != ((Uplay.Ownership.OwnedGame.Types.State)game.State).ToString()
                && x.TargetPartner != game.TargetPartner.ToString()
                && x.ProductAssociations != game.ProductAssociations.ToList()
                && x.ActivationIds != game.ActivationIds.ToList()
                ).ToList();

                if (listdiffconf.Count > 0)
                {
                    var ows = owList.Where(x => x.ProductId == game.ProductId).First();
                    ows.ProductType = ((Uplay.Ownership.OwnedGame.Types.ProductType)game.ProductType).ToString();
                    ows.State = ((Uplay.Ownership.OwnedGame.Types.State)game.State).ToString();
                    ows.TargetPartner = game.TargetPartner.ToString();
                    ows.ProductAssociations = game.ProductAssociations.ToList();
                    ows.ActivationIds = game.ActivationIds.ToList();
                }
            }
            File.WriteAllText("gamelist.json", JsonConvert.SerializeObject(owList, Formatting.Indented));
        }
    }
}
