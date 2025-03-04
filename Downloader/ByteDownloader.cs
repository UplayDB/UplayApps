using Downloader.Tools;
using Google.Protobuf;
using Serilog;
using UplayKit;
using static Downloader.Saving;
using UDFile = Uplay.Download.File;

namespace Downloader;

internal class ByteDownloader
{
    public static List<byte[]> DownloadBytes(UDFile file, List<ByteString> bytestring)
    {
        List<string> bytesstringlist = new(bytestring.Select(x => Convert.ToHexString(x.ToArray())));
        return DownloadBytes<string>(file, bytesstringlist);
    }

    public static List<byte[]> DownloadBytes<TAnySlice>(UDFile file, List<TAnySlice> list) where TAnySlice : notnull
    {
        List<byte[]> bytes = new();
        List<SliceInfo> sliceInfoList = new();
        var savefile = DLWorker.VerifyFiles.FirstOrDefault(x => x.Name == file.Name);
        if (savefile == null)
        {
            DLWorker.VerifyFiles.Add(savefile = new()
            {
                Name = file.Name,
                SliceInfo = new()
            });

        }
        var fileinfo = DLWorker.WorkInfos.FirstOrDefault(x => x.Name == file.Name);
        if (fileinfo == null)
        {
            DLWorker.WorkInfos.Add(fileinfo = new()
            {
                Name = file.Name,
                IDs = new(),
            });
        }
        var urls = SliceManager.SliceWorker(list);
        for (int urlcounter = 0; urlcounter < urls.Count; urlcounter++)
        {
            var downloadUrls = urls[urlcounter];
            bool IsDownloaded = false;
            foreach (var url in downloadUrls.Urls)
            {
                if (string.IsNullOrEmpty(url))
                    continue;
                var downloadedSlice = FileGetter.DownloadFromURL(url);
                if (downloadedSlice == null)
                {
                    Log.Warning("{file} for Slice could not be downloaded! {url}", file.Name, url);
                    continue;
                }
                ulong size = (ulong)downloadedSlice.LongLength;
                var list_url = list[urlcounter];
                if (list_url is string)
                {
                    string? sliceId = list_url as string;
                    ArgumentNullException.ThrowIfNull(sliceId);
                    fileinfo.CurrentId = sliceId;
                    if ((urlcounter + 1) < list.Count)
                    {
                        if (list[urlcounter + 1] is string slice_one)
                            fileinfo.NextId = slice_one;
                        else
                            fileinfo.NextId = "";
                    }
                    else
                    {
                        fileinfo.NextId = "";
                    }
                    var slice = file.SliceList.Where(x => Convert.ToHexString(x.DownloadSha1.ToArray()) == fileinfo.CurrentId).SingleOrDefault();
                    if (slice != null)
                    {
                        size = slice.Size;
                    }
                }

                if (list_url is Uplay.Download.Slice)
                {
                    Uplay.Download.Slice? download_slice = list_url as Uplay.Download.Slice;
                    ArgumentNullException.ThrowIfNull(download_slice);
                    var sliceId = Convert.ToHexString(download_slice.DownloadSha1.ToArray());
                    fileinfo.CurrentId = sliceId;
                    if ((urlcounter + 1) < list.Count)
                    {
                        if (list[urlcounter + 1] is Uplay.Download.Slice slice_one)
                            fileinfo.NextId = Convert.ToHexString(slice_one.DownloadSha1.ToArray());
                        else
                            fileinfo.NextId = "";
                    }
                    else
                    {
                        fileinfo.NextId = "";
                    }
                    size = download_slice.Size;
                }

                if (Config.DownloadAsChunks)
                {
                    bytes.Add(downloadedSlice);
                }
                else
                {
                    //for saving the slices
                    var decompressedslice = SliceManager.Decompress(downloadedSlice, size);
                    if (!sliceInfoList.Any(x => x.CompressedSHA == fileinfo.CurrentId))
                    {
                        sliceInfoList.Add(new()
                        {
                            DecompressedSHA = Verifier.GetSHA1Hash(decompressedslice),
                            CompressedSHA = fileinfo.CurrentId,
                            DownloadedSize = downloadedSlice.Length,
                            DecompressedSize = decompressedslice.Length,
                        });
                    }
                    bytes.Add(decompressedslice);
                }
                   
                IsDownloaded = true;
                break;
            }

            if (!IsDownloaded)
            {
                Log.Error("This should never happen");
                Environment.Exit(1);
            }
        }
        savefile.SliceInfo.AddRange(sliceInfoList);
        return bytes;
    }
}
