using UplayKit.Connection;

namespace Dumperv2
{
    internal class FromBranches
    {
        public static void Work(string currentDir, Uplay.Ownership.OwnedGame[]? games, DownloadConnection downloadConnection, OwnershipConnection ownership)
        {
            foreach (var game in games)
            {
                if (game.AvailableBranches.Count > 1)
                {
                    foreach (var productBranch in game.AvailableBranches)
                    {
                        if (productBranch.BranchId == game.ActiveBranchId)
                            continue;

                        var branch = ownership.SwitchProductBranch(game.ProductId, productBranch.BranchId, null);
                        if (branch != null)
                        {
                            File.WriteAllText($"{game.ProductId}_{productBranch.BranchId}.ownership.txt", branch.ToString());
                        }
                    }
                }
            }
        }
    }
}
