using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UplayWrapper;
using static TestForm.Wrapper.Structs;
using static UplayWrapper.Enums;
using static UplayWrapper.Structs;

namespace TestForm.Wrapper
{
    public partial class UplayImpl
    {
        #region
        #region Fields
        private static UPC_EventHandler s_eventHandler = null;
        private static long s_requestId = 0L;
        private static readonly Dictionary<long, UPC_Callback> s_waitingRequests = new Dictionary<long, UPC_Callback>();
        private static UPC_CallbackImpl UPC_OverlayShow_Callback = null;
        private static UPC_CallbackImpl UPC_AvatarGet_Callback = null;
        private static UPC_CallbackImpl UPC_OverlayBrowserUrlShow_Callback = null;
        public delegate void UPC_EventHandler(UPC_Event inEvent);
        public delegate void UPC_EventHandlerImpl(IntPtr inevent, IntPtr inData);
        public delegate void UPC_CallbackImpl(int inResult, IntPtr inData);
        public delegate void UPC_Callback(int inResult);
        public static IntPtr HandleEventDelegater;
        public delegate void GenericUpcDelegate<T>(UPC_TaskResult<T> result);

        public delegate void ProductListDelegate(UPC_Product[] productList);
        #endregion

        #region Event/Reguest Handling
        public static void HandleEvent(IntPtr inEvent, IntPtr _)
        {
            Debug.WriteDebug($"[HandleEvent] {inEvent} {_}");
            using (UPC_Event upc_Event = new UPC_Event(inEvent))
            {
                s_eventHandler(upc_Event);
            }
        }
        private static long PushRequest(UPC_Callback callback)
        {
            Debug.WriteDebug("PushRequest " + s_requestId);
            long num2;
            lock (s_waitingRequests)
            {
                long num = s_requestId;
                s_requestId = num + 1L;
                num2 = num;
                s_waitingRequests.Add(num2, callback);
            }
            return num2;
        }
        private static void CancelRequest(long requestId)
        {
            Debug.WriteDebug("CancelRequest " + requestId);
            lock (s_waitingRequests)
            {
                s_waitingRequests.Remove(requestId);
            }
        }

        [PInvokeCallback(typeof(UPC_CallbackImpl))]
        private static void HandleRequest(int inResult, IntPtr inData)
        {
            Debug.PWDebug($"HandleRequest | {inResult}| {inData}");
            long num = inData.ToInt64();
            UPC_Callback upc_Callback = null;
            lock (s_waitingRequests)
            {
                if (!s_waitingRequests.TryGetValue(num, out upc_Callback))
                {
                    throw new Exception(string.Format("Invalid Request ID received {0}, the UPC request should have been not executed but was. Something went wrong", num));
                }
                s_waitingRequests.Remove(num);
            }
            if (upc_Callback != null)
            {
                upc_Callback(inResult);
            }
        }

        #endregion


        public static string UPC_TicketGet(IntPtr inContext)
        {
            IntPtr ptr = IntPtr.Zero;
            ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
            var res = upc_r2_loader64.UPC_TicketGet_ExtendedImpl(inContext, ref ptr);
            string text = Marshal.PtrToStringAnsi(ptr);
            return (!string.IsNullOrWhiteSpace(text)) ? text : null;
        }

        public static void UPC_ProductListCallback(IntPtr inContext, string inOptUserIdUtf8, uint inFilter, ProductListDelegate productListDelegate, object inResultOptData = null)
        {
            long ptrProductList = 0L;
            long num = PushRequest(delegate (int inResult)
            {
                Debug.PWDebug("PushRequest!!!!!!!!!!!");
                UPC_Product[] array = null;
                if (inResult == 0)
                {
                    UPC_ProductList upc_ProductList = Global.IntPtrToStruct<UPC_ProductList>(new IntPtr(ptrProductList));
                    array = new UPC_Product[upc_ProductList.count];
                    int num2 = Marshal.SizeOf(typeof(IntPtr));
                    int num3 = 0;
                    while ((long)num3 < (long)((ulong)upc_ProductList.count))
                    {
                        IntPtr ptr = new IntPtr(upc_ProductList.list.ToInt64() + (long)(num2 * num3));
                        UPC_Product upc_Product = Global.IntPtrToStruct<UPC_Product>(new IntPtr(Global.IntPtrToStruct<long>(ptr)));
                        array[num3] = upc_Product;
                        num3++;
                    }
                    upc_r2_loader64.UPC_ProductListFreeImpl(inContext, ptrProductList);
                }
                UPC_TaskResult<UPC_Product[]> upc_TaskResult = new UPC_TaskResult<UPC_Product[]>(inResult, array, inResultOptData);
                if (upc_TaskResult.Result == UPC_Result.UPC_Result_Ok && productListDelegate != null)
                {
                    productListDelegate(upc_TaskResult.ResultData);
                }
            });
            var funcpointer = Marshal.GetFunctionPointerForDelegate<UPC_CallbackImpl>(new UPC_CallbackImpl(HandleRequest));
            Debug.PWDebug($"[UPC_ProductListCallback] Func: {funcpointer}");
            var ret = upc_r2_loader64.UPC_ProductListGetImpl(inContext, inOptUserIdUtf8, inFilter, ref ptrProductList, funcpointer, new IntPtr(num));
            Debug.PWDebug($"[UPC_ProductListCallback] Ret: {ret}");
            if (ret < 0)
            {
                CancelRequest(num);
            }
        }



        public static string UPC_IdGet(IntPtr inContext)
        {

            string outId = string.Empty;
            int result = (int)upc_r2_loader64.UPC_IdGet_ExtendedImpl(inContext, ref outId);
            return outId;
        }

        public static UPC_Result UPC_EventRegisterHandler(IntPtr inContext, List<UPC_EventType> inType, UPC_EventHandler handler)
        {
            if (s_eventHandler != null)
            {
                throw new InvalidOperationException("UPC_EventRegisterHandler already request previously");
            }
            s_eventHandler = handler;
            UPC_Result upc_Result = UPC_Result.UPC_Result_Ok;
            foreach (UPC_EventType item in inType)
            {
                upc_Result |= upc_r2_loader64.UPC_EventRegisterHandlerImpl(inContext, item, HandleEventDelegater, IntPtr.Zero);
                Debug.PWDebug($"[UPC_EventRegisterHandler] Reged: {upc_Result}");
            }
            if (upc_Result != UPC_Result.UPC_Result_Ok)
            {
                s_eventHandler = null;
            }
            return upc_Result;
        }
        public static UPC_StorageFile StorageFileGet(IntPtr inContext, string fileName)
        {
            long num = 0L;
            UPC_StorageFile result = default(UPC_StorageFile);
            if (upc_r2_loader64.UPC_StorageFileListGetImpl(inContext, ref num) == 0)
            {
                UPC_StorageFileList upc_StorageFileList = Global.IntPtrToStruct<UPC_StorageFileList>(new IntPtr(num));
                UPC_StorageFile[] array = new UPC_StorageFile[upc_StorageFileList.count];
                int num2 = Marshal.SizeOf(typeof(IntPtr));
                int num3 = 0;
                while ((long)num3 < (long)((ulong)upc_StorageFileList.count))
                {
                    IntPtr ptr = new IntPtr(upc_StorageFileList.list.ToInt64() + (long)(num2 * num3));
                    IntPtr ptr2 = new IntPtr(Global.IntPtrToStruct<long>(ptr));
                    UPC_StorageFile upc_StorageFile = Global.IntPtrToStruct<UPC_StorageFile>(ptr2);
                    array[num3] = upc_StorageFile;
                    if (fileName.Equals(upc_StorageFile.fileNameUtf8))
                    {
                        result = upc_StorageFile;
                        break;
                    }
                    num3++;
                }
                upc_r2_loader64.UPC_StorageFileListFreeImpl(inContext, num);
            }
            return result;
        }

        public static void UPC_AchievementImageGet(IntPtr inContext, uint inId, GenericUpcDelegate<byte[]> callback, object inResultOptData = null)
        {
            long ptrImage = 0L;
            long num = PushRequest(delegate (int inResult)
            {
                byte[] array = null;
                if (inResult == 0)
                {
                    IntPtr source = Global.IntPtrToStruct<IntPtr>(new IntPtr(ptrImage));
                    int num3 = 16384;
                    array = new byte[num3];
                    Marshal.Copy(source, array, 0, num3);
                    upc_r2_loader64.UPC_AchievementImageFreeImpl(inContext, ptrImage);
                }
                if (callback != null)
                {
                    callback(new UPC_TaskResult<byte[]>(inResult, array, inResultOptData));
                }
            });
            int num2 = upc_r2_loader64.UPC_AchievementImageGetImpl(inContext, inId, ref ptrImage, Marshal.GetFunctionPointerForDelegate<UPC_CallbackImpl>(new UPC_CallbackImpl(HandleRequest)), new IntPtr(num));
            if (num2 <= 0)
            {
                CancelRequest(num);
                if (callback != null)
                {
                    callback(new UPC_TaskResult<byte[]>(num2, null, inResultOptData));
                }
            }
        }

        public static void UPC_OverlayShow(IntPtr inContext, UPC_OverlaySection inSection, Action<UPC_TaskResult> onDone, object inResultOptData = null)
        {
            long num = PushRequest(delegate (int inResult)
            {
                onDone(new UPC_TaskResult(inResult, inResultOptData));
            });
            int num2 = upc_r2_loader64.UPC_OverlayShowImpl(inContext, (uint)inSection, Marshal.GetFunctionPointerForDelegate<UPC_CallbackImpl>(new UPC_CallbackImpl(HandleRequest)), new IntPtr(num));
            Debug.PWDebug($"[UPC_OverlayShow] Return {num2}");

            if (num2 < 0)
            {
                CancelRequest(num);
                onDone(new UPC_TaskResult(num2, inResultOptData));
            }
        }

        #endregion
        #region UPC_EventEnumerator
        public class UPC_EventEnumerator
        {
            public UPC_EventEnumerator(IntPtr context)
            {
                if (context == IntPtr.Zero)
                {
                    throw new InvalidOperationException("UPC_Init() must have been called and UPC_ContextCreate() must have been called before constructing UPC_EventEnumerator");
                }
                this.m_context = context;
            }

            static UPC_EventEnumerator()
            {
                Type[] nestedTypes = typeof(Structs).GetNestedTypes();
                Type typeFromHandle = typeof(IUPC_EventData);
                int num = -1;
                foreach (Type type in nestedTypes)
                {
                    if (!type.IsByRef && type != typeFromHandle && typeFromHandle.IsAssignableFrom(type))
                    {
                        num = Math.Max(num, Marshal.SizeOf(type));
                    }
                }
                MaxEventDataSize = UPC_Event.EventDataMemoryPadding + num;
                Debug.WriteDebug($"[UPC_EventEnumerator] MaxEventDataSize is {MaxEventDataSize}");
            }

            public UPC_EventNextPoll_Result UPC_EventNextPoll()
            {
                if (this.m_eventDataPtr == IntPtr.Zero)
                {
                    this.m_eventDataPtr = Marshal.AllocHGlobal(MaxEventDataSize);
                }
                UPC_Result resultCode = upc_r2_loader64.UPC_EventNextPollImpl(this.m_context, this.m_eventDataPtr);
                if (resultCode != UPC_Result.UPC_Result_Ok)
                {
                    return new UPC_EventNextPoll_Result
                    {
                        resultCode = resultCode
                    };
                }
                UPC_Event result = new UPC_Event(this.m_eventDataPtr);
                this.m_eventDataPtr = IntPtr.Zero;
                return new UPC_EventNextPoll_Result
                {
                    resultEvent = result,
                    resultCode = resultCode
                };
            }

            public UPC_Event UPC_EventNextPeek(out UPC_Result resultCode)
            {
                if (this.m_eventDataPtr == IntPtr.Zero)
                {
                    this.m_eventDataPtr = Marshal.AllocHGlobal(MaxEventDataSize);
                }
                resultCode = upc_r2_loader64.UPC_EventNextPeekImpl(this.m_context, this.m_eventDataPtr);
                if (resultCode != UPC_Result.UPC_Result_Ok)
                {
                    return null;
                }
                UPC_Event result = new UPC_Event(this.m_eventDataPtr);
                this.m_eventDataPtr = IntPtr.Zero;
                return result;
            }

            private IntPtr m_eventDataPtr = IntPtr.Zero;
            private IntPtr m_context = IntPtr.Zero;
            private static int MaxEventDataSize;
        }
        #endregion
        #region UPC_Event
        public class UPC_Event : IDisposable
        {
            private IntPtr ptr;

            internal static readonly int EventDataMemoryPadding = 8;

            public bool IsValid
            {
                [CompilerGenerated]
                get
                {
                    return ptr != IntPtr.Zero;
                }
            }

            public UPC_EventType Type
            {
                [CompilerGenerated]
                get
                {
                    return (UPC_EventType)Global.IntPtrToStruct<uint>(ptr);
                }
            }

            public UPC_Event(IntPtr ptr)
            {
                this.ptr = ptr;
            }

            public T GetAs<T>() where T : struct, IUPC_EventData
            {
                return Global.IntPtrToStruct<T>(new IntPtr(ptr.ToInt64() + EventDataMemoryPadding));
            }

            public void Dispose()
            {
                if (ptr != IntPtr.Zero)
                {
                    ptr = IntPtr.Zero;
                }
            }
        }
        #endregion
    }
}
