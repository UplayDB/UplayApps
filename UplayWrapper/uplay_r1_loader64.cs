using System.Runtime.InteropServices;
using System.Text;
using static UplayWrapper.Enums;
using static UplayWrapper.Structs;

namespace UplayWrapper
{
    public class uplay_r1_loader64
    {
        [DllImport("uplay_r1_loader64")]
        public static extern UplayStartResult UPLAY_Start(uint aUplayId, UplayStartFlags aFlags);

        [DllImport("uplay_r1_loader64")]
        public static extern int UPLAY_Quit();

        [DllImport("uplay_r1_loader64")]
        public static extern int UPLAY_GetLastError([MarshalAs(UnmanagedType.LPStr)][Out] StringBuilder aOutErrorString);

        [DllImport("uplay_r1_loader64")]
        public static extern IntPtr UPLAY_USER_GetTicketUtf8();

        [DllImport("uplay_r1_loader64")]
        public static extern byte UPLAY_GetNextEvent(out UPLAY_Event aEvent);

        [DllImport("uplay_r1_loader64")]
        public static extern int UPLAY_USER_IsInOfflineMode();

        [DllImport("uplay_r1_loader64")]
        public static extern IntPtr UPLAY_USER_GetAccountIdUtf8();

        [DllImport("uplay_r1_loader64")]
        public static extern int UPLAY_Update();

        [DllImport("uplay_r1_loader64")]
        public static extern int UPLAY_ACH_EarnAchievement(uint aAchivementId, ref UPLAY_Overlapped aOverlapped);

        [DllImport("uplay_r1_loader64")]
        public static extern int UPLAY_OVERLAY_Show(uint aOverlaySection, ref UPLAY_Overlapped aOverlapped);

        [DllImport("uplay_r1_loader64")]
        public static extern int UPLAY_GetOverlappedOperationResult(ref UPLAY_Overlapped aOverlapped, out int result);

        [DllImport("uplay_r1_loader64")]
        public static extern sbyte UPLAY_HasOverlappedOperationCompleted(ref UPLAY_Overlapped aOverlapped);

        [DllImport("uplay_r1_loader64")]
        public static extern int UPLAY_USER_IsOwned(uint aUplayId);

        [DllImport("uplay_r1_loader64")]
        public static extern int UPLAY_PRESENCE_SetPresence(uint presenceId, ref UPLAY_PRESENCE_TokenList tokens);
    }
}
