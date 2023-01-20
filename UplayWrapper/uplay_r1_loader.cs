using System.Runtime.InteropServices;
using static UplayWrapper.Enums;
using static UplayWrapper.Structs;

namespace UplayWrapper
{
    public class uplay_r1_loader
    {
        public static string version = "4.0.0.2041";

        [DllImport("uplay_r1_loader_v4")]
        public static extern int UPLAY_Startup(uint aUplayId, uint aGameVersion, string aLanguageCountryCodeUtf8);

        [DllImport("uplay_r1_loader_v4")]
        public static extern int UPLAY_Quit();

        [DllImport("uplay_r1_loader_v4")]
        public static extern int UPLAY_ACH_EarnAchievement(uint aAchievementId, IntPtr aOverlapped);

        [DllImport("uplay_r1_loader_v4")]
        public static extern int UPLAY_SAVE_GetSavegames(out IntPtr UPLAY_SAVE_GameList, IntPtr aOverlapped);

        [DllImport("uplay_r1_loader_v4")]
        public static extern int UPLAY_SAVE_Open(uint aSlotId, UPLAY_SAVE_Mode aMode, out uint aOutSaveHandle, IntPtr aOverlapped);

        [DllImport("uplay_r1_loader_v4")]
        public static extern int UPLAY_SAVE_Close(uint aSaveHandle);

        [DllImport("uplay_r1_loader_v4")]
        public static extern int UPLAY_SAVE_Read(uint aSaveHandle, uint aNumOfBytesToRead, uint aOffset, IntPtr aOutBuffer, out uint aOutNumOfBytesRead, IntPtr aOverlapped);

        [DllImport("uplay_r1_loader_v4")]
        public static extern int UPLAY_SAVE_Write(uint aSaveHandle, uint aNumOfBytesToWrite, IntPtr aBuffer, IntPtr aOverlapped);

        [DllImport("uplay_r1_loader_v4")]
        public static extern int UPLAY_SAVE_SetName(uint aSaveHandle, [MarshalAs(UnmanagedType.LPStr)] string aNameUtf8);

        [DllImport("uplay_r1_loader_v4")]
        public static extern int UPLAY_SAVE_Remove(uint aSlotId, IntPtr aOverlapped);

        [DllImport("uplay_r1_loader_v4")]
        public static extern bool UPLAY_GetNextEvent(out UPLAY_Event aEvent);

        [DllImport("uplay_r1_loader_v4")]
        public static extern int UPLAY_Update();

        [DllImport("uplay_r1_loader_v4")]
        public static extern bool UPLAY_HasOverlappedOperationCompleted(IntPtr aOverlapped);

        [DllImport("uplay_r1_loader_v4")]
        public static extern bool UPLAY_GetOverlappedOperationResult(IntPtr aOverlapped, out UPLAY_OverlappedResult aOutResult);

        [DllImport("uplay_r1_loader_v4")]
        public static extern int UPLAY_OVERLAY_Show(UPLAY_OVERLAY_Section aOerlaySection, IntPtr aOverlapped);

        [DllImport("uplay_r1_loader_v4")]
        public static extern IntPtr UPLAY_USER_GetUsernameUtf8();

        [DllImport("uplay_r1_loader_v4")]
        public static extern IntPtr UPLAY_USER_GetPasswordUtf8();

        [DllImport("uplay_r1_loader_v4")]
        public static extern int UPLAY_USER_IsInOfflineMode();

        [DllImport("uplay_r1_loader_v4")]
        public static extern int UPLAY_GetLastError(ref string aOutErrorString);

        [DllImport("uplay_r1_loader_v4")]
        public static extern int UPLAY_USER_GetCdKeys(out IntPtr aOutCdKeyList, IntPtr aOverlapped);
    }
}
