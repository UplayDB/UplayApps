using Google.Protobuf;
using RestSharp;
using UplayKit.Connection;
using static Downloader.Saving;
using UDFile = Uplay.Download.File;

namespace Downloader;

internal class ByteDownloader
{
    public static List<byte[]> DownloadBytes(UDFile file, List<ByteString> bytestring, DownloadConnection downloadConnection)
    {
        List<string> bytesstringlist = new();
        bytestring.ForEach(x => bytesstringlist.Add(Convert.ToHexString(x.ToArray())));
        return DownloadBytes<string>(file, bytesstringlist, downloadConnection);
    }

    public static List<byte[]> DownloadBytes<TAnySlice>(UDFile file, List<TAnySlice> list, DownloadConnection downloadConnection) where TAnySlice : notnull
    {
        List<byte[]> bytes = new();
        var rc = new RestClient();

        SliceInfo sliceInfo = new();
        List<SliceInfo> sliceInfoList = new();
        var saving = Read();
        var savefile = saving.Verify.Files.Where(x => x.Name == file.Name).FirstOrDefault();
        if (savefile == null)
        {
            saving.Verify.Files.Add(new()
            {
                Name = file.Name,
                SliceInfo = new()
            });
        }

        var urls = SliceManager.SliceWorker(list, downloadConnection, (uint)saving.Version);
        for (int urlcounter = 0; urlcounter < urls.Count; urlcounter++)
        {
            var downloadUrls = urls[urlcounter];
            bool IsDownloaded = false;
            foreach (var url in downloadUrls.Urls)
            {
                if (string.IsNullOrEmpty(url))
                    continue;
                var downloadedSlice = rc.DownloadData(new(url));
                if (downloadedSlice == null)
                    continue;
                ulong size = (ulong)downloadedSlice.LongLength;
                var list_url = list[urlcounter];
                if (list_url is string)
                {
                    string? sliceId = list_url as string;
                    ArgumentNullException.ThrowIfNull(sliceId);
                    saving.Work.CurrentId = sliceId;
                    if ((urlcounter + 1) < list.Count)
                    {
                        var slice_one = list[urlcounter + 1] as string;
                        if (slice_one != null)
                            saving.Work.NextId = slice_one;
                        else
                            saving.Work.NextId = "";
                    }
                    else
                    {
                        saving.Work.NextId = "";
                    }
                    var slice = file.SliceList.Where(x => Convert.ToHexString(x.DownloadSha1.ToArray()) == saving.Work.CurrentId).SingleOrDefault();
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
                    saving.Work.CurrentId = sliceId;
                    if ((urlcounter + 1) < list.Count)
                    {
                        Uplay.Download.Slice? slice_one = list[urlcounter + 1] as Uplay.Download.Slice;
                        if (slice_one != null)
                            saving.Work.NextId = Convert.ToHexString(slice_one.DownloadSha1.ToArray());
                        else
                            saving.Work.NextId = "";
                    }
                    else
                    {
                        saving.Work.NextId = "";
                    }
                    size = download_slice.Size;
                }
               
                if (!Config.DownloadAsChunks)
                {
                    //for saving the slices
                    var decompressedslice = SliceManager.Decompress(saving, downloadedSlice, size);
                    if (!sliceInfoList.Where(x => x.CompressedSHA == saving.Work.CurrentId).Any())
                    {
                        sliceInfo.DecompressedSHA = Verifier.GetSHA1Hash(decompressedslice);
                        sliceInfo.CompressedSHA = saving.Work.CurrentId;
                        sliceInfo.DownloadedSize = downloadedSlice.Length;
                        sliceInfo.DecompressedSize = decompressedslice.Length;
                        sliceInfoList.Add(sliceInfo);
                    }
                    sliceInfo = new();
                    bytes.Add(decompressedslice);
                }
                else
                    bytes.Add(downloadedSlice);
                IsDownloaded = true;
            }

            if (!IsDownloaded)
            {
                Console.WriteLine("This should never happen");
                Save(saving);
                Environment.Exit(1);
            }
        }

        var def = saving.Verify.Files.SingleOrDefault(x => x.Name == file.Name);
        if (def == null)
        {
            Console.WriteLine("Verifying files name returned null!");
            Save(saving);
            Environment.Exit(1);
        }
        def.SliceInfo.AddRange(sliceInfoList);
        Save(saving);
        return bytes;
    }
}
