using Avalonia;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using Crafty.Core;
using Crafty.Managers;
using Crafty.ViewModels;
using System;

namespace Crafty.Views
{
	public partial class SettingsPage : ReactiveUserControl<SettingsPageViewModel>
	{
		public SettingsPage(SettingsPageViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;

			RamSlider.Minimum = 1024;
			RamSlider.Maximum = Launcher.PhysicalMemory;
			RamSlider.Value = ConfigManager.Config.Ram;
			RamSlider.PropertyChanged += RamSlider_OnPropertyChanged;
			JavaPath.PropertyChanged += JavaPath_OnPropertyChanged;
		}

		private void RamSlider_OnPointerMoved(object? sender, PointerEventArgs e) => RamText.Text = $"{RamSlider.Value}MB";
		private void RamSlider_OnPointerExited(object? sender, PointerEventArgs e) => RamText.Text = "RAM Usage";

		private void RamSlider_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
		{
			if (RamSlider != null) ConfigManager.Config.Ram = Convert.ToInt32(RamSlider.Value);
		}

        private void JavaPath_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
		{
			if (JavaPath != null) ConfigManager.Config.UserJavaPath = JavaPath.Text;
		}

    }
}
