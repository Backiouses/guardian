using System;
using UnityEngine;

public class LevelInfo
{
	public string Name;

	private string[] Aliases = new string[0];

	public string Map;

	public string Description;

	public int Enemies;

	public bool HasSupply = true;

	public bool PlayerTitans;

	public GameMode Mode;

	public RespawnMode RespawnMode;

	public bool NoCrawlers;

	public bool Hints;

	public bool Lava;

	public bool Horses;

	public bool Punks = true;

	public bool PVP;

	public static LevelInfo[] Levels;

	private static bool Initialized;

	public Minimap.Preset MinimapPreset;

	public static LevelInfo GetInfo(string name)
	{
		InitData();
		LevelInfo[] levels = Levels;
		foreach (LevelInfo levelInfo in levels)
		{
			if (levelInfo.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
			{
				return levelInfo;
			}
			string[] aliases = levelInfo.Aliases;
			for (int j = 0; j < aliases.Length; j++)
			{
				if (aliases[j].Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					return levelInfo;
				}
			}
		}
		return Levels[3];
	}

	public static void InitData()
	{
		if (!Initialized)
		{
			Initialized = true;
			Levels = new LevelInfo[29]
			{
				new LevelInfo
				{
					Name = "[S]Tutorial",
					Aliases = new string[1] { "tutorial" },
					Map = "tutorial",
					Description = "Learn the basic functionality of AoTTG.",
					Enemies = 1,
					Mode = GameMode.KillTitans,
					RespawnMode = RespawnMode.Never,
					HasSupply = true,
					Hints = true,
					Punks = false
				},
				new LevelInfo
				{
					Name = "[S]Battle training",
					Aliases = new string[2] { "battle training", "training" },
					Map = "tutorial 1",
					Description = "Basic offensive training course.",
					Enemies = 7,
					Mode = GameMode.KillTitans,
					RespawnMode = RespawnMode.Never,
					HasSupply = true,
					Punks = false
				},
				new LevelInfo
				{
					Name = "[S]City",
					Map = "The City I",
					Description = "Kill all 15 titans invading the city!",
					Enemies = 15,
					Mode = GameMode.KillTitans,
					RespawnMode = RespawnMode.Never,
					HasSupply = true
				},
				new LevelInfo
				{
					Name = "[S]Forest",
					Map = "The Forest",
					Description = "Kill all 15 titans!",
					Enemies = 15,
					Mode = GameMode.KillTitans,
					RespawnMode = RespawnMode.Never,
					HasSupply = true
				},
				new LevelInfo
				{
					Name = "[S]Forest Survive(no crawler)",
					Map = "The Forest",
					Description = "Survive all 20 waves. (No crawlers)",
					Enemies = 3,
					Mode = GameMode.Survival,
					RespawnMode = RespawnMode.Never,
					HasSupply = true,
					NoCrawlers = true
				},
				new LevelInfo
				{
					Name = "[S]Forest Survive(no crawler no punk)",
					Map = "The Forest",
					Description = "Survive all 20 waves. (No crawlers, no punks)",
					Enemies = 3,
					Mode = GameMode.Survival,
					RespawnMode = RespawnMode.Never,
					HasSupply = true,
					NoCrawlers = true,
					Punks = false
				},
				new LevelInfo
				{
					Name = "[S]Racing - Akina",
					Map = "track - Akina",
					Description = "Test your speed!",
					Enemies = 0,
					Mode = GameMode.Racing,
					RespawnMode = RespawnMode.Never,
					HasSupply = false,
					MinimapPreset = new Minimap.Preset(new Vector3(443.2f, 0f, 1912.6f), 1929.042f)
				},
				new LevelInfo
				{
					Name = "The City",
					Aliases = new string[1] { "city" },
					Map = "The City I",
					Description = "Kill all 10 titans invading the city! (Player titans, PvP, no respawns)",
					Enemies = 10,
					Mode = GameMode.KillTitans,
					RespawnMode = RespawnMode.Never,
					HasSupply = true,
					PlayerTitans = true,
					PVP = true,
					MinimapPreset = new Minimap.Preset(new Vector3(22.6f, 0f, 13f), 731.9738f)
				},
				new LevelInfo
				{
					Name = "The City II",
					Aliases = new string[2] { "city 2", "city ii" },
					Map = "The City I",
					Description = "Kill all 10 titans invading the city! (Player titans, PvP, 10s respawn)",
					Enemies = 10,
					Mode = GameMode.KillTitans,
					RespawnMode = RespawnMode.Deathmatch,
					HasSupply = true,
					PlayerTitans = true,
					PVP = true
				},
				new LevelInfo
				{
					Name = "The City III",
					Aliases = new string[2] { "city 3", "city iii" },
					Map = "The City I",
					Description = "Capture each checkpoint to win!",
					Enemies = 0,
					Mode = GameMode.PvPCapture,
					RespawnMode = RespawnMode.Deathmatch,
					HasSupply = true,
					PlayerTitans = true,
					MinimapPreset = new Minimap.Preset(new Vector3(22.6f, 0f, 13f), 734.9738f)
				},
				new LevelInfo
				{
					Name = "The City IV",
					Aliases = new string[2] { "city 4", "city iv" },
					Map = "The City I",
					Description = "Survive all 20 waves. (No respawns)",
					Enemies = 3,
					Mode = GameMode.Survival,
					RespawnMode = RespawnMode.Never,
					HasSupply = true
				},
				new LevelInfo
				{
					Name = "The City V",
					Aliases = new string[2] { "city 5", "city v" },
					Map = "The City I",
					Description = "Survive all 20 waves. (Respawn on each new wave)",
					Enemies = 3,
					Mode = GameMode.Survival,
					RespawnMode = RespawnMode.NewRound,
					HasSupply = true
				},
				new LevelInfo
				{
					Name = "The Forest",
					Aliases = new string[1] { "forest" },
					Map = "The Forest",
					Description = "The Forest of Giant Trees. (Player titans, PvP, no respawns)",
					Enemies = 10,
					Mode = GameMode.KillTitans,
					RespawnMode = RespawnMode.Never,
					HasSupply = true,
					PlayerTitans = true,
					PVP = true
				},
				new LevelInfo
				{
					Name = "The Forest II",
					Aliases = new string[2] { "forest 2", "forest ii" },
					Map = "The Forest",
					Description = "Survive all 20 waves in The Forest of Giant Trees. (No respawns)",
					Enemies = 3,
					Mode = GameMode.Survival,
					RespawnMode = RespawnMode.Never,
					HasSupply = true
				},
				new LevelInfo
				{
					Name = "The Forest III",
					Aliases = new string[2] { "forest 3", "forst iii" },
					Map = "The Forest",
					Description = "Survive all 20 waves in The Forest of Giant Trees. (Respawn on each new wave)",
					Enemies = 3,
					Mode = GameMode.Survival,
					RespawnMode = RespawnMode.NewRound,
					HasSupply = true
				},
				new LevelInfo
				{
					Name = "The Forest IV  - LAVA",
					Aliases = new string[6] { "the forest iv - lava", "forest 4", "forest iv", "forest 4 lava", "forest iv lava", "forest lava" },
					Map = "The Forest",
					Description = "The floor is LAVA!\nSurvive all 20 waves in The Forest of Giant Trees WITHOUT touching the ground. (Respawn on each new wave, no crawlers)",
					Enemies = 3,
					Mode = GameMode.Survival,
					RespawnMode = RespawnMode.NewRound,
					HasSupply = true,
					NoCrawlers = true,
					Lava = true
				},
				new LevelInfo
				{
					Name = "Outside The Walls",
					Aliases = new string[1] { "otw" },
					Map = "OutSide",
					Description = "Capture each checkpoint to win! (Player titans, 10s respawn)",
					Enemies = 0,
					Mode = GameMode.PvPCapture,
					RespawnMode = RespawnMode.Deathmatch,
					HasSupply = true,
					Horses = true,
					PlayerTitans = true,
					MinimapPreset = new Minimap.Preset(new Vector3(2549.4f, 0f, 3042.4f), 3697.16f)
				},
				new LevelInfo
				{
					Name = "Racing - Akina",
					Aliases = new string[1] { "akina" },
					Map = "track - Akina",
					Description = "Test your speed!",
					Enemies = 0,
					Mode = GameMode.Racing,
					RespawnMode = RespawnMode.Never,
					HasSupply = false,
					PVP = true,
					MinimapPreset = new Minimap.Preset(new Vector3(443.2f, 0f, 1912.6f), 1929.042f)
				},
				new LevelInfo
				{
					Name = "Annie",
					Map = "The Forest",
					Description = "You only have 1 life. Be careful soldier!\nNape & Ankle Armor:\nNormal: 1000 / 50\nHard: 2500 / 100\nAbnormal: 4000 / 200",
					Enemies = 15,
					Mode = GameMode.KillTitans,
					RespawnMode = RespawnMode.Never,
					Punks = false,
					PVP = true
				},
				new LevelInfo
				{
					Name = "Annie II",
					Aliases = new string[1] { "annie 2" },
					Map = "The Forest",
					Description = "Nape & Ankle Armor:\nNormal: 1000 / 50\nHard: 2500 / 100\nAbnormal: 4000 / 200\n(10s respawn)",
					Enemies = 15,
					Mode = GameMode.KillTitans,
					RespawnMode = RespawnMode.Deathmatch,
					Punks = false,
					PVP = true
				},
				new LevelInfo
				{
					Name = "Colossal Titan",
					Aliases = new string[1] { "ct" },
					Map = "Colossal Titan",
					Description = "You only have 1 life. Be careful soldier!\nDefeat the Colossal Titan.\nPrevent the abnormal titan from running to the north gate.\nNape Armor:\nNormal: 2000\nHard: 3500\nAbnormal: 5000",
					Enemies = 2,
					Mode = GameMode.Colossal,
					RespawnMode = RespawnMode.Never,
					MinimapPreset = new Minimap.Preset(new Vector3(8.8f, 0f, 65f), 765.5751f)
				},
				new LevelInfo
				{
					Name = "Colossal Titan II",
					Aliases = new string[2] { "ct 2", "ct ii" },
					Map = "Colossal Titan",
					Description = "Defeat the Colossal Titan.\nPrevent the abnormal titan from running to the north gate.\nNape Armor:\n Normal: 5000\nHard: 8000\nAbnormal: 12000\n(10s respawn)",
					Enemies = 2,
					Mode = GameMode.Colossal,
					RespawnMode = RespawnMode.Deathmatch,
					MinimapPreset = new Minimap.Preset(new Vector3(8.8f, 0f, 65f), 765.5751f)
				},
				new LevelInfo
				{
					Name = "Trost",
					Map = "Colossal Titan",
					Description = "Escort Titan Eren to seal the hole in Wall Rose! (No respawns)",
					Enemies = 2,
					Mode = GameMode.Trost,
					RespawnMode = RespawnMode.Never,
					Punks = false
				},
				new LevelInfo
				{
					Name = "Trost II",
					Aliases = new string[1] { "trost 2" },
					Map = "Colossal Titan",
					Description = "Escort Titan Eren to seal the hole in Wall Rose! (10s respawn)",
					Enemies = 2,
					Mode = GameMode.Trost,
					RespawnMode = RespawnMode.Deathmatch,
					Punks = false
				},
				new LevelInfo
				{
					Name = "Cave Fight",
					Map = "CaveFight",
					Description = "PVP combat in the cavern underneath the Reiss Chapel.",
					Enemies = 0,
					Mode = GameMode.TeamDeathmatch,
					RespawnMode = RespawnMode.Never,
					HasSupply = true,
					PlayerTitans = true,
					PVP = true
				},
				new LevelInfo
				{
					Name = "House Fight",
					Map = "HouseFight",
					Description = "PVP combat.",
					Enemies = 0,
					Mode = GameMode.TeamDeathmatch,
					RespawnMode = RespawnMode.Never,
					HasSupply = true,
					PlayerTitans = true,
					PVP = true
				},
				new LevelInfo
				{
					Name = "Custom",
					Map = "The Forest",
					Description = "RC Custom Maps (Player titans allowed)",
					Enemies = 1,
					Mode = GameMode.KillTitans,
					RespawnMode = RespawnMode.Never,
					PVP = true,
					Punks = true,
					HasSupply = true,
					PlayerTitans = true
				},
				new LevelInfo
				{
					Name = "Custom (No PT)",
					Map = "The Forest",
					Description = "RC Custom Maps (No Player Titans)",
					Enemies = 1,
					Mode = GameMode.KillTitans,
					RespawnMode = RespawnMode.Never,
					PVP = true,
					Punks = true,
					HasSupply = true,
					PlayerTitans = false
				},
				new LevelInfo
				{
					Name = "Custom-Anarchy (No PT)",
					Aliases = new string[1] { "anarchy" },
					Map = "The Forest",
					Description = "Custom Maps with Anarchy Mod Scripts (No Player Titans)",
					Enemies = 1,
					Mode = GameMode.KillTitans,
					RespawnMode = RespawnMode.Never,
					PVP = true,
					HasSupply = true,
					PlayerTitans = false
				}
			};
		}
	}
}
