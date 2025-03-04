using Downloader.Managers;
using Downloader.Tools;
using Serilog;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using UplayKit;
using static Downloader.Saving;
using File = System.IO.File;
using UDFile = Uplay.Download.File;

namespace Downloader;

internal class DLWorker
{
    public static ConcurrentBag<Saving.FileInfo> WorkInfos = new();
    public static ConcurrentBag<Saving.File> VerifyFiles = new();
    public static bool UseDLSHA1;
    public static void DownloadWorker()
    {
        Log.Information("Downloading Started!");
        Stopwatch stopwatch = Stopwatch.StartNew();
        int filecounter = 0;
        var saving = Read();
        saving.Work ??= new();

        if (saving.Work.FileInfos == null)
            saving.Work.FileInfos = new();
        UseDLSHA1 = saving.Compression.HasSliceSHA;
        WorkInfos = new(saving.Work.FileInfos);
        VerifyFiles = new(saving.Verify.Files);
        Log.Information("Prefile Work");
        Parallel.ForEach(ManifestManager.ToDownloadFiles, Config.ParallelOptions, (file) =>
        {
            if (WorkInfos.Any(x => x.Name == file.Name))
                return;

            if (file.Size == 0)
                return;

            if (CheckCurrentFile(file, ref saving))
                return;

            List<string> sliceListIds = new();
            List<string> sliceIds = new();

            foreach (var sl in file.SliceList)
                sliceListIds.Add(Convert.ToHexString(sl.DownloadSha1.ToArray()));

            foreach (var sl in file.Slices)
                sliceIds.Add(Convert.ToHexString(sl.ToArray()));

            WorkInfos.Add(new()
            {
                Name = file.Name,
                IDs = new()
                {
                    SliceList = sliceListIds,
                    Slices = sliceIds
                },
                CurrentId = string.Empty,
                NextId = string.Empty,
            });

            Log.Verbose("FileInfos added for {File}", file.Name);
        });
        saving.Work.FileInfos = WorkInfos.ToList();
        saving.Verify.Files = VerifyFiles.ToList();
        File.WriteAllText(Config.VerifyBinPath + ".json", JsonSerializer.Serialize(saving, new JsonSerializerOptions() { WriteIndented = true }));
        Save(saving);
        Log.Information("Pre-Download Done! Took only: {elapsed}", stopwatch.Elapsed);
        stopwatch.Restart();
        Parallel.ForEach(ManifestManager.ToDownloadFiles, Config.ParallelOptions, (file) => 
        {
            Interlocked.Add(ref filecounter, 1);
            Log.Information("{file} ({fileSize}) started ({filecounter}/{MaxCount}) | {ThreadId}", file.Name, Formatters.FormatFileSize(file.Size), filecounter, ManifestManager.ToDownloadFiles.Count, Environment.CurrentManagedThreadId);
            DownloadFile(file);
        });
        saving.Work.FileInfos = WorkInfos.ToList();
        saving.Verify.Files = VerifyFiles.ToList();
        Save(saving);
        Log.Information("Download for app {appid} is done! Took only: {elapsed}", Config.ProductId, stopwatch.Elapsed);
    }
    public static bool CheckCurrentFile(UDFile file, ref Root saving)
    {
        if (string.IsNullOrEmpty(Config.VerifyBinPath))
            return false;
        if (!WorkInfos.Any(x=>x.Name == file.Name))
            return false;
        if (!VerifyFiles.Any(x => x.Name == file.Name))
            return false;
        var fileinfo = WorkInfos.FirstOrDefault(x => x.Name == file.Name);
        if (fileinfo == null)
            return false;
        var curId = fileinfo.CurrentId;
        var NextId = fileinfo.NextId;


        List<string> slicesToDownload = new();
        int index = 0;
        uint Size = 0;
        if (saving.Compression.HasSliceSHA)
        {
            index = fileinfo.IDs.SliceList.FindIndex(0, x => x == curId);
            index += 1;
            slicesToDownload = fileinfo.IDs.SliceList.Skip(index).ToList();

        }
        else
        {
            index = fileinfo.IDs.Slices.FindIndex(0, x => x == curId);
            index += 1;
            slicesToDownload = fileinfo.IDs.Slices.Skip(index).ToList();
        }

        if (slicesToDownload.Count == 0)
            return false;

        var sizelister = file.SliceList.Take(index).ToList();
        foreach (var sizer in sizelister)
        {
            Size += sizer.Size;
        }

        var fullPath = Path.Combine(Config.DownloadDirectory, file.Name);
        var fileInfo = new System.IO.FileInfo(fullPath);
        if (fileInfo.Length != Size)
        {
            Log.Warning("Something isnt right, +/- chunk?? Check Error_CheckCurrentFile.txt file!");
            File.WriteAllText("Error_CheckCurrentFile.txt", (uint)fileInfo.Length + " != " + Size + " " + sizelister.Count + " " + index + "\n" + curId + " " + NextId);
            // We try restore chunk after here
            //
            return false;
        }
        Log.Information("Redownloading File {file} {ThreadId}!", file.Name, Environment.CurrentManagedThreadId);
        RedownloadSlices(slicesToDownload, file);
        return true;
    }

    public static void RedownloadSlices(List<string> slicesToDownload, UDFile file)
    {
        Log.Information("Starting redownloading file: {filename} {ThreadId}", file.Name, Environment.CurrentManagedThreadId);
        var fullPath = Path.Combine(Config.DownloadDirectory, file.Name);
        var prevBytes = File.ReadAllBytes(fullPath);
        var fs = File.OpenWrite(fullPath);
        fs.Position = prevBytes.LongLength;
        var splittedList = slicesToDownload.SplitList();
        for (int listcounter = 0; listcounter < splittedList.Count; listcounter++)
        {
            var spList = splittedList[listcounter];
            var dlbytes = ByteDownloader.DownloadBytes(file, spList);
            dlbytes.ForEach(bytes => fs.Write(bytes));
        }
        fs.Flush(true);
        fs.Close();
    }

    public static void DownloadFile(UDFile file)
    {
        //Logs.MixedLogger.Information("Starting downloading file: {filename} {ThreadId}", file.Name, Environment.CurrentManagedThreadId);
        var fullPath = Path.Combine(Config.DownloadDirectory, file.Name);
        var dir = Path.GetDirectoryName(fullPath);
        if (dir != null)
            Directory.CreateDirectory(dir);
        var fs = File.OpenWrite(fullPath);
        fs.Position = 0;
        if (UseDLSHA1)
        {
            var splittedList = file.SliceList.ToList().SplitList();
            for (int listcounter = 0; listcounter < splittedList.Count; listcounter++)
            {
                var spList = splittedList[listcounter];
                var dlbytes = ByteDownloader.DownloadBytes(file, spList);
                for (int i = 0; i < spList.Count; i++)
                {
                    var sp = spList[i];
                    var barray = dlbytes[i];
                    if (Config.DownloadAsChunks)
                    {
                        OnlyChunkWriter.Write(Convert.ToHexString(sp.DownloadSha1.ToArray()), barray);
                    }
                    else
                    {
                        fs.Write(barray);
                    }
                }
            }
        }
        else
        {
            var splittedList = file.Slices.ToList().SplitList();
            for (int listcounter = 0; listcounter < splittedList.Count; listcounter++)
            {
                var spList = splittedList[listcounter];
                var dlbytes = ByteDownloader.DownloadBytes(file, spList);
                for (int i = 0; i < spList.Count; i++)
                {
                    var sp = spList[i];
                    var barray = dlbytes[i];
                    if (Config.DownloadAsChunks)
                    {
                        OnlyChunkWriter.Write(Convert.ToHexString(sp.ToArray()), barray);
                    }
                    else
                    {
                        fs.Write(barray);
                    }
                };
            }
        }
        fs.Flush(true);
        fs.Close();
        //Logs.MixedLogger.Information("File {file} finished", file.Name);
        if (Config.DownloadAsChunks)
        {
            //we delete the file because we arent even writing to it :)
            File.Delete(fullPath);
        }
        
    }
}
