using System.Diagnostics;
using UplayKit;
using Microsoft.Win32;

namespace MManagement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to MManagement!");
            Console.WriteLine("\nChoose: 0 Install | 1 Uninstall");
            
            int choosen = int.Parse(Console.ReadLine());

            var manifestData = Parsers.ParseManifestFile("uplay_install.manifest");
            var BasePath = Directory.GetCurrentDirectory();
            if (choosen == 0)
            {
                var installrus = manifestData.InstallRuns.ToList();
                foreach (var installr in installrus)
                {
                    var exe = installr.Exe;
                    var param = installr.Arguments;

                    if (exe.Contains("$INSTALL_DIR"))
                    {
                        exe = exe.Replace("$INSTALL_DIR", BasePath);
                    }

                    if (param.Contains("$INSTALL_DIR"))
                    {
                        param = param.Replace("$INSTALL_DIR", BasePath);
                    }
                    if (param.Length == 0)
                    {
                        Console.WriteLine($"Process will start {exe} without param");
                        Process.Start(exe);
                    }
                    else
                    {
                        Console.WriteLine($"Process will start {exe} {param}");
                        Process.Start(exe, param);
                    }
                }

                var installRegistries = manifestData.InstallRegistry.ToList();
                foreach (var installreq in installRegistries)
                {
                    if (installreq.RegistryStringEntry.Count > 0)
                    {
                        var regstring = installreq.RegistryStringEntry.FirstOrDefault();
                        var splittedkey = installreq.Key.Split("\\");
                        var valuename = splittedkey[splittedkey.Length - 1];
                        var regkey = String.Join("\\", splittedkey.Take(splittedkey.Length - 1).ToArray());
                        Console.WriteLine($"Registry will install here: {regkey}\n{valuename}\n{regstring.Value}");
                        Registry.SetValue(regkey, valuename, regstring.Value);
                    }
                    if (installreq.RegistryNumberEntry.Count > 0)
                    {
                        var regstring = installreq.RegistryNumberEntry.FirstOrDefault();
                        var splittedkey = installreq.Key.Split("\\");
                        var valuename = splittedkey[splittedkey.Length - 1];
                        var regkey = String.Join("\\", splittedkey.Take(splittedkey.Length - 1).ToArray());
                        Console.WriteLine($"Registry will install here: {regkey}\n{valuename}\n{regstring.Value}");
                        Registry.SetValue(regkey, valuename, regstring.Value);
                    }
                }

                Console.WriteLine("Firewall rule is not supported!");
            }
            else if (choosen == 1)
            {
                var installrus = manifestData.UninstallRuns.ToList();
                foreach (var installr in installrus)
                {
                    var exe = installr.Exe;
                    var param = installr.Arguments;

                    if (exe.Contains("$INSTALL_DIR"))
                    {
                        exe = exe.Replace("$INSTALL_DIR", BasePath);
                    }

                    if (param.Contains("$INSTALL_DIR"))
                    {
                        param = param.Replace("$INSTALL_DIR", BasePath);
                    }
                    if (param.Length == 0)
                    {
                        Console.WriteLine($"Process will start {exe} without param");
                        Process.Start(exe);
                    }
                    else
                    {
                        Console.WriteLine($"Process will start {exe} {param}");
                        Process.Start(exe, param);
                    }

                }

                var uninstallRegistries = manifestData.UninstallRegistry.ToList();
                foreach (var uninstallreq in uninstallRegistries)
                {
                    if (uninstallreq.Key.Contains("HKEY_CURRENT_USER"))
                    {
                        var uninstallkey = uninstallreq.Key.Replace("HKEY_CURRENT_USER\\", "");
                        Console.WriteLine($"Registry will uninstall here: {uninstallkey}");
                        Registry.CurrentUser.DeleteSubKeyTree(uninstallkey);
                    }
                    else if (uninstallreq.Key.Contains("HKEY_LOCAL_MACHINE"))
                    {
                        var uninstallkey = uninstallreq.Key.Replace("HKEY_LOCAL_MACHINE\\", "");
                        Console.WriteLine($"Registry will uninstall here: {uninstallkey}");
                        Registry.LocalMachine.DeleteSubKeyTree(uninstallkey);
                    }
                    else
                    {
                        Console.WriteLine("Please report this, UninstallKey: " + uninstallreq.Key);
                    }
                    
                }
            }

        }
    }
}