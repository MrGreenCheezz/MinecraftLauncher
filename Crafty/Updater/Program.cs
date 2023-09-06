using System;
using System.Diagnostics;
using System.Net;

internal class Program
{
    static void Main(string[] args)
    {
        MainAsync().GetAwaiter().GetResult();
    }

    static async Task MainAsync()
    {      
        var mainClass = new Program();
        await mainClass.DownloadFile();
        var path = Directory.GetCurrentDirectory() + "\\LauncherRelease.exe";
        string argsStart = "-s -d" + Directory.GetCurrentDirectory();
        string processName = "GcLauncher"; 
        Process[] processes = Process.GetProcessesByName(processName);
        foreach (Process process in processes)
        {
            process.Kill();
        }
        Process.Start(path, argsStart);
        Environment.Exit(0);
    }

    public async Task DownloadFile()
    {
        string downloadUrl = "http://95.182.110.19/api/GetLauncher";
        string filename = "LauncherRelease.exe";
        string dest = Directory.GetCurrentDirectory() + "\\" + filename;

        using (WebClient webClient = new WebClient())
        {
           webClient.DownloadFile(new Uri(downloadUrl), dest);
        }
    }

}

