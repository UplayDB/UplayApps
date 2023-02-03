using Google.Protobuf;
using RestSharp;
using System.Drawing;
using UplayKit.Connection;
using static Downloader.Saving;
using UDFile = Uplay.Download.File;

namespace Downloader
{
    internal class ByteDownloader
    {
        public static List<byte[]> DownloadBytes(string savingpath, uint productId, UDFile file, List<string> hashlist, DownloadConnection downloadConnection, Root saving)
        {
            List<byte[]> bytes = new();
            var rc = new RestClient();

            SliceInfo sliceInfo = new();
            List<SliceInfo> sliceInfoList = new();
            saving = Read(savingpath);
            var savefile = saving.Verify.Files.Where(x => x.Name == file.Name).FirstOrDefault();
            if (savefile == null)
            {
                saving.Verify.Files.Add(new()
                {
                    Name = file.Name,
                    SliceInfo = new()
                });
            }

            var urls = SliceManager.SliceWorker(hashlist, downloadConnection, productId, (uint)saving.Version);
            for (int urlcounter = 0; urlcounter < urls.Count; urlcounter++)
            {
                var url = urls[urlcounter];
                var downloadedSlice = rc.DownloadData(new(url));
                if (downloadedSlice == null)
                {
                    Console.WriteLine("This should never happen");
                    Save(saving, savingpath);
                    Environment.Exit(1);
                }
                else
                {
                    var sliceId = hashlist[urlcounter];
                    saving.Work.CurrentId = sliceId;
                    // this prevent for failing "out of array"
                    if ((urlcounter + 1) < hashlist.Count)
                    {
                        saving.Work.NextId = hashlist[(urlcounter + 1)];
                    }
                    else
                    {
                        saving.Work.NextId = "";
                    }
                    ulong size = (ulong)downloadedSlice.LongLength;

                    var slice = file.SliceList.Where(x => Convert.ToHexString(x.DownloadSha1.ToArray()) == sliceId).SingleOrDefault();
                    if (slice != null)
                    {
                        size = slice.Size;
                    }

                    //for saving the slices
                    var decompressedslice = SliceManager.Decompress(saving, downloadedSlice, size);
                    sliceInfo.DecompressedSHA = Verifier.GetSHA1Hash(decompressedslice);
                    sliceInfo.CompressedSHA = sliceId;
                    sliceInfo.DownloadedSize = decompressedslice.Length;
                    sliceInfoList.Add(sliceInfo);
                    sliceInfo = new();
                    bytes.Add(decompressedslice);
                }
            }
            saving.Verify.Files.SingleOrDefault(x => x.Name == file.Name).SliceInfo.AddRange(sliceInfoList);
            Save(saving, savingpath);
            return bytes;
        }

        public static List<byte[]> DownloadBytes(string savingpath, uint productId, UDFile file, List<ByteString> bytestring, DownloadConnection downloadConnection, Root saving)
        {
            List<byte[]> bytes = new();
            var rc = new RestClient();

            SliceInfo sliceInfo = new();
            List<SliceInfo> sliceInfoList = new();
            saving = Read(savingpath);
            var savefile = saving.Verify.Files.Where(x => x.Name == file.Name).FirstOrDefault();
            if (savefile == null)
            {
                saving.Verify.Files.Add(new()
                {
                    Name = file.Name,
                    SliceInfo = new()
                });
            }

            var urls = SliceManager.SliceWorker(bytestring, downloadConnection, productId, (uint)saving.Version);
            for (int urlcounter = 0; urlcounter < urls.Count; urlcounter++)
            {
                var url = urls[urlcounter];
                var downloadedSlice = rc.DownloadData(new(url));
                if (downloadedSlice == null)
                {
                    Console.WriteLine("This should never happen");
                    Save(saving, savingpath);
                    Environment.Exit(1);
                }
                else
                {
                    var sliceId = Convert.ToHexString(bytestring[urlcounter].ToArray());
                    saving.Work.CurrentId = sliceId;
                    // this prevent for failing "out of array"
                    if ((urlcounter + 1) < bytestring.Count)
                    {
                        saving.Work.NextId = Convert.ToHexString(bytestring[(urlcounter + 1)].ToArray());
                    }
                    else
                    {
                        saving.Work.NextId = "";
                    }
                    ulong size = (ulong)downloadedSlice.LongLength;

                    var slice = file.SliceList.Where(x => Convert.ToHexString(x.DownloadSha1.ToArray()) == sliceId).SingleOrDefault();
                    if (slice != null)
                    {
                        size = slice.Size;
                    }

                    //for saving the slices
                    var decompressedslice = SliceManager.Decompress(saving, downloadedSlice, size);
                    sliceInfo.DecompressedSHA = Verifier.GetSHA1Hash(decompressedslice);
                    sliceInfo.CompressedSHA = sliceId;
                    sliceInfo.DownloadedSize = decompressedslice.Length;
                    sliceInfoList.Add(sliceInfo);
                    sliceInfo = new();
                    bytes.Add(decompressedslice);
                }
            }
            saving.Verify.Files.SingleOrDefault(x => x.Name == file.Name).SliceInfo.AddRange(sliceInfoList);
            Save(saving, savingpath);
            return bytes;
        }

        public static List<byte[]> DownloadBytes(string savingpath, uint productId, string FileName, List<Uplay.Download.Slice> slices, DownloadConnection downloadConnection, Root saving)
        {
            List<byte[]> bytes = new();
            var rc = new RestClient();

            SliceInfo sliceInfo = new();
            List<SliceInfo> sliceInfoList = new();
            saving = Read(savingpath);
            var savefile = saving.Verify.Files.Where(x => x.Name == FileName).FirstOrDefault();
            if (savefile == null)
            {
                saving.Verify.Files.Add(new()
                {
                    Name = FileName,
                    SliceInfo = new()
                });
            }

            var urls = SliceManager.SliceWorker(slices.ToList(), downloadConnection, productId, (uint)saving.Version);
            for (int urlcounter = 0; urlcounter < urls.Count; urlcounter++)
            {
                var url = urls[urlcounter];
                var downloadedSlice = rc.DownloadData(new(url));
                if (downloadedSlice == null)
                {
                    Console.WriteLine("This should never happen");
                    Save(saving, savingpath);
                    Environment.Exit(1);
                }
                else
                {
                    var sliceId = Convert.ToHexString(slices[urlcounter].DownloadSha1.ToArray());
                    saving.Work.CurrentId = sliceId;
                    // this prevent for failing "out of array"
                    if ((urlcounter + 1) < slices.Count)
                    {
                        saving.Work.NextId = Convert.ToHexString(slices[(urlcounter + 1)].DownloadSha1.ToArray());
                    }
                    else
                    {
                        saving.Work.NextId = "";
                    }
                    
                    //for saving the slices
                    var decompressedslice = SliceManager.Decompress(saving, downloadedSlice, slices[urlcounter].Size);
                    sliceInfo.DecompressedSHA = Verifier.GetSHA1Hash(decompressedslice);
                    sliceInfo.DownloadedSize = decompressedslice.Length;
                    sliceInfo.CompressedSHA = sliceId;
                    sliceInfoList.Add(sliceInfo);
                    sliceInfo = new();
                    bytes.Add(decompressedslice);
                }
            }
            saving.Verify.Files.SingleOrDefault(x => x.Name == FileName).SliceInfo.AddRange(sliceInfoList);
            Save(saving, savingpath);
            return bytes;
        }
    }
}
