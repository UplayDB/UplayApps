using Google.Protobuf;
using RestSharp;
using UplayKit.Connection;
using static Downloader.Saving;
using File = System.IO.File;
using UDFile = Uplay.Download.File;

namespace Downloader
{
    internal class Downloader
    {
        public static void DownloadWorker(List<UDFile> files, string downloadPath, DownloadConnection downloadConnection, uint productId, Saving.Root saving)
        {
            Console.WriteLine("\n\t\tDownloading Started!");
            int filecounter = 0;
            foreach (var file in files)
            {
                if (file.Size == 0)
                    continue;

                filecounter++;

                List<string> sliceListIds = new();

                foreach (var sl in file.SliceList)
                {
                    sliceListIds.Add(Convert.ToHexString(sl.DownloadSha1.ToArray()));
                }

                List<string> sliceIds = new();

                foreach (var sl in file.Slices)
                {
                    sliceListIds.Add(Convert.ToHexString(sl.ToArray()));
                }
                CheckCurrentFile(downloadPath, productId, file, downloadConnection, saving);
                saving.Work.FileInfo = new()
                {
                    Name = file.Name,
                    IDs = new()
                    {
                        SliceList = sliceListIds,
                        Slices = sliceIds
                    }
                };
                Console.WriteLine($"\t\tFile {file.Name} started ({filecounter}/{files.Count})");
                DownloadFile(downloadPath, productId, file, downloadConnection, saving);
            }
            Console.WriteLine($"\t\tDownload for app {productId} is done!");
        }

        public static void CheckCurrentFile(string downloadPath, uint productId, UDFile file, DownloadConnection downloadConnection, Root saving)
        {

            if (saving.Work.FileInfo.Name != file.Name)
                return;

            var curId = saving.Work.CurrentId;
            var verifile = saving.Verify.Files.Where(x => x.Name == file.Name).FirstOrDefault();
            if (verifile == null)
                return;

            List<string> slicesToDownload = new();
            int index = 0;
            uint Size = 0;
            if (file.SliceList.Where(x => x.HasDownloadSha1).Count() > 0)
            {
                index = saving.Work.FileInfo.IDs.SliceList.FindIndex(0, x => x == curId);
                slicesToDownload = saving.Work.FileInfo.IDs.SliceList.Skip(index).ToList();

            }
            else
            {
                index = saving.Work.FileInfo.IDs.Slices.FindIndex(0, x => x == curId);
                slicesToDownload = saving.Work.FileInfo.IDs.Slices.Skip(index).ToList();
            }

            if (slicesToDownload.Count == 0)
                return;

            var sizelister = file.SliceList.Take(index).ToList();
            foreach (var sizer in sizelister)
            {
                Size += sizer.Size;
            }

            var fullPath = Path.Combine(downloadPath, file.Name);
            var fileInfo = new System.IO.FileInfo(fullPath);
            if (fileInfo.Length != Size)
            {
                Console.WriteLine("Something isnt right, +/- chunk??");
                return;
            }
            Console.WriteLine($"\t\tRedownloading File {file.Name}!");
            RedownloadSlices(downloadPath, productId, slicesToDownload, file, downloadConnection, saving);
        }

        public static void RedownloadSlices(string downloadPath, uint productId, List<string> slicesToDownload, UDFile file, DownloadConnection downloadConnection, Root saving)
        {
            var fullPath = Path.Combine(downloadPath, file.Name);

            var fs = File.OpenWrite(fullPath);
            var dlbytes = DownloadBytes(downloadPath, productId, file.Name, slicesToDownload, downloadConnection, saving);
            foreach (var barray in dlbytes)
            {
                fs.Write(barray);
                fs.Flush();
            }
            fs.Close();
        }

        public static void DownloadFile(string downloadPath, uint productId, UDFile file, DownloadConnection downloadConnection, Root saving)
        {
            var savingpath = Path.Combine(downloadPath, ".UD\\saved.bin");
            var fullPath = Path.Combine(downloadPath, file.Name);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            var fs = File.OpenWrite(fullPath);
            if (file.SliceList.Where(x => x.HasDownloadSha1).Count() > 0)
            {
                if (file.SliceList.Count > 30)
                {
                    Console.WriteLine("\tBig Slice! " + file.SliceList.Count);
                    var splittedList = file.SliceList.ToList().SplitList<Uplay.Download.Slice>(10);
                    for (int listcounter = 0; listcounter < splittedList.Count; listcounter++)
                    {
                        var spList = splittedList[listcounter];
                        var dlbytes = DownloadBytes(savingpath, productId, file.Name, spList.ToList(), downloadConnection, saving);
                        foreach (var barray in dlbytes)
                        {
                            fs.Write(barray);
                            fs.Flush();
                        }
                        Thread.Sleep(10);
                    }
                }
                else
                {
                    var dlbytes = DownloadBytes(savingpath, productId, file.Name, file.SliceList.ToList(), downloadConnection, saving);
                    foreach (var barray in dlbytes)
                    {
                        fs.Write(barray);
                        fs.Flush();
                    }
                }
            }
            else
            {
                if (file.Slices.Count > 30)
                {
                    Console.WriteLine("\tBig Slice! " + file.SliceList.Count);
                    var splittedList = file.Slices.ToList().SplitList<ByteString>(10);
                    for (int listcounter = 0; listcounter < splittedList.Count; listcounter++)
                    {
                        var spList = splittedList[listcounter];
                        var dlbytes = DownloadBytes(savingpath, productId, file.Name, spList.ToList(), downloadConnection, saving);
                        foreach (var barray in dlbytes)
                        {
                            fs.Write(barray);
                            fs.Flush();
                        }

                        if (listcounter % 10 == 0)
                        {
                            Console.WriteLine("%");
                            Thread.Sleep(1000);
                        }
                        Console.WriteLine("sleep");
                        Thread.Sleep(10);
                    }

                }
                else
                {
                    var dlbytes = DownloadBytes(savingpath, productId, file.Name, file.Slices.ToList(), downloadConnection, saving);
                    foreach (var barray in dlbytes)
                    {
                        fs.Write(barray);
                        fs.Flush();
                    }
                }

            }
            fs.Close();
            Console.WriteLine($"\t\tFile {file.Name} finished");
        }

        public static List<byte[]> DownloadBytes(string savingpath, uint productId, string FileName, List<string> hashlist, DownloadConnection downloadConnection, Root saving)
        {
            List<byte[]> bytes = new();
            var rc = new RestClient();

            SliceInfo sliceInfo = new();

            Saving.File savefile = new()
            {
                Name = FileName,
                SliceInfo = new()
            };

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
                    //for saving the slices
                    var decompressedslice = SliceManager.Decompress(saving, downloadedSlice);
                    sliceInfo.DecompressedSHA = Verifier.GetSHA1Hash(decompressedslice);
                    sliceInfo.CompressedSHA = sliceId;
                    sliceInfo.DownloadedSize = decompressedslice.Length;
                    savefile.SliceInfo.Add(sliceInfo);
                    sliceInfo = new();
                    Save(saving, savingpath);
                    bytes.Add(decompressedslice);
                }
            }
            saving.Verify.Files.Add(savefile);
            Save(saving, savingpath);
            return bytes;
        }

        public static List<byte[]> DownloadBytes(string savingpath, uint productId, string FileName, List<ByteString> bytestring, DownloadConnection downloadConnection, Root saving)
        {
            List<byte[]> bytes = new();
            var rc = new RestClient();

            SliceInfo sliceInfo = new();

            Saving.File savefile = new()
            {
                Name = FileName,
                SliceInfo = new()
            };

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
                    //for saving the slices
                    var decompressedslice = SliceManager.Decompress(saving, downloadedSlice);
                    sliceInfo.DecompressedSHA = Verifier.GetSHA1Hash(decompressedslice);
                    sliceInfo.CompressedSHA = sliceId;
                    sliceInfo.DownloadedSize = decompressedslice.Length;
                    savefile.SliceInfo.Add(sliceInfo);
                    sliceInfo = new();
                    Save(saving, savingpath);
                    bytes.Add(decompressedslice);
                }
            }
            saving.Verify.Files.Add(savefile);
            Save(saving, savingpath);
            return bytes;
        }

        public static List<byte[]> DownloadBytes(string savingpath, uint productId, string FileName, List<Uplay.Download.Slice> slices, DownloadConnection downloadConnection, Root saving)
        {
            List<byte[]> bytes = new();
            var rc = new RestClient();

            SliceInfo sliceInfo = new();

            Saving.File savefile = new()
            {
                Name = FileName,
                SliceInfo = new()
            };

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
                    var decompressedslice = SliceManager.Decompress(saving, downloadedSlice);
                    sliceInfo.DecompressedSHA = Verifier.GetSHA1Hash(decompressedslice);
                    sliceInfo.DownloadedSize = decompressedslice.Length;
                    sliceInfo.CompressedSHA = sliceId;
                    savefile.SliceInfo.Add(sliceInfo);
                    sliceInfo = new();
                    Save(saving, savingpath);
                    bytes.Add(decompressedslice);
                }
            }
            saving.Verify.Files.Add(savefile);
            Save(saving, savingpath);
            return bytes;
        }
    }
}
