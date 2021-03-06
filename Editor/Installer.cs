﻿using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using UnityEditor;
using UnityEngine;

namespace TNRD.PackageManager.Injection
{
    internal static class Installer
    {
        [InitializeOnLoadMethod]
        [MenuItem("TNRD/Package Manager/Extract Injector")]
        private static void Init()
        {
            string zipPath = GetZipPath(out string unityVersion);
            if (string.IsNullOrEmpty(zipPath))
            {
                Debug.LogError("This version of Unity is not supported by the Package Manager Injection Helper");
                return;
            }

            string outputPath = Path.Combine(Application.dataPath, "TNRD", "Package Manager", "Injection", unityVersion);

            if (Directory.Exists(outputPath) && Directory.GetFiles(outputPath, "PackageManagerInjectionHelper.cs", SearchOption.AllDirectories).Length == 1)
            {
                return;
            }

            string injectionDirectoryPath = Path.Combine(Application.dataPath, "TNRD", "Package Manager", "Injection");
            if (Directory.Exists(injectionDirectoryPath))
            {
                string[] directories = Directory.GetDirectories(injectionDirectoryPath);
                foreach (string directory in directories)
                {
                    Directory.Delete(directory, true);
                }
            }

            string fullPath = Path.GetFullPath(zipPath);
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(fullPath, outputPath, null);
            AssetDatabase.Refresh();
        }

        private static string GetZipPath(out string unityVersion)
        {
            unityVersion = Application.unityVersion;
            string path = $"Packages/net.tnrd.packagemanagerinjection/Editor/Zips/{unityVersion}.zip";
            while (!File.Exists(path))
            {
                if (!unityVersion.Contains("."))
                {
                    return string.Empty;
                }

                unityVersion = unityVersion.Substring(0, unityVersion.LastIndexOf('.'));
                path = $"Packages/net.tnrd.packagemanagerinjection/Editor/Zips/{unityVersion}.zip";
            }

            return path;
        }

#if TNRD_DEV
        [MenuItem("TNRD/Update Zip")]
        private static void UpdateZip()
        {
            string[] directories = Directory.GetDirectories("Assets/TNRD/Package Manager/Injection", "*", SearchOption.TopDirectoryOnly);
            foreach (string directory in directories)
            {
                string path = directory.Replace("\\", "/");
                string zipName = path.Substring(path.LastIndexOf('/') + 1);
                string zipPath = path;
                FastZip fastZip = new FastZip();
                fastZip.CreateZip($"Packages/net.tnrd.packagemanagerinjectionhelper/Editor/Zips/{zipName}.zip", zipPath, true, null);
            }
        }
#endif
    }
}