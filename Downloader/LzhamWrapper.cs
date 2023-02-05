using System.Runtime.InteropServices;

namespace Downloader
{
    internal class LzhamWrapper
    {
        [DllImport("x64/lzham_api", EntryPoint = "decompress")]
        public static extern int Decompress_x64([In] byte[] input, ulong inputsize, [Out] byte[] output, ulong outputsize);

        /*
        [DllImport("x64/lzham_api", EntryPoint = "compressing")]
        public static extern int Compress_x64([In] byte[] input, ulong inputsize, [Out] byte[] output, ulong outputsize);
        
        [DllImport("x86/lzham_api", EntryPoint = "compressing")]
        public static extern int Compress_x86([In] byte[] input, ulong inputsize, [Out] byte[] output, ulong outputsize);
         */

        [DllImport("x86/lzham_api", EntryPoint = "decompress")]
        public static extern int Decompress_x86([In] byte[] input, ulong inputsize, [Out] byte[] output, ulong outputsize);

        public static byte[] Decompress(byte[] input, ulong outputsize)
        {
            if (IntPtr.Size == 4)
            {
                var lzh = new byte[((int)outputsize)];
                Decompress_x86(input, (ulong)input.LongLength, lzh, outputsize);
                return lzh;
            }
            else
            {
                var lzh = new byte[((int)outputsize)];
                Decompress_x64(input, (ulong)input.LongLength, lzh, outputsize);
                return lzh;
            }
        }
        /*
        public static byte[] Compress(byte[] input, ulong outputsize)
        {
            if (IntPtr.Size == 4)
            {
                var lzh = new byte[((int)outputsize)];
                Compress_x86(input, (ulong)input.LongLength, lzh, outputsize);
                return lzh;
            }
            else
            {
                var lzh = new byte[((int)outputsize)];
                Compress_x64(input, (ulong)input.LongLength, lzh, outputsize);
                return lzh;
            }
        }
        */
    }
}
