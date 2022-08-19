using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Guardian.AntiAbuse.Validators
{
	internal class SkinChecker
	{
		public static string AllowedHostsPath = GuardianClient.RootDir + "\\Hosts.txt";

		public static List<string> AllowedHosts = new List<string>();

		public static void Init()
		{
			if (!File.Exists(AllowedHostsPath))
			{
				AllowedHosts.Add("i.imgur.com");
				AllowedHosts.Add("imgur.com");
				AllowedHosts.Add("cdn.discordapp.com");
				AllowedHosts.Add("cdn.discord.com");
				AllowedHosts.Add("media.discordapp.net");
				AllowedHosts.Add("i.gyazo.com");
				File.WriteAllLines(AllowedHostsPath, AllowedHosts.ToArray());
			}
			AllowedHosts = new List<string>(File.ReadAllLines(AllowedHostsPath));
			if (AllowedHosts.Count < 1)
			{
				GuardianClient.Logger.Warn("Allowing ALL hosts for skins.");
				GuardianClient.Logger.Warn("\tThis leaves you at risk of being IP-logged!".AsColor("FF0000"));
			}
			else
			{
				GuardianClient.Logger.Debug($"Allowing {AllowedHosts.Count} host(s) for skins.");
			}
		}

		public static WWW CreateWWW(string url)
		{
			if (url.ToLower().StartsWith("file://"))
			{
				return new WWW(url);
			}
			if (AllowedHosts.Count < 1)
			{
				return new WWW(url);
			}
			if (!Uri.TryCreate(url, UriKind.Absolute, out var result))
			{
				return null;
			}
			string text = result.Authority;
			if (text.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
			{
				text = text.Substring(4);
			}
			foreach (string allowedHost in AllowedHosts)
			{
				string text2 = allowedHost;
				if (text2.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
				{
					text2 = text2.Substring(4);
				}
				if (text.Equals(text2, StringComparison.OrdinalIgnoreCase))
				{
					return new WWW(url);
				}
			}
			if (text.Length > 0)
			{
				GuardianClient.Logger.Warn("Denied skin host: " + text);
			}
			return null;
		}
	}
}
