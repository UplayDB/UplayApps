using CoreLib;
using Uplay.Download;
using Uplay.DownloadService;
using UplayKit;
using UplayKit.Connection;

namespace Downloader;

internal class SliceManager
{
    internal static List<UrlRsp.Types.DownloadUrls> SliceWorker<TAnySlice>(List<TAnySlice> list, DownloadConnection downloadConnection, uint version) where TAnySlice : notnull
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
                listOfSliceIds.Add(GetSlicePath(sliceId, version));
            }
            if (slice is string slice_str && slice_str != null)
                listOfSliceIds.Add(GetSlicePath(slice_str, version));

        }
        Program.CheckOW(Config.ProductId);
        if (downloadConnection.IsConnectionClosed)
        {
            downloadConnection.Reconnect();
            if (downloadConnection.IsConnectionClosed)
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
                Console.WriteLine("Cannot reconnect!");
                Environment.Exit(1);
            }

        }
        Console.WriteLine("Slices: " + listOfSliceIds.Count);
        return downloadConnection.GetUrlList(Config.ProductId, listOfSliceIds);
    }

    public static string GetSlicePath(string Slice, uint Version)
    {
        if (Version == 2)
        {
            Console.WriteLine("Version 2!!!!!!!!!!!");
            new Exception("Version 2 in slice hasnt been reversed yet!, Please get help from github!");
        }
        if (Version == 3)
        {
            return ($"slices_v3/{Formatters.FormatSliceHashChar(Slice)}/{Slice}");
        }
        else
        {
            return ($"slices/{Slice}");
        }
    }

    public static byte[] Decompress(Saving.Root saved, byte[] downloadedSlice, ulong outputsize)
    {
        return DeComp.Decompress(saved.Compression.IsCompressed, saved.Compression.Method, downloadedSlice, outputsize);
    }

}
