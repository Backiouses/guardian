using System.IO;
using System.Text;
using Guardian.Networking;
using Guardian.Utilities;
using UnityEngine;

namespace Guardian.Features.Properties
{
	internal class PropertyManager : FeatureManager<Property>
	{
		private string _dataPath = GuardianClient.RootDir + "\\GameSettings.txt";

		public Property<bool> BombsKillTitans = new Property<bool>("Gamemodes_Bomb:BombsKillTitans", new string[0], value: true);

		public Property<bool> UseSkyBarrier = new Property<bool>("Gamemodes_Bomb:UseSkyBarrier", new string[0], value: true);

		public Property<bool> AnnounceRoundTime = new Property<bool>("MC_AnnounceRoundTime", new string[0], value: true);

		public Property<bool> AnnounceWaveTime = new Property<bool>("MC_AnnounceWaveTime", new string[0], value: true);

		public Property<bool> EndlessTitans = new Property<bool>("MC_EndlessTitans", new string[0], value: false);

		public Property<bool> InfiniteRoom = new Property<bool>("MC_InfiniteRoom", new string[0], value: true);

		public Property<bool> OGPunkHair = new Property<bool>("MC_OGPunkHair", new string[0], value: true);

		public Property<bool> DeadlyHooks = new Property<bool>("MC_DeadlyHooks", new string[0], value: false);

		public Property<string> ThunderSpearSkin = new Property<string>("Assets_ThunderSpearSkin", new string[0], string.Empty);

		public Property<string> LeftRopeSkin = new Property<string>("Assets_LeftRopeSkin", new string[0], string.Empty);

		public Property<float> LeftRopeTileScale = new Property<float>("Assets_LeftRopeTileScale", new string[0], 1f);

		public Property<string> RightRopeSkin = new Property<string>("Assets_RightRopeSkin", new string[0], string.Empty);

		public Property<float> RightRopeTileScale = new Property<float>("Assets_RightRopeTileScale", new string[0], 1f);

		public Property<bool> UseRawInput = new Property<bool>("Player_RawTPS-WOWInput", new string[0], value: true);

		public Property<bool> DoubleTapBurst = new Property<bool>("Player_DoubleTapBurst", new string[0], value: true);

		public Property<bool> AlternateIdle = new Property<bool>("Player_AHSSIdle", new string[0], value: false);

		public Property<bool> AlternateBurst = new Property<bool>("Player_CrossBurst", new string[0], value: false);

		public Property<bool> HideHookArrows = new Property<bool>("Player_HideHookArrows", new string[0], value: false);

		public Property<bool> HoldForBladeTrails = new Property<bool>("Player_HoldForBladeTrails", new string[0], value: true);

		public Property<bool> Interpolation = new Property<bool>("Player_Interpolation", new string[0], value: true);

		public Property<float> ReelOutScrollSmoothing = new Property<float>("Player_ReelOutScrollSmoothing", new string[0], 0.2f);

		public Property<float> OpacityOfOwnName = new Property<float>("Player_OpacityOfOwnName", new string[0], 1f);

		public Property<float> OpacityOfOtherNames = new Property<float>("Player_OpacityOfOtherNames", new string[0], 1f);

		public Property<bool> DirectionalFlares = new Property<bool>("Player_DirectionalFlares", new string[0], value: true);

		public Property<string> SuicideMessage = new Property<string>("Player_SuicideMessage", new string[0], "[FFFFFF]Suicide[-]");

		public Property<string> LavaDeathMessage = new Property<string>("Player_LavaDeathMessage", new string[0], "[FF4444]Lava[-]");

		public Property<int> LocalMinDamage = new Property<int>("Player_LocalMinDamage", new string[0], 10);

		public Property<int> MaxChatLines = new Property<int>("Chat_MaxMessages", new string[0], 100);

		public Property<bool> ChatTimestamps = new Property<bool>("Chat_Timestamps", new string[0], value: false);

		public Property<bool> DrawChatBackground = new Property<bool>("Chat_DrawBackground", new string[0], value: true);

		public Property<bool> TranslateIncoming = new Property<bool>("Chat_TranslateIncoming", new string[0], value: false);

		public Property<string> IncomingLanguage = new Property<string>("Chat_IncomingLanguage", new string[0], "auto");

		public Property<bool> TranslateOutgoing = new Property<bool>("Chat_TranslateOutgoing", new string[0], value: false);

		public Property<string> OutgoingLanguage = new Property<string>("Chat_OutgoingLanguage", new string[0], GuardianClient.SystemLanguage);

		public Property<string> JoinMessage = new Property<string>("Chat_JoinMessage", new string[0], string.Empty);

		public Property<string> ChatName = new Property<string>("Chat_UserName", new string[0], string.Empty);

		public Property<bool> BoldName = new Property<bool>("Chat_BoldName", new string[0], value: false);

		public Property<bool> ItalicName = new Property<bool>("Chat_ItalicName", new string[0], value: false);

		public Property<string> TextColor = new Property<string>("Chat_TextColor", new string[0], string.Empty);

		public Property<string> TextPrefix = new Property<string>("Chat_TextPrefix", new string[0], string.Empty);

		public Property<string> TextSuffix = new Property<string>("Chat_TextSuffix", new string[0], string.Empty);

		public Property<bool> BoldText = new Property<bool>("Chat_BoldText", new string[0], value: false);

		public Property<bool> ItalicText = new Property<bool>("Chat_ItalicText", new string[0], value: false);

		public Property<int> DrawDistance = new Property<int>("Visual_DrawDistance", new string[0], 1500);

		public Property<int> FieldOfView = new Property<int>("Visual_FieldOfView", new string[0], 50);

		public Property<bool> Blur = new Property<bool>("Visual_Blur", new string[0], value: false);

		public Property<bool> UseMainLightColor = new Property<bool>("Visual_CustomMainLightColor", new string[0], value: true);

		public Property<string> MainLightColor = new Property<string>("Visual_MainLightColor", new string[0], "FFFFFFFF");

		public Property<bool> Fog = new Property<bool>("Visual_Fog", new string[0], value: true);

		public Property<string> FogColor = new Property<string>("Visual_FogColor", new string[0], "18181865");

		public Property<float> FogDensity = new Property<float>("Visual_FogDensity", new string[0], 0.01f);

		public Property<bool> SoftShadows = new Property<bool>("Visual_SoftShadows", new string[0], value: false);

		public Property<string> Flare1Color = new Property<string>("Visual_Flare1Color", new string[0], "00FF007B");

		public Property<string> Flare2Color = new Property<string>("Visual_Flare2Color", new string[0], "FF00007B");

		public Property<string> Flare3Color = new Property<string>("Visual_Flare3Color", new string[0], "00000087");

		public Property<bool> EmissiveFlares = new Property<bool>("Visual_EmissiveFlares", new string[0], value: true);

		public Property<bool> ShowPlayerMods = new Property<bool>("Visual_ShowPlayerMods", new string[0], value: true);

		public Property<bool> ShowPlayerPings = new Property<bool>("Visual_ShowPlayerPings", new string[0], value: true);

		public Property<bool> FPSCamera = new Property<bool>("Visual_FPSCamera", new string[0], value: false);

		public Property<bool> MultiplayerNapeMeat = new Property<bool>("Visual_MultiplayerNapeMeat", new string[0], value: false);

		public Property<bool> UseRichPresence = new Property<bool>("Misc_DiscordPresence", new string[0], value: true);

		public Property<string> PhotonAppId = new Property<string>("Misc_PhotonAppId", new string[0], string.Empty);

		public Property<string> PhotonUserId = new Property<string>("Misc_PhotonUserId", new string[0], string.Empty);

		public Property<bool> ShowFramerate = new Property<bool>("Debug_ShowFramerate", new string[0], value: true);

		public Property<bool> ShowCoordinates = new Property<bool>("Debug_ShowCoordinates", new string[0], value: true);

		public Property<int> MaxLogLines = new Property<int>("Debug_MaxLogEntries", new string[0], 100);

		public Property<bool> ShowLog = new Property<bool>("Debug_ShowDebug", new string[0], value: true);

		public Property<bool> DrawDebugBackground = new Property<bool>("Debug_DrawBackground", new string[0], value: true);

		public override void Load()
		{
			Add(BombsKillTitans);
			Add(UseSkyBarrier);
			Add(AnnounceRoundTime);
			Add(AnnounceWaveTime);
			Add(EndlessTitans);
			Add(InfiniteRoom);
			Add(OGPunkHair);
			Add(DeadlyHooks);
			Add(ThunderSpearSkin);
			Add(LeftRopeSkin);
			Add(LeftRopeTileScale);
			Add(RightRopeSkin);
			Add(RightRopeTileScale);
			Add(UseRawInput);
			Add(DoubleTapBurst);
			Add(AlternateIdle);
			Add(AlternateBurst);
			Add(HideHookArrows);
			Add(HoldForBladeTrails);
			Interpolation.OnValueChanged = delegate
			{
				HERO hERO = ((IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer) ? FengGameManagerMKII.Instance.Heroes[0] : PhotonNetwork.player.GetHero());
				if (hERO != null)
				{
					hERO.rigidbody.interpolation = (Interpolation.Value ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
				}
			};
			Add(Interpolation);
			Add(ReelOutScrollSmoothing);
			OpacityOfOwnName.OnValueChanged = delegate
			{
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
				{
					foreach (HERO hero in FengGameManagerMKII.Instance.Heroes)
					{
						if (hero.photonView.isMine && hero.myNetWorkName != null)
						{
							hero.myNetWorkName.GetComponent<UILabel>().alpha = GuardianClient.Properties.OpacityOfOwnName.Value;
						}
					}
				}
			};
			Add(OpacityOfOwnName);
			OpacityOfOtherNames.OnValueChanged = delegate
			{
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
				{
					foreach (HERO hero2 in FengGameManagerMKII.Instance.Heroes)
					{
						if (!hero2.photonView.isMine && hero2.myNetWorkName != null)
						{
							hero2.myNetWorkName.GetComponent<UILabel>().alpha = GuardianClient.Properties.OpacityOfOtherNames.Value;
						}
					}
				}
			};
			Add(OpacityOfOtherNames);
			Add(DirectionalFlares);
			Add(SuicideMessage);
			Add(LavaDeathMessage);
			Add(LocalMinDamage);
			Add(MaxChatLines);
			Add(ChatTimestamps);
			Add(DrawChatBackground);
			Add(TranslateIncoming);
			Add(IncomingLanguage);
			Add(TranslateOutgoing);
			Add(OutgoingLanguage);
			Add(JoinMessage);
			Add(ChatName);
			Add(TextColor);
			Add(BoldName);
			Add(ItalicName);
			Add(TextPrefix);
			Add(TextSuffix);
			Add(BoldText);
			Add(ItalicText);
			DrawDistance.OnValueChanged = delegate
			{
				if (Camera.main != null)
				{
					Camera.main.farClipPlane = DrawDistance.Value;
				}
			};
			Add(DrawDistance);
			Blur.OnValueChanged = delegate
			{
				if (Camera.main != null)
				{
					TiltShift component3 = Camera.main.GetComponent<TiltShift>();
					if (component3 != null)
					{
						component3.enabled = Blur.Value && FengGameManagerMKII.Level != null && !FengGameManagerMKII.Level.Name.StartsWith("Custom");
					}
				}
			};
			Add(Blur);
			UseMainLightColor.OnValueChanged = delegate
			{
				MainLightColor.OnValueChanged();
			};
			Add(UseMainLightColor);
			MainLightColor.OnValueChanged = delegate
			{
				if (UseMainLightColor.Value)
				{
					GameObject gameObject2 = GameObject.Find("mainLight");
					if (gameObject2 != null)
					{
						Light component2 = gameObject2.GetComponent<Light>();
						if (gameObject2 != null)
						{
							component2.color = MainLightColor.Value.ToColor();
						}
					}
				}
			};
			Add(MainLightColor);
			Fog.OnValueChanged = delegate
			{
				RenderSettings.fog = Fog.Value;
				RenderSettings.fogMode = FogMode.Linear;
				RenderSettings.fogEndDistance = 650f;
			};
			Add(Fog);
			FogColor.OnValueChanged = delegate
			{
				RenderSettings.fogColor = FogColor.Value.ToColor();
			};
			Add(FogColor);
			FogDensity.OnValueChanged = delegate
			{
				RenderSettings.fogDensity = FogDensity.Value;
			};
			Add(FogDensity);
			SoftShadows.OnValueChanged = delegate
			{
				GameObject gameObject = GameObject.Find("mainLight");
				if (gameObject != null)
				{
					Light component = gameObject.GetComponent<Light>();
					if (component != null)
					{
						if (SoftShadows.Value)
						{
							QualitySettings.shadowCascades = 5;
							component.shadowBias = 0.04f;
							component.shadowSoftness = 32f;
						}
						else
						{
							QualitySettings.shadowCascades = 4;
							component.shadowBias = 0.15f;
							component.shadowSoftness = 4f;
						}
					}
				}
			};
			Add(SoftShadows);
			Add(Flare1Color);
			Add(Flare2Color);
			Add(Flare3Color);
			Add(EmissiveFlares);
			Add(ShowPlayerMods);
			Add(ShowPlayerPings);
			Add(FPSCamera);
			Add(MultiplayerNapeMeat);
			Add(UseRichPresence);
			PhotonAppId.OnValueChanged = delegate
			{
				PhotonApplication.Custom.Id = PhotonAppId.Value;
			};
			Add(PhotonAppId);
			Add(PhotonUserId);
			Add(ShowFramerate);
			Add(ShowCoordinates);
			Add(MaxLogLines);
			Add(ShowLog);
			Add(DrawDebugBackground);
			GuardianClient.Logger.Debug($"Registered {Elements.Count} properties.");
			LoadFromFile();
			Save();
		}

		public void LoadFromFile()
		{
			GameHelper.TryCreateFile(_dataPath, directory: false);
			string[] array = File.ReadAllLines(_dataPath);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[1] { '=' }, 2);
				Property property = Find(array2[0]);
				if (property == null)
				{
					continue;
				}
				if (property.Value is bool)
				{
					if (bool.TryParse(array2[1], out var result))
					{
						((Property<bool>)property).Value = result;
					}
				}
				else if (property.Value is int)
				{
					if (int.TryParse(array2[1], out var result2))
					{
						((Property<int>)property).Value = result2;
					}
				}
				else if (property.Value is float)
				{
					if (float.TryParse(array2[1], out var result3))
					{
						((Property<float>)property).Value = result3;
					}
				}
				else if (property.Value is string)
				{
					((Property<string>)property).Value = array2[1];
				}
			}
		}

		public override void Save()
		{
			GameHelper.TryCreateFile(_dataPath, directory: false);
			StringBuilder builder = new StringBuilder();
			Elements.ForEach(delegate(Property property)
			{
				builder.AppendLine($"{property.Name}={property.Value}");
			});
			File.WriteAllText(_dataPath, builder.ToString());
		}
	}
}
