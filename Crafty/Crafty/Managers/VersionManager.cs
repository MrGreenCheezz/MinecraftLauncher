using Avalonia.Collections;
using CmlLib.Core.Version;
using Crafty.Core;
using Crafty.Models;
using System.IO;

namespace Crafty.Managers
{
	public static class VersionManager
	{
		public static MVersionCollection MVersionList = GetMVersions();

		public static AvaloniaList<Version> GetVersions()
		{
			Config config = ConfigManager.ReturnConfig();

			MVersionCollection versions;
			try
			{
				versions = Launcher.CmLauncher.GetAllVersions();
			}
			catch
			{
				versions = Launcher.LocalVersionLoader.GetVersionMetadatas();
			}

			AvaloniaList<Version> versionList = new();

			var lanch = new LauncherMain();
			var LanchVersions = lanch.GetVersions();

			foreach(var ver in LanchVersions.Versions)
			{
				int i = 1;
				versionList.Add(new Version((string?)ver, (string?)ver, "RemoteVersion" ));
				i++;
			}

			return versionList;
		}

		private static MVersionCollection GetMVersions()
		{
			MVersionCollection versions;
			try
			{
				versions = Launcher.CmLauncher.GetAllVersions();
			}
			catch
			{
				versions = Launcher.LocalVersionLoader.GetVersionMetadatas();
			}

			return versions;
		}

		public static void UpdateVersion(Version version)
		{
			if (version.IsInstalled) return;

			int index = Launcher.VersionList.IndexOf(version);
			version.Name = $"✅ {version.Name}";
			version.IsInstalled = true;
			Launcher.VersionList[index] = version;
		}
	}
}