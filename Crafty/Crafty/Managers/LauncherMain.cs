using SharpCompress;
using CmlLib;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Downloader;
using Newtonsoft.Json;
using RestSharp;
using SharpCompress.Common;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using Crafty.Models;
using System.Net;
using System.Diagnostics;

namespace Crafty.Managers
{
    public class LauncherMain
    {
        static MinecraftPath localMinecraftPath { get; set; } = new MinecraftPath(System.IO.Directory.GetCurrentDirectory() + "/Client");
        public static CMLauncher Instance { get; set; } = new CMLauncher(localMinecraftPath);
        public CMLauncher localLauncher { get; set; }
        MSession localSession { get; set; }

        public string LocalFilesPath = Directory.GetCurrentDirectory() + "\\Downloads";
        private string _versionName;


        public LauncherMain(string versionName)
        {
            localLauncher = new CMLauncher(localMinecraftPath +"\\" + versionName);
            _versionName = versionName;
        }
        public LauncherMain()
        {
            localLauncher = new CMLauncher(localMinecraftPath);        
        }

        public ClientVersions GetVersions()
        {
            var client = new RestClient("http://95.182.110.19");
            var request = new RestRequest("/api/GetDirectories");

            request.Method = Method.Get;

            var response = client.Execute(request);

            var content = response.Content;

            try
            {
                ClientVersions versions = JsonConvert.DeserializeObject<ClientVersions>(content);
                return versions;
            }
            catch (JsonReaderException e)
            {
                Console.WriteLine($"Ошибка при чтении JSON: {e.Message}");
                return null;
            }

        }


        public float GetRemoteClientVersion()
        {
            var client = new RestClient("http://95.182.110.19");
            var request = new RestRequest("/api/GetLauncherVersion");

            request.Method = Method.Get;
            var response = client.Execute(request);
            return float.Parse(response.Content.Replace("\"", string.Empty));
        }

        public bool CheckForUpdate()
        {
            float currentVersion;
            float remoteVersion;
            if(!Directory.Exists(Directory.GetCurrentDirectory() + "\\Configs"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Configs");
            }
            using (FileStream fs = new(Directory.GetCurrentDirectory() + "\\Configs" + "\\version.ini", FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (StreamReader file = new StreamReader(fs))
                {
                    float.TryParse(file.ReadLine(), out currentVersion);
                }
                remoteVersion =  GetRemoteClientVersion();

                if(remoteVersion > currentVersion)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task DownloadUpdater()
        {
            string downloadUrl = "http://95.182.110.19/api/GetUpdater";
            string filename = "Updater.exe";
            string dest = Directory.GetCurrentDirectory() + "\\" + filename;

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(new Uri(downloadUrl), dest);
            }
        }

        public async Task LaunchGame(string Name, bool needToDownload, string Version, int ram)
        {
            var session = MSession.GetOfflineSession(Name);
            MLaunchOption launchOption;
            string JavaPathLine;
            JavaPathLine = ConfigManager.Config.UserJavaPath;

            if (!string.IsNullOrEmpty(JavaPathLine))
            {
                launchOption = new MLaunchOption
                {
                    MaximumRamMb = ram,
                    Session = session,
                    JavaPath = JavaPathLine,
                };
            }
            else
            {

                launchOption = new MLaunchOption
                {
                    MaximumRamMb = ram,
                    Session = session,
                };
            }

                    
            var adress = "95.182.110.19";
            
            var versionName = Version;
            if(needToDownload )
            {
                var files = new DownloadFile[1];
                var FilesList = new DownloadFile(System.IO.Directory.GetCurrentDirectory() + "/Downloads/FilesList.json", "http://" + adress + "/api/GetFileList?versionName=" + versionName);
                files[0] = FilesList;

                await Instance.DownloadGameFiles(files);

                string jsonString = File.ReadAllText(LocalFilesPath + "\\FilesList.json");
                FileList filesList = JsonConvert.DeserializeObject<FileList>(jsonString);

                var ClientFiles = new List<DownloadFile>();

                foreach (var file in filesList.files)
                {
                    var urlString = "http://" + adress + "/api/GetFile?versionName=" + versionName + "&fileName=" + file;
                    var tempFile = new DownloadFile(System.IO.Directory.GetCurrentDirectory() + "/Downloads/" + file, urlString);
                    tempFile.Type = MFile.Others;
                    tempFile.Name = file;
                    ClientFiles.Add(tempFile);
                }

                await Instance.DownloadGameFiles(ClientFiles.ToArray());

                await UnpackClientRar(filesList);

            }

            var process = await Instance.CreateProcessAsync(Version, launchOption, false);
            process.StartInfo.CreateNoWindow = true;

            process.Start();
           
        }

        public async Task UnpackClientRar(FileList list)
        {
            foreach (var file in list.files)
            {
                using (var archive = RarArchive.Open(LocalFilesPath + "\\" + file))
                {
                    foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                    {
                        entry.WriteToDirectory(System.IO.Directory.GetCurrentDirectory() + "/Client"+ "\\" + _versionName, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }
                }
            }
        }

    }
}

