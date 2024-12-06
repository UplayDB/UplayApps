using System.Text.Json;

namespace Downloader.Managers;

internal class VerifyManager
{
    public static void Init()
    {
        if (string.IsNullOrEmpty(Config.ManifestId))
        {
            Console.WriteLine("ManifestId is null!");
            return;
        }
        Saving.Root saving = new();
        Config.VerifyBinPath = Path.Combine(Config.DownloadDirectory, ".UD","verify.bin");
        var verifybinPathDir = Path.GetDirectoryName(Config.VerifyBinPath);
        if (verifybinPathDir != null)
            Directory.CreateDirectory(verifybinPathDir);
        if (File.Exists(Config.VerifyBinPath)) 
        {
            var readedBin = Saving.Read();
            if (readedBin == null)
            {
                saving = Saving.MakeNew(Config.ProductId, Config.ManifestId, ManifestManager.ParsedManifest);
            }
            else
            {
                saving = readedBin;
            }
            
        }
        else if (File.Exists($"{Config.VerifyBinPath}.json"))
        {
            saving = JsonSerializer.Deserialize<Saving.Root>(File.ReadAllText($"{Config.VerifyBinPath}.json"))!;
        }
        else
        {
            saving = Saving.MakeNew(Config.ProductId, Config.ManifestId, ManifestManager.ParsedManifest);
        }
        if (Config.ParseVerifyBin)
        {
            File.WriteAllText($"{Config.VerifyBinPath}.json", JsonSerializer.Serialize(saving, new JsonSerializerOptions() { WriteIndented = true }));
        }
        Saving.Save(saving);
        if (Config.Verify && !Config.DownloadAsChunks)
        {
           Verifier.Verify();
        }
        //System.IO.File.WriteAllText("files_to_download_after_verify.json", JsonSerializer.Serialize(ManifestManager.ToDownloadFiles, new JsonSerializerOptions() { WriteIndented = true }));
    }
}
