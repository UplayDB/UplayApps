using System.Runtime.InteropServices;

namespace Downloader
{
    internal class LzhamWrapper
    {
        [DllImport("lzham_api", EntryPoint = "decompress")]
        public static extern int Decompress([In] byte[] input, ulong inputsize, [Out] byte[] output, ulong outputsize);

        [DllImport("lzham_api", EntryPoint = "Test")]
        public static extern int Test();
    }
}
