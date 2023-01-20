using System.Runtime.InteropServices;
using static UplayWrapper.Enums;

namespace UplayWrapper
{
    public class upc_r2_loader64
    {
        public static string version = "128.0.10632.0";

        [DllImport("upc_r2_loader64", EntryPoint = "UPC_AchievementImageFree")]
        public static extern int UPC_AchievementImageFreeImpl(IntPtr inContext, long inImageRGBA);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_AchievementImageGet")]
        public static extern int UPC_AchievementImageGetImpl(IntPtr inContext, uint inId, ref long outImageRGBA, IntPtr inCallback, IntPtr inCallbackData);

        [DllImport("upc_r2_loader64", EntryPoint = "UPC_AchievementListFree")]
        public static extern int UPC_AchievementListFreeImpl(IntPtr inContext, long inAchievementList);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_AchievementListGet")]
        public static extern int UPC_AchievementListGetImpl(IntPtr inContext, [MarshalAs(UnmanagedType.LPStr)] string inOptUserIdUtf8, uint inFilter, ref long outAchievementList, IntPtr inCallback, IntPtr inCallbackData);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_AchievementUnlock")]
        public static extern int UPC_AchievementUnlockImpl(IntPtr inContext, uint inId, IntPtr inOptCallback, IntPtr inOptCallbackData);

        [DllImport("upc_r2_loader64", EntryPoint = "UPC_AvatarFree")]
        public static extern int UPC_AvatarFreeImpl(IntPtr inContext, long inImageRGBA);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_AvatarGet")]
        public static extern int UPC_AvatarGetImpl(IntPtr inContext, [MarshalAs(UnmanagedType.LPStr)] string inOptUserIdUtf8, uint inSize, ref long outImageRGBA, IntPtr inCallback, IntPtr inCallbackData);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_ApplicationIdGet")]
        public static extern UPC_Result UPC_ApplicationIdGetImpl(IntPtr inContext, ref IntPtr appID);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_BlacklistAdd")]
        public static extern int UPC_BlacklistAddImpl(IntPtr inContext, [MarshalAs(UnmanagedType.LPStr)] string inUserIdUtf8, IntPtr inOptCallback, IntPtr inOptCallbackData);

        [DllImport("upc_r2_loader64", EntryPoint = "UPC_BlacklistHas")]
        public static extern int UPC_BlacklistHasImpl(IntPtr inContext, [MarshalAs(UnmanagedType.LPStr)] string inUserIdUtf8);

        [DllImport("upc_r2_loader64", EntryPoint = "UPC_Cancel")]
        public static extern UPC_Result UPC_CancelImpl(IntPtr inContext, int inHandle);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_ContextCreate")]
        public static extern IntPtr UPC_ContextCreateImpl(uint inVersion, ref Structs.UPC_ContextSettings inOptSettings);

        [DllImport("upc_r2_loader64", EntryPoint = "UPC_ContextFree")]
        public static extern UPC_Result UPC_ContextFreeImpl(IntPtr inContext);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_CPUScoreGet")]
        public static extern UPC_Result UPC_CPUScoreGetImpl(IntPtr inContext, out uint outScore);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_EmailGet")]
        public static extern IntPtr UPC_EmailGetImpl(IntPtr inContext);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "UPC_ErrorToString")]
        public static extern IntPtr UPC_ErrorToStringImpl(int inError);

        [DllImport("upc_r2_loader64", EntryPoint = "UPC_EventNextPeek")]
        public static extern UPC_Result UPC_EventNextPeekImpl(IntPtr inContext, IntPtr outEvent);

        [DllImport("upc_r2_loader64", EntryPoint = "UPC_EventNextPoll")]
        public static extern UPC_Result UPC_EventNextPollImpl(IntPtr inContext, IntPtr outEvent);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_EventRegisterHandler")]
        public static extern UPC_Result UPC_EventRegisterHandlerImpl(IntPtr inContext, UPC_EventType inType, IntPtr inHandler, IntPtr inOptData);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_FriendAdd")]
        public static extern int UPC_FriendAddImpl(IntPtr inContext, [MarshalAs(UnmanagedType.LPStr)] string inSearchStringUtf8, IntPtr inOptCallback, IntPtr inOptCallbackData);

        [DllImport("upc_r2_loader64", EntryPoint = "UPC_FriendCheck")]
        public static extern int UPC_FriendCheckImpl(IntPtr inContext, [MarshalAs(UnmanagedType.LPStr)] string inUserIdUtf8);

        [DllImport("upc_r2_loader64", EntryPoint = "UPC_FriendListFree")]
        public static extern int UPC_FriendListFreeImpl(IntPtr inContext, long inFriendList);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_FriendListGet")]
        public static extern int UPC_FriendListGetImpl(IntPtr inContext, uint inOptOnlineStatusFilter, ref long outFriendList, IntPtr inCallback, IntPtr inOptCallbackData);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_FriendRemove")]
        public static extern int UPC_FriendRemoveImpl(IntPtr inContext, [MarshalAs(UnmanagedType.LPStr)] string inUserIdUtf8, IntPtr inOptCallback, IntPtr inOptCallbackData);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_GPUScoreGet")]
        public static extern UPC_Result UPC_GPUScoreGetImpl(IntPtr inContext, out uint outScore, out float outConfidenceLevel);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_IdGet")]
        public static extern IntPtr UPC_IdGetImpl(IntPtr inContext);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_IdGet_Extended")]
        public static extern UPC_Result UPC_IdGet_ExtendedImpl(IntPtr inContext, ref string id);

        [DllImport("upc_r2_loader64", EntryPoint = "UPC_Init")]
        public static extern UPC_InitResult UPC_InitImpl(uint inVersion, uint inProductId);

        [DllImport("upc_r2_loader64", EntryPoint = "UPC_InstallChunkListFree")]
        private static extern int UPC_InstallChunkListFree(IntPtr inContext, IntPtr inChunkList);

        [DllImport("upc_r2_loader64", EntryPoint = "UPC_InstallChunkListGet")]
        private static extern int UPC_InstallChunkListGetImpl(IntPtr inContext, ref IntPtr outChunkList);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "UPC_InstallLanguageGet")]
        public static extern string UPC_InstallLanguageGetImpl(IntPtr inContext);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_MultiplayerInviteAnswer")]
        public static extern int UPC_MultiplayerInviteAnswerImpl(IntPtr inContext, [MarshalAs(UnmanagedType.LPUTF8Str)] string inSenderIdUtf8, int inIsAccepted, IntPtr inOptCallback, IntPtr inOptCallbackData);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_MultiplayerInvite")]
        public static extern int UPC_MultiplayerInviteImpl(IntPtr inContext, [MarshalAs(UnmanagedType.LPUTF8Str)] string inUserIdUtf8, IntPtr inOptCallback, IntPtr inOptCallbackData);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_MultiplayerSessionClear")]
        public static extern UPC_Result UPC_MultiplayerSessionClearImpl(IntPtr inContext);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_MultiplayerSessionFree")]
        public static extern int UPC_MultiplayerSessionFreeImpl(IntPtr inContext, IntPtr inMultiplayerSession);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_MultiplayerSessionGet")]
        public static extern int UPC_MultiplayerSessionGetImpl(IntPtr inContext, ref IntPtr outMultiplayerSession);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_MultiplayerSessionSet")]
        public static extern int UPC_MultiplayerSessionSetImpl(IntPtr inContext, IntPtr inMultiplayerSession);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_NameGet")]
        public static extern IntPtr UPC_NameGetImpl(IntPtr inContext);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_NameGet_Extended")]
        public static extern UPC_Result UPC_NameGet_ExtendedImpl(IntPtr inContext, ref IntPtr userName);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_OverlayBrowserUrlShow")]
        public static extern int UPC_OverlayBrowserUrlShowImpl(IntPtr inContext, string inBrowserUrlUtf8, IntPtr inOptCallback, IntPtr inOptCallbackData);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_OverlayFriendInvitationShow")]
        public static extern int UPC_OverlayFriendInvitationShowImpl(IntPtr inContext, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] inOptIdListUtf8, uint inOptIdListLength);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_OverlayFriendSelectionFree")]
        public static extern int UPC_OverlayFriendSelectionFreeImpl(IntPtr inContext, long inSelectedFriends);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_OverlayFriendSelectionShow")]
        public static extern int UPC_OverlayFriendSelectionShowImpl(IntPtr inContext, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] inIdListUtf8, uint inIdListLength, ref long outSelectedFriends, IntPtr inCallback, IntPtr inCallbackData);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_OverlayMicroAppShow")]
        public static extern int UPC_OverlayMicroAppShowImpl(IntPtr inContext, string inAppName, ref Structs.UPC_Overlay_MicroAppParamList inOptMicroAppParamList, IntPtr inOptCallback, IntPtr inOptCallbackData);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_OverlayNotificationShow")]
        public static extern UPC_Result UPC_OverlayNotificationShow(IntPtr inContext, uint inId);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_OverlayShow")]
        public static extern int UPC_OverlayShowImpl(IntPtr inContext, uint inSection, IntPtr inOptCallback, IntPtr inOptCallbackData);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_ProductConsume")]
        public static extern int UPC_ProductConsumeImpl(IntPtr inContext, uint inProductId, uint inQuantity, string inTransactionIdUtf8, string inSignatureUtf8, ref long outResponseSignatureUtf8, IntPtr inCallback, IntPtr inOptCallbackData);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_ProductConsumeSignatureFree")]
        public static extern int UPC_ProductConsumeSignatureFreeImpl(IntPtr inContext, IntPtr inResponseSignature);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_ProductListFree")]
        public static extern int UPC_ProductListFreeImpl(IntPtr inContext, long inProductList);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_ProductListGet")]
        public static extern int UPC_ProductListGetImpl(IntPtr inContext, [MarshalAs(UnmanagedType.LPStr)] string inOptUserIdUtf8, uint inFilter, ref long outProductList, IntPtr inCallback, IntPtr inOptCallbackData);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_RichPresenceSet")]
        public static extern int UPC_RichPresenceSetImpl(IntPtr inContext, uint inId, IntPtr inOptTokenList);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_RichPresenceSet")]
        public static extern int UPC_RichPresenceSetImpl(IntPtr inContext, uint inId, ref Structs.UPC_RichPresenceTokenList inOptTokenList);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_ShowBrowserUrl")]
        public static extern UPC_Result UPC_ShowBrowserUrlImpl(IntPtr inContext, [MarshalAs(UnmanagedType.LPStr)] string inBrowserUrlUtf8);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_StorageFileClose")]
        public static extern int UPC_StorageFileCloseImpl(IntPtr inContext, uint inHandle);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_StorageFileDelete")]
        public static extern int UPC_StorageFileDeleteImpl(IntPtr inContext, [MarshalAs(UnmanagedType.LPStr)] string inFileNameUtf8);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_StorageFileListFree")]
        public static extern int UPC_StorageFileListFreeImpl(IntPtr inContext, long inStorageFileList);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_StorageFileListGet")]
        public static extern int UPC_StorageFileListGetImpl(IntPtr inContext, ref long outStorageFileList);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_StorageFileOpen")]
        public static extern int UPC_StorageFileOpenImpl(IntPtr inContext, string inFileNameUtf8, uint inFlags, ref IntPtr outHandle);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_StorageFileRead")]
        public static extern int UPC_StorageFileReadImpl(IntPtr inContext, uint inHandle, uint inBytesToRead, uint inBytesReadOffset, IntPtr outData, ref IntPtr outBytesRead, IntPtr inCallback, IntPtr inCallbackData);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_StorageFileWrite")]
        public static extern int UPC_StorageFileWriteImpl(IntPtr inContext, uint inHandle, IntPtr inData, uint inSize, IntPtr inCallback, IntPtr inCallbackData);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_StoreCheckout")]
        public static extern UPC_Result UPC_StoreCheckoutImpl(IntPtr inContext, uint inId);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_StoreIsEnabled")]
        public static extern int UPC_StoreIsEnabledImpl(IntPtr inContext);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_StoreLanguageSet")]
        public static extern int UPC_StoreLanguageSetImpl(IntPtr inContext, [MarshalAs(UnmanagedType.LPStr)] string inLanguageCountryCode);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_StorePartnerGet")]
        public static extern UPC_StorePartner UPC_StorePartnerGetImpl(IntPtr inContext);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_StorePartnerGet")]
        public static extern UPC_Result UPC_StorePartnerGetImpl(IntPtr inContext, ref IntPtr storePartner);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_StoreProductDetailsShow")]
        public static extern UPC_Result UPC_StoreProductDetailsShowImpl(IntPtr inContext, uint inId);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_StoreProductListFree")]
        public static extern int UPC_StoreProductListFreeImpl(IntPtr inContext, IntPtr inProductList);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_StoreProductListGet")]
        public static extern int UPC_StoreProductListGetImpl(IntPtr inContext, ref long outProductList, IntPtr inCallback, IntPtr inCallbackData);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_StoreProductsShow")]
        public static extern int UPC_StoreProductsShowImpl(IntPtr inContext, ref Structs.UPC_StoreTagList inTagsList);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_TicketGet")]
        public static extern IntPtr UPC_TicketGetImpl(IntPtr inContext);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_TicketGet_Extended")]
        public static extern UPC_Result UPC_TicketGet_ExtendedImpl(IntPtr inContext, ref IntPtr ticket);

        [DllImport("upc_r2_loader64", EntryPoint = "UPC_Uninit")]
        public static extern void UPC_UninitImpl();

        [DllImport("upc_r2_loader64", EntryPoint = "UPC_Update")]
        public static extern UPC_Result UPC_UpdateImpl(IntPtr inContext);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_UserFree")]
        public static extern int UPC_UserFreeImpl(IntPtr inContext, long inUser);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.StdCall, EntryPoint = "UPC_UserGet")]
        public static extern int UPC_UserGetImpl(IntPtr inContext, [MarshalAs(UnmanagedType.LPStr)] string inOptUserIdUtf8, ref long outUser, IntPtr inCallback, IntPtr inCallbackData);

        [DllImport("upc_r2_loader64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UPC_UserPlayedWithAdd")]
        public static extern int UPC_UserPlayedWithAddImpl(IntPtr inContext, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] inUserIdUtf8List, uint inListLength);

    }
}