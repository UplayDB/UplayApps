using System.Security.Cryptography;
using UDFile = Uplay.Download.File;

namespace Downloader
{
    internal class Verifier
    {
        public static List<UDFile> Verify(List<UDFile> files, Saving.Root saving, string downloadPath)
        {
            List<UDFile> fileschecked = new();
            List<UDFile> remover = new();
            Console.WriteLine("\t\tVerification Started!");
            foreach (var file in files)
            {
                if (fileschecked.Contains(file))
                {
                    continue;
                }

                var fullPath = Path.Combine(downloadPath, file.Name);
                if (File.Exists(fullPath))
                {
                    var Verified = VerifyFile(file, fullPath, saving, out var failes);
                    string addinfo = "";
                    if (Verified)
                    {
                        addinfo = "Check successful!";
                        remover.Add(file);
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
                    }
                    fileschecked.Add(file);
                    Console.WriteLine($"\t\tFile {file.Name} verified! {addinfo}");
                }

            }
            List<UDFile> returner = new();
            returner.AddRange(files);
            foreach (var rf in remover)
            {
                returner.Remove(rf);
            }
            Console.WriteLine("\t\tVerification Done!");
            return returner;
        }

        public static bool VerifyFile(UDFile file, string PathToFile, Saving.Root saving, out List<int> failinplace)
        {
            failinplace = new();
            var fileInfo = new FileInfo(PathToFile);
            if ((ulong)fileInfo.Length != file.Size)
            {
                failinplace.Add(-1);
            }

            var filebytes = File.ReadAllBytes(PathToFile);

            if (saving.Verify.Files.Count == 0)
                goto END;

            var sfile = saving.Verify.Files.Where(x => x.Name == file.Name).FirstOrDefault();
            if (sfile == null)
                goto END;
            var takenSize = 0;
            for (int sinfocount = 0; sinfocount < sfile.SliceInfo.Count; sinfocount++)
            {
                var sinfo = sfile.SliceInfo[sinfocount];
                var fslist = file.SliceList[sinfocount];
                var size = fslist.Size;
                var fibytes = filebytes.Skip(takenSize).Take((int)size).ToArray();
                takenSize += (int)size;
                var decsha = GetSHA1Hash(fibytes);
                if (sinfo.DecompressedSHA != decsha)
                {
                    failinplace.Add(takenSize);
                }
                if (sinfo.DownloadedSize != fibytes.Length)
                {
                    failinplace.Add(-fibytes.Length);
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
}
