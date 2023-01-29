using Newtonsoft.Json;
using System.Runtime.InteropServices;
using TestForm.Wrapper;
using UplayWrapper;
using static TestForm.Wrapper.UplayImpl;
using static UplayWrapper.Enums;
using static UplayWrapper.Structs;

namespace TestForm
{
    public class dllTest
    {
        public delegate void ProductOwnershipUpdatedDelegate(uint id, UPC_ProductOwnership ownership);
        public delegate void ProductAddedDelegate(UPC_Product product);
        public delegate void ProductListRefreshedDelegate(List<UPC_Product> products);
        public event ProductOwnershipUpdatedDelegate ProductOwnershipUpdated;
        public event ProductAddedDelegate ProductAdded;
        public event ProductListRefreshedDelegate ProductListRefreshed;
        public UPC_EventEnumerator eventEnumerator;
        public delegate void OnOverlayShown();
        public event OnOverlayShown OverlayShown;
        public UPC_StorageFile storageFile;
        private bool isFirstSave;
        public List<UPC_Product> ProductsList { get; private set; }
        public bool IsProductListReady { get; private set; }
        public IntPtr Context { get; private set; }
        public string Ticket { get; private set; } = "";
        public void Init(Label label)
        {
            HandleEventDelegater = Marshal.GetFunctionPointerForDelegate<UPC_EventHandlerImpl>(HandleEvent);
            if (!File.Exists("upc_r2.ini"))
            {
                MessageBox.Show("upc_r2.ini File not exist! (THIS IS NOT A CRACK! ITS A WRAPPER!)");
                Environment.Exit(1);
            }
            // basic Dll impl (CURRENTLY ONLY WORK WITH Hungry Shark World)
            var x = File.ReadAllLines("upc_r2.ini");
            var inVersion = uint.Parse(x[2]);
            var inProductId = uint.Parse(x[4]);

            var InitializationResult = upc_r2_loader64.UPC_InitImpl(inVersion, inProductId);
            Debug.WriteDebug(InitializationResult.ToString());
            if (InitializationResult == UPC_InitResult.UPC_InitResult_Ok)
            {
                label.Text = InitializationResult.ToString();
                UPC_ContextSettings upc_ContextSettings = new UPC_ContextSettings()
                {
                    subsystems = (UPC_ContextSubsystem.UPC_ContextSubsystem_Achievement |
                    UPC_ContextSubsystem.UPC_ContextSubsystem_Product |
                    UPC_ContextSubsystem.UPC_ContextSubsystem_Overlay |
                    UPC_ContextSubsystem.UPC_ContextSubsystem_Friend |
                    UPC_ContextSubsystem.UPC_ContextSubsystem_Multiplayer |
                    UPC_ContextSubsystem.UPC_ContextSubsystem_Store)
                };
                Context = upc_r2_loader64.UPC_ContextCreateImpl(inVersion, ref upc_ContextSettings);
                Ticket = UPC_TicketGet(Context);
                eventEnumerator = new UPC_EventEnumerator(Context);
                Debug.WriteDebug(Ticket);
                List<UPC_EventType> list = new List<UPC_EventType>
                {
                    UPC_EventType.UPC_Event_OverlayShown,
                    UPC_EventType.UPC_Event_OverlayHidden
                };
                UPC_EventRegisterHandler(Context, list, new UplayImpl.UPC_EventHandler(EventHandler));
            }
            else
            {
                Environment.Exit(1);
            }

        }

        private void EventHandler(UPC_Event inEvent)
        {
            Debug.WriteDebug($"[dllTest.EventHandler]  {inEvent.Type} {inEvent.IsValid}");
            UPC_EventType type = inEvent.Type;
            if (type != UPC_EventType.UPC_Event_OverlayShown)
            {
                if (type == UPC_EventType.UPC_Event_OverlayHidden)
                {
                    Debug.WriteDebug("[EventHandler] Overlay Hidden");
                }
            }
            else
            {
                Debug.WriteDebug("[EventHandler] Overlay Shown");
                TriggerOverlayShown();
            }

        }
        public void ShowOverlayForSection(UPC_OverlaySection section)
        {
            UPC_OverlayShow(this.Context, section, delegate (UPC_TaskResult result)
            {
                if (result.Result != UPC_Result.UPC_Result_Ok)
                {
                    Debug.PWDebug("[UbiConnectManager] Problem opening the overlay " + result.Result);
                }
            }, null);
        }
        private void TriggerOverlayShown()
        {
            OverlayShown?.Invoke();
        }

        public void GetId()
        {
            var ID = UPC_IdGet(Context);
            Debug.WriteDebug(ID);

        }

        public void showbrowser()
        {
            var ID = UPC_IdGet(Context);
            UPC_AchievementListGet(Context,ID, new GenericUpcDelegate<UPC_Achievement[]>(AchiList));
            //UPC_AchievementImageGet(Context, 1, new GenericUpcDelegate<byte[]>(ImageGet));
        }

        private void AchiList(UPC_TaskResult<UPC_Achievement[]> result)
        {
            var x = JsonConvert.SerializeObject(result);
            File.WriteAllText("AchiList.json", x);
        }

        private void ImageGet(UPC_TaskResult<byte[]> result)
        {
            var x = JsonConvert.SerializeObject(result);
            File.WriteAllText("ImageGet.json", x);
            //File.WriteAllBytes("img",result.ResultData);
        }
        private void GetStorageFile()
        {
            this.isFirstSave = false;
            this.storageFile = StorageFileGet(Context, "1");
            if (this.storageFile.fileNameUtf8 == null)
            {
                this.storageFile.fileNameUtf8 = "1";
                this.isFirstSave = true;
            }
        }

        public void Update()
        {
            UpdateInternal();
            UpdateEvents();
        }

        public void UpdateInternal()
        {
            var UpdateResult = upc_r2_loader64.UPC_UpdateImpl(Context);
            Debug.PWDebug(UpdateResult);
        }

        private void UpdateEvents()
        {
            for (; ; )
            {
                Wrapper.Structs.UPC_EventNextPoll_Result upc_EventNextPoll_Result = eventEnumerator.UPC_EventNextPoll();
                Debug.PWDebug("[UpdateEvents] Received Uplay Event Next Pool : " + JsonConvert.SerializeObject(upc_EventNextPoll_Result));
                if (upc_EventNextPoll_Result.resultEvent == null || upc_EventNextPoll_Result.resultCode == UPC_Result.UPC_Result_NotFound)
                {
                    break;
                }
                var code = upc_EventNextPoll_Result.resultCode;
                UPC_EventType type = upc_EventNextPoll_Result.resultEvent.Type;
                var isValid = upc_EventNextPoll_Result.resultEvent.IsValid;
                Debug.PWDebug($"[UpdateEvents] Received Uplay Event : {type} {isValid} with Code: {code}");

                switch (type)
                {
                    case UPC_EventType.UPC_Event_FriendAdded:
                        break;
                    case UPC_EventType.UPC_Event_FriendNameUpdated:
                        break;
                    case UPC_EventType.UPC_Event_FriendPresenceUpdated:
                        break;
                    case UPC_EventType.UPC_Event_FriendRemoved:
                        break;
                    case UPC_EventType.UPC_Event_MultiplayerSessionCleared:
                        break;
                    case UPC_EventType.UPC_Event_MultiplayerSessionUpdated:
                        break;
                    case UPC_EventType.UPC_Event_MultiplayerInviteReceived:
                        break;
                    case UPC_EventType.UPC_Event_MultiplayerInviteAccepted:
                        break;
                    case UPC_EventType.UPC_Event_MultiplayerInviteDeclined:
                        break;
                    case UPC_EventType.UPC_Event_MultiplayerJoiningRequested:
                        break;
                    case UPC_EventType.UPC_Event_OverlayShown:
                        break;
                    case UPC_EventType.UPC_Event_OverlayHidden:
                        break;
                    case UPC_EventType.UPC_Event_ProductAdded:
                        Debug.PWDebug("UPC_Event_ProductAdded");
                        Wrapper.Structs.UPC_EventData_ProductAdded EventData_ProductAdded = upc_EventNextPoll_Result.resultEvent.GetAs<Wrapper.Structs.UPC_EventData_ProductAdded>();
                        UPC_Product upc_Product2 = ProductsList.FirstOrDefault((UPC_Product x) => x.id == EventData_ProductAdded.product.id);
                        if (upc_Product2.id == EventData_ProductAdded.product.id)
                        {
                            upc_Product2.ownership = EventData_ProductAdded.product.ownership;
                        }
                        else
                        {
                            RefreshProductList();
                        }
                        if (ProductAdded != null)
                        {
                            ProductAdded(EventData_ProductAdded.product);
                        }
                        break;
                    case UPC_EventType.UPC_Event_ProductOwnershipUpdated:
                        Debug.PWDebug("UPC_Event_ProductOwnershipUpdated");
                        Wrapper.Structs.UPC_EventData_ProductOwnershipUpdated EventData_ProductOwnershipUpdated = upc_EventNextPoll_Result.resultEvent.GetAs<Wrapper.Structs.UPC_EventData_ProductOwnershipUpdated>();
                        UPC_Product upc_Product = ProductsList.FirstOrDefault((UPC_Product x) => x.id == EventData_ProductOwnershipUpdated.id);
                        if (upc_Product.id == EventData_ProductOwnershipUpdated.id)
                        {
                            upc_Product.ownership = EventData_ProductOwnershipUpdated.newOwnership;
                        }
                        else
                        {
                            RefreshProductList();
                        }
                        if (ProductOwnershipUpdated != null)
                        {
                            ProductOwnershipUpdated(EventData_ProductOwnershipUpdated.id, EventData_ProductOwnershipUpdated.newOwnership);
                        }
                        break;
                    case UPC_EventType.UPC_Event_ProductStateUpdated:
                        break;
                    case UPC_EventType.UPC_Event_ProductBalanceUpdated:
                        break;
                    case UPC_EventType.UPC_Event_InstallChunkInstalled:
                        break;
                    case UPC_EventType.UPC_Event_InstallChunkProgress:
                        break;
                    case UPC_EventType.UPC_Event_InstallProgress:
                        break;
                    case UPC_EventType.UPC_Event_UpdateAvailable:
                        break;
                    case UPC_EventType.UPC_Event_StoreProductsListUpdated:
                        break;
                    case UPC_EventType.UPC_Event_StoreStatusUpdated:
                        break;
                    case UPC_EventType.UPC_Event_StoreCheckoutStarted:
                        break;
                    case UPC_EventType.UPC_Event_StoreCheckoutFinished:
                        break;
                    case UPC_EventType.UPC_Event_UserShutdown:
                        break;
                    case UPC_EventType.UPC_Event_StreamingCurrentUserCountryUpdated:
                        break;
                    case UPC_EventType.UPC_Event_StreamingDeviceUpdated:
                        break;
                    case UPC_EventType.UPC_Event_StreamingInputTypeUpdated:
                        break;
                    case UPC_EventType.UPC_Event_StreamingResolutionUpdated:
                        break;
                    default:
                        break;
                }
            }
        }

        public void Shutdown()
        {
            upc_r2_loader64.UPC_ContextFreeImpl(Context);
            upc_r2_loader64.UPC_UninitImpl();

        }

        public void RefreshProductList()
        {
            var ID = UPC_IdGet(Context);
            Debug.WriteDebug(ID);
            uint maxValue = uint.MaxValue;
            this.IsProductListReady = false;
            UPC_ProductListCallback(this.Context, ID, maxValue, new ProductListDelegate(OnProductListFetched), null);
        }

        private void OnProductListFetched(UPC_Product[] productList)
        {
            ProductsList = new List<UPC_Product>(productList);
            Debug.PWDebug($"[OnProductListFetched] {JsonConvert.SerializeObject(ProductsList)}");
            IsProductListReady = true;
            if (ProductListRefreshed != null)
            {
                ProductListRefreshed(ProductsList);
            }
        }

    }
}
