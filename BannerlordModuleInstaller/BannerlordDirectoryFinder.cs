using System;
using System.IO;
using Microsoft.Win32;

namespace BannerlordModuleInstaller
{
    public static class BannerlordDirectoryFinder
    {
        public static string Find()
        {
            string steamKey = "Software\\Valve\\Steam";
            string steamappKey = "Software\\Valve\\Steam\\Apps\\261550";
            string steamappFolderName = "Mount & Blade II Bannerlord";

            //Making sure Steam and the SteamApp in question are both installed
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(steamKey);
            if (regKey == null)
            {
                return "";
            }

            regKey = Registry.CurrentUser.OpenSubKey(steamappKey);
            if (regKey == null)
            {
                return "";
            }

            //Getting Steam Path
            regKey = Registry.CurrentUser.OpenSubKey(steamKey);
            string steamPath = (string)regKey.GetValue("SteamPath");

            //Checking the steamapps folder within steam path
            string defaultSteamappPath = Path.Combine(steamPath, "steamapps", "common");
            if (Directory.Exists(Path.Combine(defaultSteamappPath, steamappFolderName)))
            {
                return Path.Combine(defaultSteamappPath, steamappFolderName);
            }
            //Getting paths to additional steam libraries if the queried app isn't in the default steamapps folder
            else
            {
                string steamConfigPath = Path.Combine(steamPath, "config", "config.vdf");
                if (File.Exists(steamConfigPath))
                {
                    string[] lines = File.ReadAllLines(steamConfigPath);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Contains("BaseInstallFolder"))
                        {
                            string[] splits = lines[i].Split('\"');
                            string libraryBasePath = splits[3].Replace("\\\\", "\\"); 
                            string libraryPath = Path.Combine(libraryBasePath, "steamapps", "common");
                            if (Directory.Exists(Path.Combine(libraryPath, steamappFolderName)))
                            {
                                return Path.Combine(libraryPath, steamappFolderName);
                            }
                        }
                    }
                }
            }

            return "";
        }
    }
}
