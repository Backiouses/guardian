using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;

namespace Guardian.AntiAbuse
{
	internal class ModDetector
	{
		public static void OnPlayerPropertyModification(object[] playerAndUpdatedProps)
		{
			PhotonPlayer photonPlayer = playerAndUpdatedProps[0] as PhotonPlayer;
			foreach (DictionaryEntry item in playerAndUpdatedProps[1] as ExitGames.Client.Photon.Hashtable)
			{
				switch (item.Key as string)
				{
				case "Ping":
				{
					object value = item.Value;
					if (value is int)
					{
						int num = (photonPlayer.Ping = (int)value);
					}
					continue;
				}
				case "FoxMod":
					photonPlayer.IsFoxMod = true;
					continue;
				case "guildName":
					if (item.Value is string text && (text.Equals("photonMod") || text.Equals("photonMod2")))
					{
						photonPlayer.IsPhotonMod = true;
					}
					continue;
				}
				if (item.Value is string text2 && (text2.Equals("N_user") || text2.Equals("N_owner")))
				{
					photonPlayer.IsNekoMod = true;
					photonPlayer.IsNekoModUser = text2.Equals("N_user");
					photonPlayer.IsNekoModOwner = text2.Equals("N_owner");
				}
			}
		}

		public static List<string> GetMods(PhotonPlayer player)
		{
			List<string> list = new List<string>();
			ExitGames.Client.Photon.Hashtable customProperties = player.customProperties;
			if (player.IsNekoMod)
			{
				List<string> list2 = new List<string>();
				if (player.IsNekoModUser)
				{
					list2.Add("user");
				}
				if (player.IsNekoModOwner)
				{
					list2.Add("owner");
				}
				list.Add("[EE00EE][Neko[FFFFFF](" + string.Join(",", list2.ToArray()) + ")[-]]");
			}
			if (player.IsFoxMod)
			{
				list.Add("[FF6600][Fox]");
			}
			if (player.IsCyrusMod)
			{
				list.Add("[FFFF00][CE]");
			}
			if (player.IsAnarchyMod || (player.customProperties.ContainsKey("AnarchyFlags") && player.customProperties["AnarchyFlags"] is int) || (player.customProperties.ContainsKey("AnarchyAbuseFlags") && player.customProperties["AnarchyAbuseFlags"] is int))
			{
				list.Add("[00BBCC][Anarchy]");
			}
			if (player.IsKNKMod)
			{
				list.Add("[FF0000][KnK]");
			}
			if (player.IsNRCMod)
			{
				list.Add("[FFFFFF][NRC]");
			}
			if (player.IsTRAPMod)
			{
				list.Add("[EE66FF][TRAP]");
			}
			if (player.IsRC83Mod)
			{
				list.Add("[FFFFFF][RC83]");
			}
			if (customProperties.ContainsKey("GuardianMod"))
			{
				List<string> list3 = new List<string>();
				if (customProperties["GuardianMod"] is string)
				{
					list3.Add(GExtensions.AsString(customProperties["GuardianMod"]));
				}
				else if (customProperties["GuardianMod"] is int)
				{
					list3.Add("legacy");
				}
				if (customProperties.ContainsKey("Stats") && customProperties["Stats"] is int)
				{
					List<char> list4 = ModifiedStats.FromInt(GExtensions.AsInt(customProperties["Stats"]));
					if (list4.Count > 0)
					{
						list3.Add("inf:" + string.Join(",", list4.Select((char c) => c.ToString()).ToArray()));
					}
					else
					{
						list3.Add("inf:none");
					}
				}
				list.Add("[FFBB00][Guardian[FFFFFF](" + string.Join(",", list3.ToArray()) + ")[-]]");
			}
			for (int i = 0; i < 5; i++)
			{
				if (player.customProperties.ContainsKey($"CO_SLOT_{i}") && player.customProperties[$"CO_SLOT_{i}"] is string)
				{
					list.Add("[FFCCFF][AliceRC]");
					break;
				}
			}
			if (customProperties.ContainsKey("ZMOD") && customProperties["ZMOD"] is string && ((customProperties.ContainsKey("idleGas") && customProperties["idleGas"] is bool) || (customProperties.ContainsKey("idleEffect") && customProperties["idleEffect"] is string) || (customProperties.ContainsKey("infGas") && customProperties["infGas"] is bool) || (customProperties.ContainsKey("infBlades") && customProperties["infBlades"] is bool) || (customProperties.ContainsKey("infAHSS") && customProperties["infAHSS"] is bool)))
			{
				List<string> list5 = new List<string>();
				if (customProperties.ContainsKey("infGas") && (bool)customProperties["infGas"])
				{
					list5.Add("infGas");
				}
				if (customProperties.ContainsKey("infBlades") && (bool)customProperties["infBlades"])
				{
					list5.Add("infBla");
				}
				if (customProperties.ContainsKey("infAHSS") && (bool)customProperties["infAHSS"])
				{
					list5.Add("infAhss");
				}
				list.Add("[550055][ZMOD[FFFFFF](" + string.Join(",", list5.ToArray()) + ")[-]]");
			}
			if (customProperties.ContainsKey("Xeres") && GExtensions.AsString(customProperties["Xeres"]).Equals("yo mama perhaps") && ((customProperties.ContainsKey("infGas") && customProperties["infGas"] is bool) || (customProperties.ContainsKey("infBlades") && customProperties["infBlades"] is bool) || (customProperties.ContainsKey("infAHSS") && customProperties["infAHSS"] is bool)))
			{
				List<string> list6 = new List<string>();
				if (customProperties.ContainsKey("infGas") && (bool)customProperties["infGas"])
				{
					list6.Add("infGas");
				}
				if (customProperties.ContainsKey("infBlades") && (bool)customProperties["infBlades"])
				{
					list6.Add("infBla");
				}
				if (customProperties.ContainsKey("infAHSS") && (bool)customProperties["infAHSS"])
				{
					list6.Add("infAhss");
				}
				list.Add("[000000][Xeres[FFFFFF](" + string.Join(",", list6.ToArray()) + ")[-]]");
			}
			if (customProperties.ContainsKey("CatielRC"))
			{
				list.Add("[FFFFFF][CatielRC]");
			}
			if (customProperties.ContainsKey("NoodleDoodle"))
			{
				list.Add("[FF6600][NudelBot]");
			}
			if ((customProperties.ContainsKey("A.S Guard") && customProperties["A.S Guard"] is int) || (customProperties.ContainsKey("Allstar Mod") && customProperties["Allstar Mod"] is int))
			{
				list.Add("[FFFFFF][[FF0000]A[-]llStar]");
			}
			if (customProperties.ContainsKey("dogshitmod") && GExtensions.AsString(customProperties["dogshitmod"]).Equals("dogshitmod"))
			{
				list.Add("[FFFFFF][DogS]");
			}
			if (customProperties.ContainsKey("LNON"))
			{
				list.Add("[FFFFFF][LNON]");
			}
			if (customProperties.ContainsKey("Ignis"))
			{
				list.Add("[FFFFFF][Ignis]");
			}
			if (customProperties.ContainsKey("Krab"))
			{
				list.Add("[FF0000][Krab]");
			}
			if (player.IsPBMod || customProperties.ContainsKey("PBModRC"))
			{
				list.Add("[FF6600][Pedo[553300]Bear]");
			}
			if (customProperties.ContainsKey("DiscipleMod"))
			{
				list.Add("[777777][Disciple]");
			}
			if (customProperties.ContainsKey("TLW"))
			{
				list.Add("[FFFFFF][TLW]");
			}
			if (customProperties.ContainsKey("ARC-CREADOR"))
			{
				list.Add("[FFFFFF][ARC (Creator)]");
			}
			if (customProperties.ContainsKey("ARC"))
			{
				list.Add("[FFFFFF][ARC]");
			}
			if (customProperties.ContainsKey("SRC"))
			{
				list.Add("[FFFFFF][SRC]");
			}
			if (player.IsCyanMod || customProperties.ContainsKey("CyanMod") || customProperties.ContainsKey("CyanModNew"))
			{
				list.Add("[00FFFF][Cyan]");
			}
			if (player.IsExpeditionMod || customProperties.ContainsKey("ExpMod") || customProperties.ContainsKey("EMID") || customProperties.ContainsKey("Version") || customProperties.ContainsKey("Pref"))
			{
				List<string> list7 = new List<string>();
				if (customProperties.ContainsKey("Version"))
				{
					list7.Add(GExtensions.AsFloat(customProperties["Version"]).ToString());
				}
				if (customProperties.ContainsKey("Pref"))
				{
					list7.Add(GExtensions.AsString(customProperties["Pref"]));
				}
				list.Add("[009900][Expedition[FFFFFF](" + string.Join(string.Empty, list7.ToArray()) + ")[-]]");
			}
			if (player.IsUniverseMod || customProperties.ContainsKey("UPublica") || customProperties.ContainsKey("UPublica2") || customProperties.ContainsKey("UGrup") || customProperties.ContainsKey("Hats") || customProperties.ContainsKey("UYoutube") || customProperties.ContainsKey("UVip") || customProperties.ContainsKey("SUniverse") || customProperties.ContainsKey("UAdmin") || customProperties.ContainsKey("coins") || (customProperties.ContainsKey(string.Empty) && customProperties[string.Empty] is string))
			{
				List<string> list8 = new List<string>();
				if (customProperties.ContainsKey("UYoutube"))
				{
					list8.Add("y[FF0000]t[-]");
				}
				if (customProperties.ContainsKey("UVip"))
				{
					list8.Add("[FFCC00]vip[-]");
				}
				if (customProperties.ContainsKey("UAdmin"))
				{
					list8.Add("[FF0000]admin[-]");
				}
				list.Add("[AA00AA][Universe[FFFFFF](" + string.Join(",", list8.ToArray()) + ")[-]]");
			}
			if (customProperties.ContainsKey("Teiko"))
			{
				list.Add("[AED6F1][Teiko]");
			}
			if (customProperties.ContainsKey("Wings") || customProperties.ContainsKey("EarCat") || customProperties.ContainsKey("Horns"))
			{
				list.Add("[FFFFFF][SLB]");
			}
			if (player.IsRRCMod || customProperties.ContainsKey("bronze") || customProperties.ContainsKey("diamond") || (customProperties.ContainsKey(string.Empty) && customProperties[string.Empty] is int))
			{
				list.Add("[FFFFFF][RRC]");
			}
			if (customProperties.ContainsKey("DeadInside"))
			{
				list.Add("[000000][DeadInside]");
			}
			if (customProperties.ContainsKey("DFSAO"))
			{
				list.Add("[FFFFFF][DFSAO]");
			}
			if (customProperties.ContainsKey("AOE") && GExtensions.AsString(customProperties["AOE"]).Equals("Made By Exile"))
			{
				list.Add("[0000FF][AoE]");
			}
			if (customProperties.ContainsKey("FSociety") || customProperties.ContainsKey("Fsociety"))
			{
				list.Add("Fsociety");
			}
			if (player.IsPhotonMod)
			{
				list.Add("Photon");
			}
			string text = GExtensions.AsString(player.customProperties[PhotonPlayerProperty.Name]);
			string text2 = GExtensions.AsString(player.customProperties[PhotonPlayerProperty.Guild]);
			if (text2.StartsWith("[00FF00]PARROT'S MOD"))
			{
				list.Add("[00FF00][PARROT]");
			}
			if (player.IsUnknownMod || customProperties.ContainsKey("FGT") || customProperties.ContainsKey("me") || customProperties.ContainsKey("Taquila") || customProperties.ContainsKey("Pain") || customProperties.ContainsKey("uishot"))
			{
				list.Add("[FFFFFF][???]");
			}
			if (customProperties.ContainsKey(PhotonPlayerProperty.RCTeam) || customProperties.ContainsKey(PhotonPlayerProperty.RCBombR) || customProperties.ContainsKey(PhotonPlayerProperty.RCBombG) || customProperties.ContainsKey(PhotonPlayerProperty.RCBombB) || customProperties.ContainsKey(PhotonPlayerProperty.RCBombA) || customProperties.ContainsKey(PhotonPlayerProperty.RCBombRadius) || customProperties.ContainsKey(PhotonPlayerProperty.CustomBool) || customProperties.ContainsKey(PhotonPlayerProperty.CustomFloat) || customProperties.ContainsKey(PhotonPlayerProperty.CustomInt) || customProperties.ContainsKey(PhotonPlayerProperty.CustomString))
			{
				List<string> list9 = new List<string>();
				if (player.IsNewRCMod)
				{
					list9.Add("new");
				}
				list.Add("[9999FF][RC[FFFFFF](" + string.Join(",", list9.ToArray()) + ")[-]]");
			}
			if (text.Length > 48 || text2.Length > 40)
			{
				string text3 = string.Empty;
				if (text.Length > 48)
				{
					text3 += ">48";
				}
				if (text2.Length > 40)
				{
					text3 += ((text.Length > 48) ? "|>40" : ">40");
				}
				list.Add("[FFFFFF][" + text3 + "]");
			}
			return list;
		}
	}
}
