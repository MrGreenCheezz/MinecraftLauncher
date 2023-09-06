using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using Crafty.Core;
using Crafty.Managers;
using Crafty.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Version = Crafty.Models.Version;

namespace Crafty.Views
{
	public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
	{
		public static string configPath = Directory.GetCurrentDirectory() + "\\Configs";
        public string BarText { get; set; }
		public MainWindow()
		{
			InitializeComponent();

			var lanch = new LauncherMain();
			UpdateLauncherButton.IsEnabled = lanch.CheckForUpdate();

			if (!Directory.Exists(configPath))
			{
				Directory.CreateDirectory(configPath);
			}
			else
			{
				LoadLauncherConfig();
			}

			try
			{
				VersionList.SelectedItem = Launcher.VersionList.Where(x => x.Id == ConfigManager.Config.LastVersionUsed).First();
			}
			catch
			{
				VersionList.SelectedItem = null;
			}
		}

        private void LoadLauncherConfig()
        {
			using(FileStream fs = new(configPath + "\\config.ini", FileMode.OpenOrCreate, FileAccess.Read))
			{
				using (StreamReader file = new StreamReader(fs))
				{
					var line = file.ReadLine();
					if (line != null)
					{
						bool isChecked;
						bool.TryParse(file.ReadLine(),out isChecked);

                        SaveUsernameCheck.IsChecked = isChecked;
						if (SaveUsernameCheck.IsChecked == true)
						{
							var nick = file.ReadLine();
							Username.Text = nick;
							ConfigManager.Config.Username = nick;
						}
					}
				}
            }
           
        }

        private async void PlayClicked(object? sender, RoutedEventArgs e)
		{
			if (VersionList.SelectedItem == null) return;

			if (!Launcher.IsLoggedIn) Username.IsEnabled = false;
			VersionList.IsEnabled = false;
			PlayButton.IsEnabled = false;

			Version selectedVersion = (Version)VersionList.SelectedItem;
            if (SaveUsernameCheck.IsChecked == true)
            {
				ConfigManager.Config.LastVersionUsed = ((Version)VersionList.SelectedItem).Id;
				ConfigManager.SaveConfig();
                SaveConfigFile();
            }

            



            try
            {
                var mineLauncher = new LauncherMain(selectedVersion.Name);

                await mineLauncher.LaunchGame(Username.Text, false, selectedVersion.Name, ConfigManager.Config.Ram);

                ProgressBar.ProgressTextFormat = "Done!";
			}
			catch(Exception exception)
			{
				using(FileStream fs = new(Directory.GetCurrentDirectory() + "\\PlayError.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
				{
					using(StreamWriter sw = new StreamWriter(fs))
					{
						sw.WriteLine(exception.ToString());
					}
				}

                ProgressBar.Background = Brush.Parse("#8a1a1a");
				ProgressBar.ProgressTextFormat = $"Couldn't launch {selectedVersion.Id}!";
			}

			await Task.Delay(3000);

			if (!Launcher.IsLoggedIn) Username.IsEnabled = true;
			VersionList.IsEnabled = true;
			PlayButton.IsEnabled = true;
			ProgressBar.Background = Brush.Parse("#141414");
			ProgressBar.ProgressTextFormat = "";
			ProgressBar.Value = 0;
		}

		private void SaveConfigFile()
		{
			using (FileStream fs = new(configPath + "\\config.ini", FileMode.OpenOrCreate, FileAccess.Write))
			{
                using (StreamWriter file = new StreamWriter(fs))
                {
                    file.WriteLine("LauncherConfig:");
                    file.WriteLine(SaveUsernameCheck.IsChecked);
                    file.WriteLine(Username.Text);
                }
            }
            
		}

		private async void UpdateLauncher(object? sender, RoutedEventArgs e)
		{
			var launcernew = new LauncherMain();
			await launcernew.DownloadUpdater();
			Process.Start(Directory.GetCurrentDirectory() + "\\Updater.exe");
		}

        private async void RepairClicked(object? sender, RoutedEventArgs e)
        {
            if (VersionList.SelectedItem == null) return;

            if (!Launcher.IsLoggedIn) Username.IsEnabled = false;
            VersionList.IsEnabled = false;
            PlayButton.IsEnabled = false;

            Version selectedVersion = (Version)VersionList.SelectedItem;
			if(SaveUsernameCheck.IsChecked == true)
			{
                ConfigManager.Config.LastVersionUsed = ((Version)VersionList.SelectedItem).Id;
                ConfigManager.SaveConfig();
                SaveConfigFile();
            }
            


            try
            {
                var mineLauncher = new LauncherMain(selectedVersion.Name);
                await mineLauncher.LaunchGame(Username.Text, true, selectedVersion.Name, ConfigManager.Config.Ram);


            }
            catch(Exception exception)
            {

                using (FileStream fs = new(Directory.GetCurrentDirectory() + "\\PlayError.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(exception.ToString());
                    }
                }

                ProgressBar.Background = Brush.Parse("#8a1a1a");
                ProgressBar.ProgressTextFormat = $"Couldn't launch {selectedVersion.Id}!";
            }

            await Task.Delay(3000);

            if (!Launcher.IsLoggedIn) Username.IsEnabled = true;
            VersionList.IsEnabled = true;
            PlayButton.IsEnabled = true;
            ProgressBar.Background = Brush.Parse("#141414");
            ProgressBar.ProgressTextFormat = "";
            ProgressBar.Value = 0;
        }
    }
}