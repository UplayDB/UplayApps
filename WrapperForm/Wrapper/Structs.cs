using System.Runtime.InteropServices;
using static UplayWrapper.Enums;
using static UplayWrapper.Global;
using static UplayWrapper.Structs;

namespace TestForm.Wrapper
{
    public class Structs
    {
        public struct UPC_EventNextPoll_Result
        {
            public UplayImpl.UPC_Event resultEvent;

            public UPC_Result resultCode;
        }
        public struct UPC_EventData_ProductOwnershipUpdated : IUPC_EventData
        {
            public uint id;
            public UPC_ProductOwnership newOwnership;
        }
        public struct UPC_EventData_ProductAdded : IUPC_EventData
        {
            [MarshalAs(UnmanagedType.Struct)]
            public UPC_Product product;
        }
        public struct UPC_EventData_MultiplayerSessionUpdated : IUPC_EventData
        {
            [MarshalAs(UnmanagedType.Struct)]
            public UPC_MultiplayerSession newSession;
        }

        public struct UPC_EventData_MultiplayerInviteReceived : IUPC_EventData
        {
            public string senderId;

            [MarshalAs(UnmanagedType.Struct)]
            public UPC_MultiplayerSession session;
        }

        public struct UPC_EventData_MultiplayerInviteAccepted : IUPC_EventData
        {
            public string senderId;

            [MarshalAs(UnmanagedType.Struct)]
            public UPC_MultiplayerSession session;
        }

        public struct UPC_EventData_MultiplayerInviteDeclined : IUPC_EventData
        {
            public string senderId;
        }

        public struct UPC_EventData_MultiplayerJoiningRequested : IUPC_EventData
        {
            [MarshalAs(UnmanagedType.Struct)]
            public UPC_MultiplayerSession session;
        }

        public struct UPC_EventData_FriendAdded : IUPC_EventData
        {
            public UPC_User newFriend
            {
                get
                {
                    return this.newFriendImpl.BuildMemoryCopy();
                }
            }

            [MarshalAs(UnmanagedType.Struct)]
            private UPC_UserImpl newFriendImpl;
        }
        private struct UPC_UserImpl
        {
            public UPC_User BuildMemoryCopy()
            {
                UPC_User upc_User = new UPC_User();
                upc_User.idUtf8 = this.idUtf8;
                upc_User.nameUtf8 = this.nameUtf8;
                upc_User.relationship = this.relationship;
                if (this.presence != IntPtr.Zero)
                {
                    upc_User.presence = IntPtrToStruct<UPC_PresenceImpl>(this.presence).BuildMemoryCopy();
                }
                return upc_User;
            }

            public string idUtf8;

            public string nameUtf8;

            public UPC_Relationship relationship;

            public IntPtr presence;
        }
        public struct UPC_EventData_FriendNameUpdated : IUPC_EventData
        {
            public string friendId;

            public string newName;
        }

        public struct UPC_EventData_FriendPresenceUpdated : IUPC_EventData
        {
            public UPC_Presence newPresence
            {
                get
                {
                    return this.newPresenceImpl.BuildMemoryCopy();
                }
            }

            public string friendId;

            [MarshalAs(UnmanagedType.Struct)]
            private UPC_PresenceImpl newPresenceImpl;
        }

        public struct UPC_EventData_FriendRemoved : IUPC_EventData
        {
            public string friendId;
        }

        public struct UPC_EventData_InstallChunkProgress : IUPC_EventData
        {
            public uint id;

            public ulong installedBytes;

            public ulong sizeInBytes;
        }

        public struct UPC_EventData_InstallProgress : IUPC_EventData
        {
            public ulong installedBytes;

            public ulong sizeInBytes;

            public ulong bytesPerSecond;
        }

        public struct UPC_EventData_InstallChunkInstalled : IUPC_EventData
        {
            public uint id;
        }

        public struct UPC_EventData_StoreCheckoutStarted : IUPC_EventData
        {
            public uint productId;
        }

        public struct UPC_EventData_StoreCheckoutFinished : IUPC_EventData
        {
            public uint productId;

            public int result;
        }

        public struct UPC_EventData_StoreStatusUpdated : IUPC_EventData
        {
            public int enabled;
        }

        public struct UPC_EventData_ProductStateUpdated : IUPC_EventData
        {
            public uint id;

            public UPC_ProductState newState;
        }

        public struct UPC_EventData_ProductBalanceUpdated : IUPC_EventData
        {
            public uint id;

            public uint newBalance;
        }

        public struct UPC_EventData_UserShutdown : IUPC_EventData
        {
            private uint reason;
        }
    }
}
