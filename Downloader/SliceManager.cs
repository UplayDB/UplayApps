using Google.Protobuf;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Uplay.Download;
using UplayKit;
using UplayKit.Connection;
using ZstdNet;
using CoreLib;

namespace Downloader
{
    internal class SliceManager
    {
        public static List<string> SliceWorker(List<Slice> slices, DownloadConnection downloadConnection, uint Version)
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
            return GetUrlsForSlices(listOfSliceIds, downloadConnection);
        }

        public static List<string> SliceWorker(List<ByteString> slices, DownloadConnection downloadConnection, uint Version)
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
            return GetUrlsForSlices(listOfSliceIds, downloadConnection);
        }

        public static List<string> SliceWorker(List<string> slices, DownloadConnection downloadConnection, uint Version)
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
            return GetUrlsForSlices(listOfSliceIds, downloadConnection);
        }

        public static List<string> GetUrlsForSlices(List<string> listOfSliceIds, DownloadConnection downloadConnection)
        {
            Program.CheckOW(Downloader.Config.ProductId);
            Program.UbiTicketReNew();
            if (downloadConnection.isConnectionClosed)
            {
                downloadConnection.Reconnect();
                if (downloadConnection.isServiceSuccess)
                {
                    bool InitTrue = downloadConnection.InitDownloadToken(Program.OWToken);

                    if (!InitTrue)
                    {
                        Console.WriteLine("The ownership Token has been expired! Quiting and please try again!");
                        Environment.Exit(1);
                    }
                }
                else
                {
                    Console.WriteLine("Cannot made to reconnect!");
                    Environment.Exit(1);
                }

            }
            return downloadConnection.GetUrlList(Downloader.Config.ProductId, listOfSliceIds);
        }
        public static byte[] Decompress(Saving.Root saved, byte[] downloadedSlice, ulong outputsize)
        {
            return DeComp.Decompress(saved.Compression.IsCompressed, saved.Compression.Method, downloadedSlice, outputsize);
        }
    }
}
