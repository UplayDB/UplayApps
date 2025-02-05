using UbiServices.Records;
using Uplay.Uplay;
using UplayKit;
using UplayKit.Connection;

namespace AchiSync;

internal class AchCon
{
    string? UserId;
    AchievementConnection? ach_con;
    public void ConnectAchi(DemuxSocket demux, string token, string userId)
    {
        UserId = userId;
        ach_con = new AchievementConnection(demux);
        //ach_con.Auth(token);
    }

    public List<ProductAchievements> GetProductAchievements(string? prodId)
    {
        if (ach_con == null)
            return [];
        if (ach_con.IsConnectionClosed)
            return [];
        if (string.IsNullOrEmpty(prodId))
            return [];
        if (string.IsNullOrEmpty(UserId))
            return [];
        return ach_con.GetAchievements(UserId, prodId, "PC");
    }
}
