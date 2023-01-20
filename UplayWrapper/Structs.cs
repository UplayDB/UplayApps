using System.Runtime.InteropServices;
using static UplayWrapper.Enums;

namespace UplayWrapper
{
    public class Structs
    {
        public struct UPC_StorageFile
        {
            public string fileNameUtf8;
            public string legacyNameUtf8;
            public uint size;
            public ulong lastModifiedMs;
        }

        public struct UPC_UserList
        {
            public uint count;
            public IntPtr list;
        }

        public struct UPC_AchievementList
        {
            public uint count;
            public IntPtr ptrList;
        }

        public struct UPC_UserIdList
        {
            public uint count;
            public IntPtr list;
        }

        public struct UPC_InstallChunkList
        {
            public uint count;
            public IntPtr list;
        }

        public struct UPC_ProductList
        {
            public uint count;
            public IntPtr list;
        }

        public struct UPC_StorageFileList
        {
            public uint count;
            public IntPtr list;
        }

        public struct UPC_StoreTagList
        {
            public uint count;
            public IntPtr list;
        }

        public struct UPC_StoreProductList
        {
            public uint count;
            public IntPtr list;
        }

        public struct UPC_StreamResolution
        {
            public uint width;
            public uint height;
        }

        public struct UPC_RichPresenceTokenList
        {
            public uint count;
            public IntPtr list;
        }

        public struct UPC_ContextSettings
        {
            public UPC_ContextSubsystem subsystems;
        }

        public struct UPC_Overlay_MicroAppParam
        {
            public string paramNameUtf8;
            public string paramValueUtf8;
        }

        public struct UPC_Overlay_MicroAppParamList
        {
            public uint count;
            public IntPtr list;
        }

        public struct UPC_Achievement
        {
            public uint id;
            public string nameUtf8;
            public string descriptionUtf8;
            public int completed;
        }

        public struct UPC_MultiplayerSession
        {
            public string id;
            public UPC_MultiplayerSessionJoinability joinability;
            public uint size;
            public uint maxSize;
            public IntPtr internalData;
            public uint internalDataSize;
        }

        public struct UPC_RichPresenceToken
        {
            public string idUtf8;
            public string valueIdUtf8;
        }

        public struct UPC_Product
        {
            public uint id;
            public UPC_ProductType type;
            public UPC_ProductOwnership ownership;
            public UPC_ProductState state;
            public uint balance;
            public UPC_ProductActivation activation;
        }

        #region Class
        public class UPC_Presence
        {
            public UPC_OnlineStatus onlineStatus;
            public string detailsUtf8;
            public uint titleId;
            public string titleNameUtf8;
            public string multiplayerId;
            public int multiplayerJoinable;
            public uint multiplayerSize;
            public uint multiplayerMaxSize;
            public byte[] multiplayerInternalData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class UPC_StoreProduct
        {
            public uint id;
            public string titleUtf8;
            public string descriptionUtf8;
            public string imageUrlUtf8;
            public byte isOwned;
            public float price;
            public float priceOriginal;
            public string currencyUtf8;
            public string userBlobUtf8;
            public UPC_StoreTag[] tags;
        }
        public class UPC_User
        {
            [MarshalAs(UnmanagedType.LPUTF8Str)]
            public string idUtf8;
            [MarshalAs(UnmanagedType.LPUTF8Str)]
            public string nameUtf8;
            public UPC_Relationship relationship;
            public UPC_Presence presence;
        }
        #endregion
        #region Struct With functions
        public struct UPC_PresenceImpl
        {
            public UPC_Presence BuildMemoryCopy()
            {
                UPC_Presence upc_Presence = new UPC_Presence();
                upc_Presence.onlineStatus = this.onlineStatus;
                upc_Presence.detailsUtf8 = this.detailsUtf8;
                upc_Presence.titleId = this.titleId;
                upc_Presence.titleNameUtf8 = this.titleNameUtf8;
                upc_Presence.multiplayerId = this.multiplayerId;
                upc_Presence.multiplayerJoinable = this.multiplayerJoinable;
                upc_Presence.multiplayerSize = this.multiplayerSize;
                upc_Presence.multiplayerMaxSize = this.multiplayerMaxSize;
                if (this.multiplayerInternalData != IntPtr.Zero)
                {
                    upc_Presence.multiplayerInternalData = new byte[this.multiplayerInternalDataSize];
                    Marshal.Copy(this.multiplayerInternalData, upc_Presence.multiplayerInternalData, 0, upc_Presence.multiplayerInternalData.Length);
                }
                return upc_Presence;
            }

            public UPC_OnlineStatus onlineStatus;
            public string detailsUtf8;
            public uint titleId;
            public string titleNameUtf8;
            public string multiplayerId;
            public int multiplayerJoinable;
            public uint multiplayerSize;
            public uint multiplayerMaxSize;
            public IntPtr multiplayerInternalData;
            public uint multiplayerInternalDataSize;
        }
        public struct UPC_StoreProductImpl
        {
            public UPC_StoreProduct BuildMemoryCopy()
            {
                UPC_StoreProduct upc_StoreProduct = new UPC_StoreProduct();
                upc_StoreProduct.id = this.id;
                upc_StoreProduct.titleUtf8 = this.titleUtf8;
                upc_StoreProduct.descriptionUtf8 = this.descriptionUtf8;
                upc_StoreProduct.imageUrlUtf8 = this.imageUrlUtf8;
                upc_StoreProduct.isOwned = this.isOwned;
                upc_StoreProduct.price = this.price;
                upc_StoreProduct.priceOriginal = this.priceOriginal;
                upc_StoreProduct.currencyUtf8 = this.currencyUtf8;
                upc_StoreProduct.userBlobUtf8 = this.userBlobUtf8;
                if (this.tags.list != IntPtr.Zero)
                {
                    UPC_StoreTag[] array = new UPC_StoreTag[this.tags.count];
                    int num = Marshal.SizeOf(typeof(uint));
                    int num2 = 0;
                    while ((long)num2 < (long)((ulong)this.tags.count))
                    {
                        UPC_StoreTag upc_StoreTag = (UPC_StoreTag)Global.IntPtrToStruct<uint>(new IntPtr(this.tags.list.ToInt64() + (long)(num * num2)));
                        array[num2] = upc_StoreTag;
                        num2++;
                    }
                    upc_StoreProduct.tags = array;
                }
                return upc_StoreProduct;
            }
            public uint id;
            public string titleUtf8;
            public string descriptionUtf8;
            public string imageUrlUtf8;
            public byte isOwned;
            public float price;
            public float priceOriginal;
            public string currencyUtf8;
            public string userBlobUtf8;
            [MarshalAs(UnmanagedType.Struct)]
            public UPC_StoreTagList tags;
        }

        public class UPC_TaskResult
        {
            public UPC_Result Result { get; }
            public object OptionalData { get; }
            public UPC_TaskResult(int resultCode, object optionalData)
            {
                this.Result = (UPC_Result)resultCode;
                this.OptionalData = optionalData;
            }
        }

        public class UPC_TaskResult<T>
        {
            public UPC_Result Result { get; }
            public T ResultData { get; }
            public object OptionalData { get; }
            public UPC_TaskResult(int resultCode, T resultData, object optionalData)
            {
                this.Result = (UPC_Result)resultCode;
                this.ResultData = resultData;
                this.OptionalData = optionalData;
            }
        }
        #endregion

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct UPLAY_Event
        {
            [MarshalAs(UnmanagedType.U4)]
            public UPLAY_EventType type;
            public IntPtr evt;
        }

        public struct UPLAY_Overlapped
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public ulong[] _internal;
        }

        public struct UPLAY_PRESENCE_TokenList
        {
            public uint count;
            public IntPtr list;
        }
    }
}
