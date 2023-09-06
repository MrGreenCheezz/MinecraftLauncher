﻿using Avalonia.Collections;
using Avalonia.Media.Imaging;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;
using CmlLib.Core.VersionLoader;
using Crafty.Managers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Version = Crafty.Models.Version;

namespace Crafty.Core;

public static class Launcher
{
	public static readonly string Version = "v2.0";
	public static readonly string MinecraftPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/.GcLauncher";
	public static CMLauncher CmLauncher = new(new MinecraftPath(MinecraftPath));
	public static LocalVersionLoader LocalVersionLoader = new(CmLauncher.MinecraftPath);
	public static AvaloniaList<Version> VersionList = VersionManager.GetVersions();
	public static MSession? Session;
	public static JavaEditionLoginHandler? LoginHandler;
	public static bool IsLoggedIn;
	public static Bitmap? Skin;

	public static readonly string AllowedUsernameChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_1234567890";
	public static int PhysicalMemory = GetPhysicalMemory();

	public static async Task<bool> Login()
	{
		if (!IsLoggedIn)
		{
			var session = await MsalClientManager.Login();

			if (session == null) return false;

			Session = session;
			IsLoggedIn = true;
		}

		return true;
	}

	public static async Task Logout()
	{
		MsalClientManager.Logout();
		Skin = null;
		IsLoggedIn = false;
	}

	public static bool CheckUsername(string username)
	{
		foreach (char unvalid in username) if (!AllowedUsernameChars.Contains(unvalid.ToString())) return false;
		if (username.Length < 3 || username.Length > 16 || string.IsNullOrEmpty(username)) return false;

		return true;
	}

	private static int GetPhysicalMemory()
	{
		decimal installedMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
		int physicalMemory = (int)Math.Round(installedMemory / 1048576);
		Debug.WriteLine($"Physical Memory: {physicalMemory}MB");

		return physicalMemory;
	}
}