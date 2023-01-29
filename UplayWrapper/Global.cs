using System.Runtime.InteropServices;
using static UplayWrapper.Enums;
using static UplayWrapper.Structs;

namespace UplayWrapper
{
    public class Global
    {
        public static T IntPtrToStruct<T>(IntPtr ptr) where T : struct
        {
            return (T)((object)Marshal.PtrToStructure(ptr, typeof(T)));
        }
        public static TElement[] CreateStructureArray<TElement>(int inResult, long ptr) where TElement : struct
        {
            return CreateStructureArray<TElement>((UPC_Result)inResult, ptr);
        }

        public static TElement[] CreateStructureArray<TElement>(UPC_Result inResult, long ptr) where TElement : struct
        {
            TElement[] array = null;
            if (inResult != UPC_Result.UPC_Result_Ok)
            {
                return array;
            }
            UPC_GenericArray upc_GenericArray = IntPtrToStruct<UPC_GenericArray>(new IntPtr(ptr));
            array = new TElement[upc_GenericArray.count];
            int num = Marshal.SizeOf(typeof(IntPtr));
            int num2 = 0;
            while ((long)num2 < (long)((ulong)upc_GenericArray.count))
            {
                IntPtr ptr2 = new IntPtr(upc_GenericArray.list.ToInt64() + (long)(num * num2));
                TElement telement = IntPtrToStruct<TElement>(new IntPtr(IntPtrToStruct<long>(ptr2)));
                array[num2] = telement;
                num2++;
            }
            return array;
        }
    }
}
