using Avalonia.Collections;
using Avalonia.Media.Imaging;
using CmlLib.Core.Downloader;
using Crafty.Core;
using Crafty.Managers;
using ReactiveUI;
using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Version = Crafty.Models.Version;

namespace Crafty.ViewModels
{
	public class MainWindowViewModel : ReactiveObject, IScreen
	{
		public LauncherMain localLauncher;
		public MainWindowViewModel()
		{
			

			try
			{
				if (File.Exists($"{Launcher.MinecraftPath}/crafty_session.json"))
				{
					var loginTask = Task.Run(Launcher.Login);
					loginTask.Wait();
					Username = Launcher.Session.Username;
				}
				else throw new Exception("Couldn't find session file");
			}
			catch
			{
				Username = ConfigManager.Config.Username;
			}

            LauncherMain.Instance.ProgressChanged +=(s, e) =>
            {

                ProgressBarText = $"Preparing libraries..{e.ProgressPercentage}%)";


                ProgressBarMaximum = 100;
                ProgressBarValue = e.ProgressPercentage;
            };

            try
			{
				SelectedItem = Launcher.VersionList.Where(x => x.Id == ConfigManager.Config.LastVersionUsed).First();
			}
			catch
			{
				SelectedItem = null;
			}

			NavigateAboutCommand = ReactiveCommand.Create(NavigateAbout);
			NavigateAccountCommand = ReactiveCommand.Create(NavigateAccount);
			NavigateSettingsCommand = ReactiveCommand.Create(NavigateSettings);
			NavigateBackCommand = ReactiveCommand.Create(NavigateBack);
		}

		public string Title => $"GcLauncher ({Launcher.Version})";

		public Task<Bitmap> Cover => RandomManager.RandomCover();

		private string _progressBarText;

		private bool _saveUsername;

		public bool SaveUsername
		{
			get => _saveUsername;
			set => this.RaiseAndSetIfChanged(ref _saveUsername, value);
		}

		public string ProgressBarText
		{
			get => _progressBarText;
			set => this.RaiseAndSetIfChanged(ref _progressBarText, value);
		}

		private double _progressBarMaximum = 100;

		public LauncherMain CustomLauncher
		{
			get => localLauncher;
			set => this.localLauncher = value;
		}

		public double ProgressBarMaximum
		{
			get => _progressBarMaximum;
			set => this.RaiseAndSetIfChanged(ref _progressBarMaximum, value);
		}

		private double _progressBarValue;

		public double ProgressBarValue
		{
			get => _progressBarValue;
			set => this.RaiseAndSetIfChanged(ref _progressBarValue, value);
		}

		private AvaloniaList<Version> _versionList;

		public AvaloniaList<Version> VersionList
		{
			get => Launcher.VersionList;
			set => this.RaiseAndSetIfChanged(ref _versionList, value);
		}

		private bool _isLoggedIn;

		public bool IsLoggedIn
		{
			get => Launcher.IsLoggedIn;
			set => this.RaiseAndSetIfChanged(ref _isLoggedIn, value);
		}

		private string _username;

		public string Username
		{
			get => _username;
			set => this.RaiseAndSetIfChanged(ref _username, value);
		}

		private Version? _selectedItem;

		public Version? SelectedItem
		{
			get => _selectedItem;
			set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
		}

		public RoutingState Router { get; } = new();

		public ReactiveCommand<Unit, Unit> NavigateAboutCommand { get; }
		public ReactiveCommand<Unit, Unit> NavigateAccountCommand { get; }
		public ReactiveCommand<Unit, Unit> NavigateModBrowserCommand { get; }
		public ReactiveCommand<Unit, Unit> NavigateSettingsCommand { get; }
		public ReactiveCommand<Unit, Unit> NavigateBackCommand { get; }

		private void NavigateAbout() => Router.Navigate.Execute(new AboutPageViewModel(this));
		private void NavigateAccount() => Router.Navigate.Execute(new AccountPageViewModel(this, Router));
		private void NavigateSettings() => Router.Navigate.Execute(new SettingsPageViewModel(this));

		public void NavigateBack()
		{
			try
			{
				IRoutableViewModel? currentViewModel = Router.GetCurrentViewModel();

				if (currentViewModel.GetType() == typeof(AccountPageViewModel) && Launcher.IsLoggedIn)
				{
					Username = Launcher.Session.Username;
				}
				else if (currentViewModel.GetType() == typeof(SettingsPageViewModel))
				{
					ConfigManager.SaveConfig();
				}

				Router.NavigateBack.Execute();
			}
			catch { }
		}
	}
}