using System.Runtime.InteropServices;

namespace TestApp
{
    internal class uplay_r1
    {
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_ACH_EarnAchievement(uint aAchivementId, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_ACH_GetAchievementImage(uint aId, IntPtr aOutImage, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_ACH_GetAchievements(uint aFilter, IntPtr aAccountIdUtf8OrNULLIfCurrentUser, IntPtr aOutAchievementList, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_ACH_ReleaseAchievementImage(IntPtr aImage);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_ACH_ReleaseAchievementList(IntPtr aList);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_ACH_Write(IntPtr aAchievement);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_AVATAR_GetBitmap(IntPtr aAvatarId, int aAvatarSize, IntPtr aOutRGBA, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_AVATAR_GetAvatarIdForCurrentUser(IntPtr aOutAvatarId, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_AVATAR_Get(IntPtr aAccountIdUtf8, int aAvatarSize, IntPtr aOutRGBA, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_AVATAR_Release(IntPtr aRGBA);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_CHAT_GetHistory(IntPtr aAccountIdUtf8, uint aMaxNumberOfMessages, IntPtr aOutHistoryList, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_CHAT_Init(int aFlags);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_CHAT_ReleaseHistoryList(IntPtr aHistoryList);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_CHAT_SendMessage(IntPtr aAccountIdUtf8, IntPtr aMessageUtf8, IntPtr aData);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_CHAT_SetMessagesRead(IntPtr aAccountIdUtf8);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_ClearGameSession();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_FRIENDS_AddPlayedWith(IntPtr aDescriptionUtf8, IntPtr aAccountIdListUtf8, uint aAccountIdListLength);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_FRIENDS_AddToBlackList(IntPtr aAccountIdUtf8, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_FRIENDS_DisableFriendMenuItem(uint aId);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_FRIENDS_EnableFriendMenuItem(uint aId, uint aMenuItemMode, uint aFilter);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_FRIENDS_GetFriendList(uint aFriendListFilter, IntPtr aOutFriendList);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_FRIENDS_Init(uint aFlags);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_FRIENDS_InviteToGame(IntPtr aAccountIdUtf8, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_FRIENDS_IsBlackListed(IntPtr aAccountIdUtf8);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_FRIENDS_IsFriend(IntPtr aAccountIdUtf8);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_FRIENDS_RemoveFriendship(IntPtr aAccountIdUtf8, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_FRIENDS_RemoveFromBlackList(IntPtr aAccountIdUtf8, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_FRIENDS_RequestFriendship(IntPtr aSearchStringUtf8, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_FRIENDS_RespondToGameInvite(uint aInvitationId, IntPtr aAccept);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_FRIENDS_ShowFriendSelectionUI(IntPtr aAccountIdFilterListUTF8, uint aAccountIdFilterListLength, IntPtr aOverlapped, IntPtr aOutResult);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_FRIENDS_ShowInviteFriendsToGameUI(IntPtr aAccountIdFilterListUtf8, uint aAccountIdFilterListLength);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_GetLastError(IntPtr aOutErrorString);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_GetNextEvent(IntPtr aEvent);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_GetOverlappedOperationResult(IntPtr aOverlapped, IntPtr aOutResult);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_HasOverlappedOperationCompleted(IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_INSTALLER_AreChunksInstalled(IntPtr aChunkIds, uint aChunkCount);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_INSTALLER_GetChunkIdsFromTag(IntPtr aTagUtf8, IntPtr aOutChunkIdList);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_INSTALLER_GetChunks(IntPtr aOutChunkIdList);
        [DllImport("uplay_r1_loader")]
        public static extern IntPtr UPLAY_INSTALLER_GetLanguageUtf8();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_INSTALLER_Init(uint aFlags);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_INSTALLER_ReleaseChunkIdList(IntPtr aChunkIdList);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_INSTALLER_UpdateInstallOrder(IntPtr aChunkIds, uint aChunkCount);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_Init();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_METADATA_ClearContinuousTag(IntPtr aStringNameUtf8);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_METADATA_SetContinuousTag(IntPtr aStringNameUtf8, IntPtr aStringValueUtf8);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_METADATA_SetSingleEventTag(IntPtr aStringNameUtf8, IntPtr aStringValueUtf8);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_OPTIONS_Apply(IntPtr aFileHandle, IntPtr aKeyValueList, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_OPTIONS_Close(IntPtr aFileHandle);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_OPTIONS_Enumerate(IntPtr aFileHandle, IntPtr aOutKeyValueList, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_OPTIONS_Get(IntPtr aKeyValueList, IntPtr aKey);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_OPTIONS_Open(IntPtr aName);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_OPTIONS_ReleaseKeyValueList(IntPtr aKeyValueList);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_OPTIONS_Set(IntPtr aKeyValueList, IntPtr aKey, IntPtr aValue);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_OPTIONS_SetInGameState(uint aFlags);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_OVERLAY_SetShopUrl(IntPtr aUrl, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_OVERLAY_Show(IntPtr aOverlaySection, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_OVERLAY_ShowBrowserUrl(IntPtr aUrlUtf8);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_OVERLAY_ShowFacebookAuthentication(IntPtr aFacebookAppId, IntPtr aRedirectUri, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_OVERLAY_ShowMicroApp(IntPtr aAppName, IntPtr aMicroAppParamList, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_OVERLAY_ShowNotification(uint aNotificationId);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_OVERLAY_ShowShopUrl(IntPtr aUrlUtf8);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PARTY_DisablePartyMemberMenuItem();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PARTY_EnablePartyMemberMenuItem();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PARTY_GetFullMemberList(IntPtr aOutMemberList);
        [DllImport("uplay_r1_loader")]
        public static extern int UPLAY_PARTY_GetId();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PARTY_GetInGameMemberList(IntPtr aOutMemberList);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PARTY_Init(uint aFlags);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PARTY_InvitePartyToGame(IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PARTY_InviteToParty(IntPtr aAccountIdUtf8, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PARTY_IsInParty(IntPtr aAccountIdUtf8);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PARTY_IsPartyLeader(IntPtr aAccountIdUtf8);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PARTY_PromoteToLeader(IntPtr aAccountIdUtf8, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PARTY_RespondToGameInvite(uint aInvitationId, bool aAccept);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PARTY_SetGuest(IntPtr guestId, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PARTY_SetUserData(IntPtr aDataBlob);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PARTY_ShowGameInviteOverlayUI(uint aInvitationId);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PRESENCE_SetPresence(uint presenceId, IntPtr tokens);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PRODUCT_GetProductList(IntPtr aOverlapped, IntPtr aOutProductList);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PRODUCT_ReleaseProductList(IntPtr aProductList);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_PeekNextEvent();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_Quit();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_Release(IntPtr aPointer);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_SAVE_Close(IntPtr aOutSaveHandle);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_SAVE_GetSavegames(IntPtr aOutGamesList, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_SAVE_Open(uint aSlotId, uint aMode, IntPtr aOutSaveHandle, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_SAVE_Read(IntPtr aSaveHandle, uint aNumOfBytesToRead, uint aOffset, IntPtr aOutBuffer, uint aOutNumOfBytesRead, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_SAVE_ReleaseGameList(IntPtr aGamesList);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_SAVE_Remove(uint aSlotId, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_SAVE_SetName(IntPtr aSaveHandle, IntPtr aNameUtf8);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_SAVE_Write(IntPtr aSaveHandle, uint aNumOfBytesToWrite, IntPtr aBuffer, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_STORE_Checkout(uint aId);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_STORE_GetPartner();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_STORE_GetProducts(IntPtr aOverlapped, IntPtr aOutProductList);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_STORE_IsEnabled();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_STORE_ReleaseProductsList(IntPtr aProductList);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_STORE_ShowProductDetails(uint aId);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_STORE_ShowProducts(uint aTags);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_SetGameSession(IntPtr aGameSessionIdentifier, IntPtr aSessionData, uint aFlags);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_SetLanguage(IntPtr aLanguageCountryCode);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_Start(uint aUplayId, uint aFlags);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_Startup(uint aUplayId, uint aGameVersion, IntPtr aLanguageCountryCodeUtf8);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_USER_ClearGameSession();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_USER_ConsumeItem(IntPtr aTransactionIdUtf8, uint aUplayId, uint aQuantity, IntPtr aSignatureUtf8, IntPtr aOverlapped, IntPtr aOutResult);
        [DllImport("uplay_r1_loader")]
        public static extern IntPtr UPLAY_USER_GetAccountId(IntPtr aOutAccountId);
        [DllImport("uplay_r1_loader")]
        public static extern IntPtr UPLAY_USER_GetAccountIdUtf8();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_USER_GetCPUScore(IntPtr aOutCpuScore);
        [DllImport("uplay_r1_loader")]
        public static extern IntPtr UPLAY_USER_GetCdKeyUtf8(uint aUplayId);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_USER_GetCdKeys(IntPtr aOutCdKeyList, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_USER_GetConsumableItems(IntPtr aOutConsumableItemsList);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_USER_GetCredentials(IntPtr aOutUserCredentials, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern IntPtr UPLAY_USER_GetEmail(IntPtr aOutEmail);
        [DllImport("uplay_r1_loader")]
        public static extern IntPtr UPLAY_USER_GetEmailUtf8();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_USER_GetGPUScore(IntPtr aOutGpuScore);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_USER_GetGPUScoreConfidenceLevel(IntPtr aOutConfidenceLevel);
        [DllImport("uplay_r1_loader")]
        public static extern IntPtr UPLAY_USER_GetNameUtf8();
        [DllImport("uplay_r1_loader")]
        public static extern IntPtr UPLAY_USER_GetPassword(IntPtr aOutPassword);
        [DllImport("uplay_r1_loader")]
        public static extern IntPtr UPLAY_USER_GetPasswordUtf8();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_USER_GetProfile(IntPtr aAccountIdUtf8, IntPtr aOverlapped, IntPtr aOutProfile);
        [DllImport("uplay_r1_loader")]
        public static extern IntPtr UPLAY_USER_GetTicketUtf8();
        [DllImport("uplay_r1_loader")]
        public static extern IntPtr UPLAY_USER_GetUsername(IntPtr aOutUsername);
        [DllImport("uplay_r1_loader")]
        public static extern IntPtr UPLAY_USER_GetUsernameUtf8();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_USER_IsConnected();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_USER_IsInOfflineMode();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_USER_IsOwned(uint aUplayId);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_USER_ReleaseCdKeyList(IntPtr aCdKeyList);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_USER_ReleaseConsumeItemResult(IntPtr aConsumeItemResult);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_USER_ReleaseProfile(IntPtr aOutProfile);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_USER_SetGameSession(IntPtr aGameSessionIdentifier, IntPtr aSessionData, uint aFlags);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_Update();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_WIN_GetActions(IntPtr aOutActionList, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_WIN_GetRewards(IntPtr aOutRewardList, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_WIN_GetUnitBalance(int aOutBalance, IntPtr aOverlapped);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_WIN_RefreshActions();
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_WIN_ReleaseActionList(IntPtr aActionList);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_WIN_ReleaseRewardList(IntPtr aRewardList);
        [DllImport("uplay_r1_loader")]
        public static extern bool UPLAY_WIN_SetActionsCompleted(IntPtr aActionIdsUtf8, uint aActionIdsCount, IntPtr aOverlapped);
    }
}
