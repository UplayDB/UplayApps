using Google.Protobuf;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO.Compression;
using Uplay.Download;
using UplayKit;
using UplayKit.Connection;
using ZstdNet;

namespace Downloader
{
    internal class SliceManager
    {
        public static List<string> SliceWorker(List<Slice> slices, DownloadConnection downloadConnection, uint productId, uint Version)
        {
            List<string> listOfSliceIds = new();
            foreach (var slice in slices)
            {
                if (slice == null)
                {
                    Console.WriteLine("Your slice is Null?");
                }

                if (slice.HasFileOffset) { Console.WriteLine("[!!!] FILE OFFSET! " + slice.FileOffset); }
                string sliceId = Convert.ToHexString(slice.DownloadSha1.ToArray());
                if (Version == 3)
                {
                    listOfSliceIds.Add($"slices_v3/{Formatters.FormatSliceHashChar(sliceId)}/{sliceId}");
                }
                else
                {
                    listOfSliceIds.Add($"slices/{sliceId}");
                }
            }
            return GetUrlsForSlices(listOfSliceIds, downloadConnection, productId);
        }

        public static List<string> SliceWorker(List<ByteString> slices, DownloadConnection downloadConnection, uint productId, uint Version)
        {
            List<string> listOfSliceIds = new();
            foreach (var slice in slices)
            {
                string sliceId = Convert.ToHexString(slice.ToArray());
                if (Version == 3)
                {
                    listOfSliceIds.Add($"slices_v3/{Formatters.FormatSliceHashChar(sliceId)}/{sliceId}");
                }
                else
                {
                    listOfSliceIds.Add($"slices/{sliceId}");
                }
            }
            return GetUrlsForSlices(listOfSliceIds, downloadConnection, productId);
        }

        public static List<string> SliceWorker(List<string> slices, DownloadConnection downloadConnection, uint productId, uint Version)
        {
            List<string> listOfSliceIds = new();
            foreach (var slice in slices)
            {
                if (Version == 3)
                {
                    listOfSliceIds.Add($"slices_v3/{Formatters.FormatSliceHashChar(slice)}/{slice}");
                }
                else
                {
                    listOfSliceIds.Add($"slices/{slice}");
                }
            }
            return GetUrlsForSlices(listOfSliceIds, downloadConnection, productId);
        }

        public static List<string> GetUrlsForSlices(List<string> listOfSliceIds, DownloadConnection downloadConnection, uint productId)
        {
            Program.CheckOW(productId);
            Program.UbiTicketReNew();
            if (downloadConnection.isConnectionClosed)
            {
                downloadConnection.Reconnect();
                bool InitTrue = downloadConnection.InitDownloadToken(Program.OWToken);

                if (!InitTrue)
                {
                    Console.WriteLine("The ownership Token has been expired! Quiting and please try again!");
                    Environment.Exit(1);
                }
            }
            return downloadConnection.GetUrlList(productId, listOfSliceIds);
        }

        public static byte[] Decompress(Manifest manifest, byte[] downloadedSlice, ulong outputsize)
        {
            if (!manifest.IsCompressed)
            {
                return downloadedSlice;
            }

            switch (manifest.CompressionMethod) // check compression method
            {
                case CompressionMethod.Zstd:
                    Decompressor decompressorZstd = new();
                    var returner = decompressorZstd.Unwrap(downloadedSlice);
                    decompressorZstd.Dispose();
                    return returner;
                case CompressionMethod.Deflate:
                    var decompressor = new InflaterInputStream(new MemoryStream(downloadedSlice), new(false));
                    MemoryStream ms = new((int)outputsize);
                    decompressor.CopyTo(ms);
                    decompressor.Dispose();
                    return ms.ToArray();
                case CompressionMethod.Lzham:
                    return LzhamWrapper.Decompress(downloadedSlice, outputsize);
            }
            return downloadedSlice;
        }

        public static byte[] Decompress(Saving.Root saved, byte[] downloadedSlice, ulong outputsize)
        {
            if (!saved.Compression.IsCompressed)
            {
                return downloadedSlice;
            }

            switch (saved.Compression.Method) // check compression method
            {
                case "Zstd":
                    Decompressor decompressorZstd = new();
                    var returner = decompressorZstd.Unwrap(downloadedSlice);
                    decompressorZstd.Dispose();
                    return returner;
                case "Deflate":
                    /*
                    MemoryStream ms = new();
                    var compressor = new ZLibStream(new MemoryStream(downloadedSlice), CompressionLevel.SmallestSize);
                    ms.CopyTo(compressor);
                    compressor.Close();
                    return ms.ToArray();
                    */
                    var decompressor = new InflaterInputStream(new MemoryStream(downloadedSlice), new(false));
                    MemoryStream ms = new(10 * 1000);
                    decompressor.CopyTo(ms);
                    decompressor.Dispose();
                    return ms.ToArray();
                case "Lzham":
                    return LzhamWrapper.Decompress(downloadedSlice, outputsize);
            }
            return downloadedSlice;
        }
    }
}
