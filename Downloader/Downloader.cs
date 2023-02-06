using Google.Protobuf;
using UplayKit;
using UplayKit.Connection;
using static Downloader.Saving;
using File = System.IO.File;
using UDFile = Uplay.Download.File;

namespace Downloader
{
    internal class Downloader
    {
        public static Config Config = new Config();

        public static void DownloadWorker(List<UDFile> files, string downloadPath, DownloadConnection downloadConnection, uint productId, Saving.Root saving)
        {
            var savingpath = Path.Combine(downloadPath, ".UD\\saved.bin");
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
                    sliceIds.Add(Convert.ToHexString(sl.ToArray()));
                }
                if (CheckCurrentFile(downloadPath, productId, file, downloadConnection, saving))
                    continue;





                saving = Read(savingpath);
                saving.Work.FileInfo = new()
                {
                    Name = file.Name,
                    IDs = new()
                    {
                        SliceList = sliceListIds,
                        Slices = sliceIds
                    }
                }; 
                Save(saving, savingpath);
                saving = Read(savingpath);
                Console.WriteLine($"\t\tFile {file.Name} started ({filecounter}/{files.Count}) [{Formatters.FormatFileSize(file.Size)}]");
                DownloadFile(downloadPath, productId, file, downloadConnection, saving);
            }
            Console.WriteLine($"\t\tDownload for app {productId} is done!");
        }

        public static bool CheckCurrentFile(string downloadPath, uint productId, UDFile file, DownloadConnection downloadConnection, Root saving)
        {

            if (saving.Work.FileInfo.Name != file.Name)
                return false;

            var curId = saving.Work.CurrentId;
            var NextId = saving.Work.NextId;
            var verifile = saving.Verify.Files.Where(x => x.Name == file.Name).FirstOrDefault();
            if (verifile == null)
                return false;

            List<string> slicesToDownload = new();
            int index = 0;
            uint Size = 0;
            if (saving.Compression.HasSliceSHA)
            {
                index = saving.Work.FileInfo.IDs.SliceList.FindIndex(0, x => x == curId);
                index += 1;
                slicesToDownload = saving.Work.FileInfo.IDs.SliceList.Skip(index).ToList();

            }
            else
            {
                index = saving.Work.FileInfo.IDs.Slices.FindIndex(0, x => x == curId);
                index += 1;
                slicesToDownload = saving.Work.FileInfo.IDs.Slices.Skip(index).ToList();
            }

            if (slicesToDownload.Count == 0)
                return false;

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
                File.WriteAllText("x.txt",(uint)fileInfo.Length + " != " +  Size + " " + sizelister.Count+ " "  + index + "\n" + curId + " " + NextId);
                // We try restore chunk after here
                //
                return false;
            }
            Console.WriteLine($"\t\tRedownloading File {file.Name}!");
            RedownloadSlices(downloadPath, productId, slicesToDownload, file, downloadConnection, saving);
            return true;
        }

        public static void RedownloadSlices(string downloadPath, uint productId, List<string> slicesToDownload, UDFile file, DownloadConnection downloadConnection, Root saving)
        {
            var fullPath = Path.Combine(downloadPath, file.Name);
            var savingpath = Path.Combine(downloadPath, ".UD\\saved.bin");

            var prevBytes = File.ReadAllBytes(fullPath);

            var fs = File.OpenWrite(fullPath);
            fs.Position = prevBytes.LongLength;
            if (slicesToDownload.Count > 30)
            {
                Console.WriteLine("\tBig Slice! " + slicesToDownload.Count);
                var splittedList = slicesToDownload.SplitList(10);
                for (int listcounter = 0; listcounter < splittedList.Count; listcounter++)
                {
                    var spList = splittedList[listcounter];
                    var dlbytes = ByteDownloader.DownloadBytes(savingpath, productId, file, spList.ToList(), downloadConnection, saving);
                    foreach (var barray in dlbytes)
                    {
                        fs.Write(barray);
                        fs.Flush(true);
                    }
                    if (listcounter % 10 == 0)
                    {
                        Console.WriteLine("%");
                        Thread.Sleep(1000);
                    }
                    Thread.Sleep(10);
                }
            }
            else
            {
                var dlbytes = ByteDownloader.DownloadBytes(savingpath, productId, file, slicesToDownload, downloadConnection, saving);
                foreach (var barray in dlbytes)
                {
                    fs.Write(barray);
                    fs.Flush(true);
                }
            }
            fs.Close();
        }

        public static void DownloadFile(string downloadPath, uint productId, UDFile file, DownloadConnection downloadConnection, Root saving)
        {
            var savingpath = Path.Combine(downloadPath, ".UD\\saved.bin");
            var fullPath = Path.Combine(downloadPath, file.Name);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            var fs = File.OpenWrite(fullPath);
            if (saving.Compression.HasSliceSHA)
            {
                if (file.SliceList.Count > 30)
                {
                    Console.WriteLine("\tBig Slice! " + file.SliceList.Count);
                    var splittedList = file.SliceList.ToList().SplitList<Uplay.Download.Slice>(10);
                    for (int listcounter = 0; listcounter < splittedList.Count; listcounter++)
                    {
                        var spList = splittedList[listcounter];
                        var dlbytes = ByteDownloader.DownloadBytes(savingpath, productId, file.Name, spList.ToList(), downloadConnection, saving);
                        foreach (var barray in dlbytes)
                        {
                            fs.Write(barray);
                            fs.Flush(true);
                        }
                        if (listcounter % 10 == 0)
                        {
                            Debug.PWDebug("%10 wait 1000ms");
                            Thread.Sleep(1000);
                        }
                        Thread.Sleep(10);
                    }
                }
                else
                {
                    var dlbytes = ByteDownloader.DownloadBytes(savingpath, productId, file.Name, file.SliceList.ToList(), downloadConnection, saving);
                    foreach (var barray in dlbytes)
                    {
                        fs.Write(barray);
                        fs.Flush(true);
                    }
                }
            }
            else
            {
                if (file.Slices.Count > 30)
                {
                    Console.WriteLine("\tBig Slice! " + file.Slices.Count);
                    var splittedList = file.Slices.ToList().SplitList<ByteString>(10);
                    for (int listcounter = 0; listcounter < splittedList.Count; listcounter++)
                    {
                        var spList = splittedList[listcounter];
                        var dlbytes = ByteDownloader.DownloadBytes(savingpath, productId, file, spList.ToList(), downloadConnection, saving);
                        foreach (var barray in dlbytes)
                        {
                            fs.Write(barray);
                            fs.Flush(true);
                        }

                        if (listcounter % 10 == 0)
                        {
                            Debug.PWDebug("%10 wait 1000ms");
                            Thread.Sleep(1000);
                        }
                        Thread.Sleep(10);
                    }

                }
                else
                {
                    var dlbytes = ByteDownloader.DownloadBytes(savingpath, productId, file, file.Slices.ToList(), downloadConnection, saving);
                    foreach (var barray in dlbytes)
                    {
                        fs.Write(barray);
                        fs.Flush(true);
                    }
                }

            }
            fs.Close();
            Console.WriteLine($"\t\tFile {file.Name} finished");
        }


    }
}
