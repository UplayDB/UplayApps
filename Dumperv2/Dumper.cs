namespace Dumperv2
{
    internal class Dumper
    {
        public static void Dump(Uplay.Download.Manifest ManifestFile, string OutputFile)
        {
            var sw = File.CreateText(OutputFile);
            #region Counting
            int numFiles = 0;
            uint slicesize = 0, downloadedsize = 0;
            ulong fileSizes = 0;
            foreach (var chunks in ManifestFile.Chunks)
            {
                numFiles += chunks.Files.Count;
                foreach (var files in chunks.Files)
                {
                    fileSizes += files.Size;
                    foreach (var slices in files.SliceList)
                    {
                        slicesize += slices.Size;
                        downloadedsize += slices.DownloadSize;
                    }
                }
            }
            #endregion

            sw.WriteLine($"Total number of files  : {numFiles}");
            sw.WriteLine($"Total number of chunks : {ManifestFile.Chunks.Count}");
            sw.WriteLine($"Total size of Files    : {fileSizes,16} Bytes | {UplayKit.Formatters.FormatFileSize(fileSizes)}");
            sw.WriteLine($"Total size of Slices   : {slicesize,16} Bytes | {UplayKit.Formatters.FormatFileSize(slicesize)}");
            sw.WriteLine($"Total size Downloaded  : {downloadedsize,16} Bytes | {UplayKit.Formatters.FormatFileSize(downloadedsize)}");
            sw.WriteLine();
            sw.WriteLine("Compression Method      : " + ManifestFile.CompressionMethod);
            sw.WriteLine("Version                 : " + ManifestFile.Version);
            sw.WriteLine("GameVersion             : " + ManifestFile.GameVersion);
            sw.WriteLine("Is Require Patch        : " + ManifestFile.PatchRequired);
            sw.WriteLine("Is file compressed      : " + ManifestFile.IsCompressed);
            sw.WriteLine("Chunk Version           : " + ManifestFile.ChunksVersion);
            sw.WriteLine("Legacy Installer        : " + ManifestFile.LegacyInstaller);
            sw.WriteLine();
            if (ManifestFile.DeprecatedLanguages.Count != 0)
            {
                sw.Write("DeprecatedLanguages: ");
                foreach (var language in ManifestFile.DeprecatedLanguages)
                {
                    sw.Write(language + " , ");
                }
                sw.WriteLine();
            }
            if (ManifestFile.SlicerConfig != null)
            {
                sw.WriteLine();
                sw.WriteLine("SlicerConfig:");
                sw.WriteLine($" SliceTpye: {ManifestFile.SlicerConfig.SlicerType}" +
    $" | MinSSizeBytes {ManifestFile.SlicerConfig.MinSliceSizeBytes}" +
    $" | ExpectedSSizeBytes {ManifestFile.SlicerConfig.ExpectedSliceSizeBytes}" +
    $" | MaxSSizeBytes {ManifestFile.SlicerConfig.MaxSliceSizeBytes}" +
    $" | ConfigVersion {ManifestFile.SlicerConfig.ConfigVersion}");
            }
            if (ManifestFile.ReadmeFiles != null)
            {
                sw.WriteLine();
                sw.WriteLine("Readme Files:");
                sw.WriteLine($"RootPath: {ManifestFile.ReadmeFiles.RootPath}, Files Count:{ManifestFile.ReadmeFiles.Files.Count}");
                foreach (var file in ManifestFile.ReadmeFiles.Files)
                {
                    sw.WriteLine($"Name: {file.FileName} | Locale: {file.Locale}");
                }
            }
            if (ManifestFile.ManualFiles != null)
            {
                sw.WriteLine();
                sw.WriteLine("Manual Files:");
                sw.WriteLine($"RootPath: {ManifestFile.ManualFiles.RootPath}, Files Count:{ManifestFile.ManualFiles.Files.Count}");
                foreach (var file in ManifestFile.ManualFiles.Files)
                {
                    sw.WriteLine($"Name: {file.FileName} | Locale: {file.Locale}");
                }
            }

            sw.WriteLine();
            //Parsing Chunks
            foreach (var chunks in ManifestFile.Chunks)
            {
                sw.WriteLine("================================================Chunks================================================");
                sw.WriteLine("\nChunk ID: " + chunks.Id
                    + ", Chunk Type: " + chunks.Type + ", Chunk UplayID: " + chunks.UplayId
                    + ", Chunk Disc: " + chunks.Disc + ", Chunk Language: " + chunks.Language
                    + ", Chunk Tags: " + chunks.Tags);

                if (chunks.Files[0].SliceList.Count == 0)
                {
                    sw.WriteLine("\t\tSHA1\t\t\t | Slice Size | FileName");
                    foreach (var files in chunks.Files)
                    {
                        var i = 0;
                        foreach (var slices in files.Slices)
                        {
                            string hexSha1 = Convert.ToHexString(slices.ToArray());
                            sw.WriteLine($"{hexSha1,40} | {files.Size,10} | {files.Name}");
                            i++;
                        }
                    }
                }
                else
                {
                    sw.WriteLine("\t\tSHA1\t\t\t |     \t\t Slice SHA1\t\t    |  FileSize  |Slice Size| DownSize | FileName");
                    foreach (var files in chunks.Files)
                    {
                        var i = 0;
                        foreach (var slices in files.SliceList)
                        {
                            string hexSha1 = Convert.ToHexString(slices.DownloadSha1.ToArray());
                            var slice = Convert.ToHexString(files.Slices[i].ToArray());
                            sw.WriteLine($"{hexSha1,40} | {slice,40} | {files.Size,10} | {slices.Size,8} | {slices.DownloadSize,8} | {files.Name}");
                            i++;
                        }
                    }
                }
            }

            //Parsing Licenses
            sw.WriteLine();
            foreach (var license in ManifestFile.Licenses)
            {
                sw.WriteLine("===============================================Licenses===============================================");
                sw.WriteLine("\nIdentifier: " + license.Identifier + ", Version: " + license.Version
                    + ", Format: " + license.Format);

                sw.WriteLine("		SHA1			 |  Lang  |  Text  | Name");
                foreach (var licenseLocale in license.Locales)
                {
                    string hexSha1 = Convert.ToHexString(licenseLocale.Sha1.ToArray());
                    sw.WriteLine($"{hexSha1,40} | {licenseLocale.Language,6} | {licenseLocale.Text,6} | {licenseLocale.Name}");
                }
            }

            //Parsing InstallRuns
            sw.WriteLine();
            foreach (var installRun in ManifestFile.InstallRuns)
            {
                sw.WriteLine("=============================================Install Runs=============================================");
                sw.WriteLine("\nExe: " + installRun.Exe + ", Working Dir: " + installRun.WorkingDir
                    + "\nArguments: " + installRun.Arguments + ", Description: " + installRun.Description);
                sw.WriteLine("Identifier: " + installRun.Identifier + ", Version: " + installRun.Version
                    + "\nPlatform: " + installRun.Platform + ", Platform Type: " + installRun.PlatformType);
                sw.WriteLine("Ignore All Exit Codes?: " + installRun.IgnoreAllExitCodes + ", Restart Required?: " + installRun.RestartRequired);
            }

            //Parsing InstallRegistry
            sw.WriteLine();
            foreach (var installRegistry in ManifestFile.InstallRegistry)
            {
                sw.WriteLine("===========================================Install Registry===========================================");
                sw.WriteLine("\nKey: " + installRegistry.Key + ", Type: " + installRegistry.Type);


                if (installRegistry.RegistryNumberEntry.Count > 0)
                {
                    sw.WriteLine("Registry Number Entry");
                    foreach (var registryNumberEntry in installRegistry.RegistryNumberEntry)
                    {
                        sw.WriteLine($"{registryNumberEntry.Value}, {registryNumberEntry.Language}");
                    }
                }
                if (installRegistry.RegistryStringEntry.Count > 0)
                {
                    sw.WriteLine("Registry String Entry");
                    foreach (var registryStringEntry in installRegistry.RegistryStringEntry)
                    {
                        sw.WriteLine($"{registryStringEntry.Value}, {registryStringEntry.Language}");
                    }
                }
            }

            //Parsing InstallGameExplorer
            if (ManifestFile.InstallGameExplorer.Count > 0)
            {
                sw.WriteLine();
                foreach (var installGameExplorer in ManifestFile.InstallGameExplorer)
                {
                    sw.WriteLine("=========================================Install Game Explorer========================================");
                    sw.WriteLine("\nGdfPath: " + installGameExplorer.GdfPath + ", Version: " + installGameExplorer.Version);
                }
            }

            //Parsing InstallFirewallRules
            if (ManifestFile.InstallFirewallRules.Count > 0)
            {
                sw.WriteLine();
                foreach (var installFirewallRule in ManifestFile.InstallFirewallRules)
                {
                    sw.WriteLine("========================================Install Firewall Rules========================================");
                    sw.WriteLine("\nName: " + installFirewallRule.Name + ", Exe: " + installFirewallRule.Exe
                        + ", Profile: " + installFirewallRule.Profile + ", Protocol: " + installFirewallRule.Protocol);
                    sw.WriteLine("Ports: " + installFirewallRule.Ports + ", Version: " + installFirewallRule.Version);
                }
            }

            //Parsing InstallCompatibility
            if (ManifestFile.InstallFirewallRules.Count > 0)
            {
                sw.WriteLine();
                foreach (var installCompatibility in ManifestFile.InstallCompatibility)
                {
                    sw.WriteLine("=========================================Install Game Explorer========================================");
                    sw.WriteLine("\nExe: " + installCompatibility.Exe + ", Options: " + installCompatibility.Options + ", Platform: " + installCompatibility.Platform);
                }
            }

            //Parsing UninstallRuns
            sw.WriteLine();
            foreach (var uninstallRun in ManifestFile.UninstallRuns)
            {
                sw.WriteLine("============================================Uninstall Runs============================================");
                sw.WriteLine("\nExe: " + uninstallRun.Exe + ", Working Dir: " + uninstallRun.WorkingDir
                    + "\nArguments: " + uninstallRun.Arguments);
                sw.WriteLine("Platform: " + uninstallRun.Platform + ", Platform Type: " + uninstallRun.PlatformType);
            }

            //Parsing UninstallRegistry
            sw.WriteLine();
            foreach (var uninstallRegistry in ManifestFile.UninstallRegistry)
            {
                sw.WriteLine("==========================================Uninstall Registry==========================================");
                sw.WriteLine("\nKey: " + uninstallRegistry.Key);
            }

            //Parsing Languages
            sw.WriteLine();
            foreach (var language in ManifestFile.Languages)
            {
                sw.WriteLine("==============================================Languages===============================================");
                sw.WriteLine("\nCode: " + language.Code + ", UplayIds Count: " + language.UplayIds.Count);
                foreach (var ids in language.UplayIds)
                {
                    sw.WriteLine($"UplayID: {ids}");
                }
            }

            sw.Flush();
            sw.Close();

        }

        public static void DumpAsCSV(Uplay.Download.Manifest ManifestFile, Uplay.DownloadInstallState.DownloadInstallState? StateFile, string OutputFile, string shaid = "UNKOWN", uint UplayId = 0)
        {
            if (!OutputFile.Contains("csv")) { OutputFile += ".csv"; }

            var sw = File.CreateText(OutputFile);
            #region Counting
            int numFiles = 0;
            uint uncompressedSize = 0, compressedSize = 0;
            ulong fileSizes = 0;
            foreach (var chunks in ManifestFile.Chunks)
            {
                numFiles += chunks.Files.Count;
                foreach (var files in chunks.Files)
                {
                    fileSizes += files.Size;

                    foreach (var slices in files.SliceList)
                    {
                        uncompressedSize += slices.Size;
                        compressedSize += slices.DownloadSize;
                    }
                }
            }
            #endregion

            string SHA = shaid;
            uint uid = UplayId;
            if (StateFile != null)
            {
                if (SHA == "UNKOWN" && uid == 0)
                {
                    SHA = StateFile.ManifestSha1;
                    uid = StateFile.UplayId;
                }
            }

            sw.WriteLine($"\"{uid}\",\"{SHA}\",\"0000-00-00\",{numFiles},{fileSizes},{uncompressedSize},{compressedSize},{(int)ManifestFile.CompressionMethod}");

            uint downsize = 0, slicesize = 0;
            foreach (var chunks in ManifestFile.Chunks)
            {
                foreach (var files in chunks.Files)
                {
                    downsize = 0;
                    slicesize = 0;
                    foreach (var slices in files.SliceList)
                    {
                        downsize += slices.DownloadSize;
                        slicesize += slices.Size;

                    }

                    sw.WriteLine($"\"{SHA}\",\"{files.Name}\",{files.Size},{slicesize},{downsize},{chunks.Id}");
                }
            }
            sw.Flush();
            sw.Close();

        }
    }
}
