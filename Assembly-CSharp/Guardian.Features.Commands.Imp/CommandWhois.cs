namespace Guardian.Features.Commands.Impl
{
	internal class CommandWhois : Command
	{
		public CommandWhois()
			: base("whois", new string[0], "<id>", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length < 1 || !int.TryParse(args[0], out var result))
			{
				return;
			}
			PhotonPlayer photonPlayer = PhotonPlayer.Find(result);
			if (photonPlayer != null)
			{
				irc.AddLine($"Whois Report (#{photonPlayer.Id})".AsColor("AAFF00").AsBold());
				irc.AddLine("Name: ".AsColor("FFCC00") + GExtensions.AsString(photonPlayer.customProperties[PhotonPlayerProperty.Name]).NGUIToUnity());
				irc.AddLine("Guild: ".AsColor("FFCC00") + GExtensions.AsString(photonPlayer.customProperties[PhotonPlayerProperty.Guild]).NGUIToUnity());
				irc.AddLine("Status: ".AsColor("FFCC00") + (GExtensions.AsBool(photonPlayer.customProperties[PhotonPlayerProperty.IsDead]) ? "Dead" : "Alive"));
				int num = GExtensions.AsInt(photonPlayer.customProperties[PhotonPlayerProperty.Kills]);
				int num2 = GExtensions.AsInt(photonPlayer.customProperties[PhotonPlayerProperty.Deaths]);
				int num3 = GExtensions.AsInt(photonPlayer.customProperties[PhotonPlayerProperty.TotalDamage]);
				irc.AddLine("Kills: ".AsColor("FFCC00") + num);
				irc.AddLine("Deaths: ".AsColor("FFCC00") + num2);
				irc.AddLine("K/D Ratio: ".AsColor("FFCC00") + ((num2 < 2) ? ((double)num) : ((double)num / (double)num2)).ToString("F2") + $" ({num}:{num2})");
				irc.AddLine("Max Damage: ".AsColor("FFCC00") + GExtensions.AsInt(photonPlayer.customProperties[PhotonPlayerProperty.MaxDamage]));
				irc.AddLine("Total Damage: ".AsColor("FFCC00") + num3);
				irc.AddLine("Average Damage: ".AsColor("FFCC00") + ((num == 0) ? "n/a" : (num3 / num).ToString()));
				float num4 = GExtensions.AsFloat(photonPlayer.customProperties[PhotonPlayerProperty.RCBombRadius]);
				if (photonPlayer.customProperties.ContainsKey(PhotonPlayerProperty.RCBombRadius))
				{
					irc.AddLine("Bomb Radius: ".AsColor("FFCC00") + (num4 - 20f) / 4f);
				}
				string text = "Human (Blade)";
				if (photonPlayer.IsAhss)
				{
					text = "Human (AHSS)";
				}
				else if (photonPlayer.IsTitan)
				{
					text = "Player Titan";
				}
				irc.AddLine("Team: ".AsColor("FFCC00") + text);
			}
		}
	}
}
