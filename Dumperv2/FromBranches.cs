namespace Dumperv2;

internal class FromBranches
{
    public class pb
    {
        public uint Product;
        public uint branch;
    }

    public static List<pb> Work(string currentDir, Uplay.Ownership.OwnedGame[]? games)
    {
        if (games == null)
            return new();
        List<pb> pb = new();
        Console.WriteLine("FromBranches.Work!");
        foreach (var game in games)
        {
            Console.WriteLine("FromBranches.Work! " + game.ProductId);
            if (game.AvailableBranches.Count > 1)
            {
                foreach (var productBranch in game.AvailableBranches)
                {
                    if (productBranch.BranchId == game.ActiveBranchId)
                        continue;
                    Console.WriteLine($"{game.ProductId} _ {productBranch.BranchId}");
                    pb.Add( new FromBranches.pb() { Product = game.ProductId, branch = game.BrandId } );
                }
            }
        }
        File.WriteAllText("product_branch.txt", Newtonsoft.Json.JsonConvert.SerializeObject(pb));
        return pb;
    }
}
