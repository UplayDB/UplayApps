using CoreLib;
using Downloader.Managers;
using Uplay.Download;
using Uplay.DownloadService;
using UplayKit;

namespace Downloader;

internal class SliceManager
{
    internal static List<UrlRsp.Types.DownloadUrls> SliceWorker<TAnySlice>(List<TAnySlice> list) where TAnySlice : notnull
    {
        List<string> listOfSliceIds = new();
        foreach (var slice in list)
        {
            if (slice == null)
            {
                Console.WriteLine("Your slice is Null?");
                throw new NullReferenceException("Slice is null!");
            }
            if (slice is Slice file_slice && file_slice != null)
            {
                if (file_slice.HasFileOffset) { Console.WriteLine("[!!!] FILE OFFSET! " + file_slice.FileOffset); }
                string sliceId = Convert.ToHexString(file_slice.DownloadSha1.ToArray());
                listOfSliceIds.Add(GetSlicePath(sliceId));
            }
            if (slice is string slice_str && slice_str != null)
                listOfSliceIds.Add(GetSlicePath(slice_str));

        }
        SocketManager.CheckOW(Config.ProductId);
        if (SocketManager.Download == null)
        {
            Console.WriteLine("Download is null!");
            Environment.Exit(1);
        }
        else if (SocketManager.Download.IsConnectionClosed)
        {
            SocketManager.Download.Reconnect();
            if (SocketManager.Download.IsConnectionClosed)
            {
                bool InitTrue = SocketManager.InitDownload();

                if (!InitTrue)
                {
                    Console.WriteLine("The ownership Token has been expired! Quiting and please try again!");
                    Environment.Exit(1);
                }
            }
            else
            {
                Console.WriteLine("Cannot reconnect!");
                Environment.Exit(1);
            }
        }
        return SocketManager.Download.GetUrlList(Config.ProductId, listOfSliceIds);
    }

    public static string GetSlicePath(string Slice)
    {
        uint version = ManifestManager.ParsedManifest.Version;
        if (version == 2)
        {
            Console.WriteLine("Version 2!!!!!!!!!!!");
            throw new Exception("Version 2 in slice hasnt been reversed yet!, Please get help from github!");
        }
        if (version == 3)
        {
            return ($"slices_v3/{Formatters.FormatSliceHashChar(Slice)}/{Slice}");
        }
        else
        {
            return ($"slices/{Slice}");
        }
    }

    public static byte[] Decompress(byte[] downloadedSlice, ulong outputsize)
    {
        return DeComp.Decompress(ManifestManager.ParsedManifest.IsCompressed, ManifestManager.ParsedManifest.CompressionMethod.ToString(), downloadedSlice, outputsize);
    }

}
