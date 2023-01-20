using System.Runtime.InteropServices;

namespace UplayWrapper
{
    public class Global
    {
        public static T IntPtrToStruct<T>(IntPtr ptr) where T : struct
        {
            return (T)((object)Marshal.PtrToStructure(ptr, typeof(T)));
        }
    }
}
