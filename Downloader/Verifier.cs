using Downloader.Managers;
using Serilog;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text.Json;
using UplayKit;
using UDFile = Uplay.Download.File;

namespace Downloader;

internal class Verifier
{
    public static void Verify()
    {
        ConcurrentBag<UDFile> CheckedFiles = new();
        ConcurrentBag<UDFile> ToRemoveFiles = new();
        ConcurrentBag<string> ToPrint = new();
        var saving = Saving.Read();
        Log.Information("Verification Started!");
        Parallel.ForEach(ManifestManager.ToDownloadFiles, Config.ParallelOptions, (udfile) => 
        {
            if (CheckedFiles.Contains(udfile))
                return;
            var fullPath = Path.Combine(Config.DownloadDirectory, udfile.Name);
            if (!File.Exists(fullPath))
                return;
            string addinfo = "";
            if (VerifyFile(udfile, fullPath, out var failes, saving))
            {
                addinfo = "Check successful!";
                ToRemoveFiles.Add(udfile);
            }
            else
            {
                addinfo = "Check failed!";
                if (failes.Contains(-1))
                {
                    addinfo += " (FileSize)";
                }
                else
                {
                    addinfo += " (SHA Missmatch)";
                }
                ToPrint.Add($"{udfile.Name} - {JsonSerializer.Serialize(failes)}");
            }
            CheckedFiles.Add(udfile);
            Log.Information("File {file} verified! {addinfo}", udfile.Name, addinfo);
        });
        File.AppendAllLines("FailedFiles.txt", ToPrint.ToArray());
        foreach (var rf in ToRemoveFiles)
        {
            ManifestManager.ToDownloadFiles.Remove(rf);
        }
        Log.Information("Verification Done!");
    }

    public static bool VerifyFile(UDFile file, string PathToFile, out List<int> failinplace, Saving.Root saving)
    {
        failinplace = new();
        var fileInfo = new FileInfo(PathToFile);
        if ((ulong)fileInfo.Length != file.Size)
        {
            failinplace.Add(-1);
            goto END;
        }
        if (saving == null)
            goto END;
        if (saving.Verify.Files.Count == 0)
            goto END;
        var sfile = saving.Verify.Files.Where(x => x.Name == file.Name).FirstOrDefault();
        if (sfile == null)
            goto END;

        var takenSize = 0;
        var fileread = File.OpenRead(PathToFile);
        Log.Debug("File Length: {len}", fileInfo.Length);
        for (int sinfocount = 0; sinfocount < sfile.SliceInfo.Count; sinfocount++)
        {
            var sinfo = sfile.SliceInfo[sinfocount];
            if (sinfo == null)
            {
                Log.Warning("Slice info inside the Verify.bin not found!");
                goto END;
            }

            var fslist = file.SliceList[sinfocount];
            byte[] fibytes = new byte[sinfo.DecompressedSize];
            int readAmount = fileread.Read(fibytes, 0, sinfo.DecompressedSize);

            if (readAmount != sinfo.DecompressedSize)
            {
                Log.Warning("Reading failed Need to read: {needToRead} but read: {readed}!", sinfo.DecompressedSize, readAmount);
                failinplace.Add(-2);
                goto END;
            }

            takenSize += sinfo.DecompressedSize;
            var decsha = GetSHA1Hash(fibytes);

            if (sinfo.DecompressedSHA != decsha)
            {
                Log.Warning("{decompressedSHA} != {FileDecSHA} (Decompressed hash is not the same!)", sinfo.DecompressedSHA, decsha);
                failinplace.Add(takenSize);
            }
            if (sinfo.DecompressedSize != fibytes.Length)
            {
                Log.Warning("{decompressedSize} != {FileLength} (Decompressed size is not the same!)", sinfo.DecompressedSize, fibytes.Length);
                failinplace.Add(fibytes.Length * (-1));
            }
        }

    END:
        if (failinplace.Count != 0)
        {
            return false;
        }
        return true;
    }

    public static string GetSHA1Hash(byte[] input)
    {
        using var sha1 = SHA1.Create();
        return Convert.ToHexString(sha1.ComputeHash(input));
    }

}
