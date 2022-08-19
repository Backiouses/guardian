using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using Discord;
using ExitGames.Client.Photon;
using Guardian.AntiAbuse;
using Guardian.AntiAbuse.Validators;
using Guardian.Features.Commands;
using Guardian.Features.Gamemodes;
using Guardian.Features.Properties;
using Guardian.Networking;
using Guardian.UI;
using Guardian.UI.Toasts;
using Guardian.Utilities;
using UnityEngine;

namespace Guardian
{
	internal class GuardianClient : MonoBehaviour
	{
		public static readonly string Build = "1.0.0";

		public static readonly string RootDir = Application.dataPath + "\\..";

		public static readonly string CustomPropertyName = "GuardianMod";

		public static readonly CommandManager Commands = new CommandManager();

		public static readonly GamemodeManager Gamemodes = new GamemodeManager();

		public static readonly PropertyManager Properties = new PropertyManager();

		public static readonly FrameCounter FpsCounter = new FrameCounter();

		public static readonly ToastManager Toasts = new ToastManager();

		public static readonly Logger Logger = new Logger();

		public static GuiController GuiController;

		public static readonly Regex BlacklistedTagsPattern = new Regex("<\\/?(size|material|quad)[^>]*>", RegexOptions.IgnoreCase);

		public static bool WasQuitRequested = false;

		private static bool IsFirstInit = true;

		private static bool HasJoinedRoom = false;

		public static string SystemLanguage => CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

		private void Start()
		{
			if (ResourceLoader.TryGetAsset<Texture2D>("Custom/Textures/hud.png", out var value))
			{
				GameObject gameObject = GameObject.Find("Background");
				if (gameObject != null)
				{
					Material material = gameObject.GetComponent<UISprite>().material;
					material.mainTextureScale = value.GetScaleVector(2048, 2048);
					material.mainTexture = value;
				}
			}
			StartCoroutine(CoWaitAndSetParticleTexture());
			GuiController = base.gameObject.AddComponent<GuiController>();
			base.gameObject.AddComponent<MicEF>();
			if (IsFirstInit)
			{
				IsFirstInit = false;
				NetworkChecker.Init();
				SkinChecker.Init();
				FengGameManagerMKII.NameField = PlayerPrefs.GetString("name", string.Empty);
				if (FengGameManagerMKII.NameField.StripNGUI().Length < 1)
				{
					FengGameManagerMKII.NameField = LoginFengKAI.Player.Name;
				}
				LoginFengKAI.Player.Guild = PlayerPrefs.GetString("guildname", string.Empty);
				Commands.Load();
				Gamemodes.Load();
				Properties.Load();
				DiscordRPC.StartTime = GameHelper.CurrentTimeMillis();
				DiscordRPC.Initialize();
				StartCoroutine(CoCheckForUpdate());
			}
		}

		private IEnumerator CoCheckForUpdate()
		{
			Logger.Info("Checking for update...");
			Logger.Info("Installed: " + Build);
			using (WWW www = new WWW("http://aottg.winnpixie.xyz/clients/guardian/version.txt?t=" + GameHelper.CurrentTimeMillis()))
			{
				yield return www;
				if (www.error != null)
				{
					Logger.Error(www.error);
					Logger.Error("\nIf errors persist, PLEASE contact me!");
					Logger.Info("Discord:");
					Logger.Info("\t- " + "https://discord.gg/JGzTdWm".AsColor("0099FF"));
					try
					{
						GameObject.Find("VERSION").GetComponent<UILabel>().text = "[FF0000]COULD NOT VERIFY BUILD.[-] If this persists, PLEASE contact me @ [0099FF]https://discord.gg/JGzTdWm[-]!";
						yield break;
					}
					catch
					{
						yield break;
					}
				}
				string text = "";
				string[] array = www.text.Split('\n');
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[1] { '=' }, 2);
					if (array2[0].Equals("MOD"))
					{
						text = array2[1].Trim();
					}
				}
				Logger.Info("Latest: " + text);
				if (!text.Equals(Build))
				{
					Toasts.Add(new Toast("SYSTEM", "Your copy of Guardian is OUT OF DATE, please update!", 20f));
					Logger.Info("Your copy of Guardian is " + "OUT OF DATE".AsBold().AsItalic().AsColor("FF0000") + "!");
					Logger.Info("If you don't have the launcher, download it here:");
					Logger.Info("\t- " + "https://cb.run/GuardianAoT".AsColor("0099FF"));
					try
					{
						GameObject.Find("VERSION").GetComponent<UILabel>().text = "[FF0000]OUT OF DATE![-] Please update from the launcher @ [0099FF]https://cb.run/GuardianAoT[-]!";
						yield break;
					}
					catch
					{
						yield break;
					}
				}
			}
		}

		private IEnumerator CoWaitAndSetParticleTexture()
		{
			ResourceLoader.TryGetAsset<Texture2D>("Custom/Textures/dust.png", out var dustTexture);
			ResourceLoader.TryGetAsset<Texture2D>("Custom/Textures/blood.png", out var bloodTexture);
			ResourceLoader.TryGetAsset<Texture2D>("Custom/Textures/gun_smoke.png", out var gunSmokeTexture);
			while (true)
			{
				ParticleSystem[] array = Object.FindObjectsOfType<ParticleSystem>();
				foreach (ParticleSystem particleSystem in array)
				{
					if (dustTexture != null && (particleSystem.name.Contains("smoke") || particleSystem.name.StartsWith("boom") || particleSystem.name.StartsWith("bite") || particleSystem.name.StartsWith("Particle System 2") || particleSystem.name.StartsWith("Particle System 3") || particleSystem.name.StartsWith("Particle System 4") || particleSystem.name.Contains("colossal_steam") || particleSystem.name.Contains("FXtitan") || particleSystem.name.StartsWith("dust")) && !particleSystem.name.StartsWith("3dmg"))
					{
						particleSystem.renderer.material.mainTexture = dustTexture;
					}
					if (bloodTexture != null && particleSystem.name.Contains("blood"))
					{
						particleSystem.renderer.material.mainTexture = bloodTexture;
					}
					if (gunSmokeTexture != null && particleSystem.name.Contains("shotGun"))
					{
						particleSystem.renderer.material.mainTexture = gunSmokeTexture;
					}
				}
				yield return new WaitForSeconds(0.1f);
			}
		}

		public void ApplyCustomRenderSettings()
		{
			Properties.DrawDistance.OnValueChanged();
			Properties.Blur.OnValueChanged();
			Properties.Fog.OnValueChanged();
			Properties.FogColor.OnValueChanged();
			Properties.FogDensity.OnValueChanged();
			Properties.SoftShadows.OnValueChanged();
		}

		private void Update()
		{
			if (PhotonNetwork.isMasterClient)
			{
				Gamemodes.CurrentMode.OnUpdate();
			}
			DiscordRPC.RunCallbacks();
			FpsCounter.UpdateCounter();
		}

		private void OnLevelWasLoaded(int level)
		{
			ApplyCustomRenderSettings();
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer || PhotonNetwork.offlineMode)
			{
				string text;
				switch (IN_GAME_MAIN_CAMERA.Difficulty)
				{
				case -1:
					text = "Training";
					break;
				case 0:
					text = "Normal";
					break;
				case 1:
					text = "Hard";
					break;
				case 2:
					text = "Abnormal";
					break;
				default:
					text = "Unknown";
					break;
				}
				string text2 = text;
				Activity presence = default(Activity);
				presence.Details = "Playing offline.";
				presence.State = FengGameManagerMKII.Level.Name + " / " + text2;
				DiscordRPC.SetPresence(presence);
			}
			if (PhotonNetwork.isMasterClient)
			{
				Gamemodes.CurrentMode.OnReset();
			}
			if (!HasJoinedRoom)
			{
				HasJoinedRoom = true;
				string text3 = Properties.JoinMessage.Value.NGUIToUnity();
				if (text3.StripNGUI().Length < 1)
				{
					text3 = Properties.JoinMessage.Value;
				}
				if (text3.Length >= 1)
				{
					Commands.Find("say").Execute(InRoomChat.Instance, text3.Split(' '));
				}
			}
		}

		private void OnPhotonPlayerConnected(PhotonPlayer player)
		{
			if (PhotonNetwork.isMasterClient)
			{
				Gamemodes.CurrentMode.OnPlayerJoin(player);
			}
			Toasts.Add(new Toast("PLAYER CONNECTED".AsColor("00FF00"), $"({player.Id}) {player.Username.NGUIToUnity()} connected.", 15f));
			Logger.Info($"({player.Id}) " + player.Username.NGUIToUnity() + " connected.".AsColor("00FF00"));
		}

		private void OnPhotonPlayerDisconnected(PhotonPlayer player)
		{
			if (PhotonNetwork.isMasterClient)
			{
				Gamemodes.CurrentMode.OnPlayerLeave(player);
			}
			Toasts.Add(new Toast("PLAYER DISCONNECTED".AsColor("FF0000"), $"({player.Id}) {player.Username.NGUIToUnity()} disconnected.", 15f));
			Logger.Info($"({player.Id}) " + player.Username.NGUIToUnity() + " disconnected.".AsColor("FF0000"));
		}

		private void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
		{
			NetworkChecker.OnPlayerPropertyModification(playerAndUpdatedProps);
			ModDetector.OnPlayerPropertyModification(playerAndUpdatedProps);
		}

		private void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
		{
			NetworkChecker.OnRoomPropertyModification(propertiesThatChanged);
			PhotonPlayer photonPlayer = null;
			if (propertiesThatChanged.ContainsKey("sender") && propertiesThatChanged["sender"] is PhotonPlayer photonPlayer2)
			{
				photonPlayer = photonPlayer2;
			}
			if (photonPlayer != null && !photonPlayer.isMasterClient)
			{
				return;
			}
			if (propertiesThatChanged.ContainsKey("Map") && propertiesThatChanged["Map"] is string text)
			{
				LevelInfo info = LevelInfo.GetInfo(text);
				if (info != null)
				{
					FengGameManagerMKII.Level = info;
				}
			}
			if (propertiesThatChanged.ContainsKey("Lighting") && propertiesThatChanged["Lighting"] is string input && GExtensions.TryParseEnum<DayLight>(input, out var value))
			{
				Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetLighting(value);
			}
		}

		private void OnJoinedLobby()
		{
			PhotonNetwork.playerName = Properties.PhotonUserId.Value;
		}

		private void OnJoinedRoom()
		{
			HasJoinedRoom = false;
			int[] array = new int[255];
			for (int i = 0; i < 255; i++)
			{
				array[i] = i + 1;
			}
			PhotonNetwork.SetReceivingEnabled(array, null);
			PhotonNetwork.SetSendingEnabled(array, null);
			PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { CustomPropertyName, Build } });
			StartCoroutine(CoUpdateMyPing());
			string[] array2 = PhotonNetwork.room.name.Split('`');
			if (array2.Length >= 7)
			{
				Activity presence = default(Activity);
				presence.Details = "Playing in " + ((array2[5].Length < 1) ? string.Empty : "[PWD]") + " " + array2[0].StripNGUI();
				presence.State = "(" + NetworkHelper.GetRegionCode().ToUpper() + ") " + array2[1] + " / " + array2[2].ToUpper();
				DiscordRPC.SetPresence(presence);
			}
		}

		private IEnumerator CoUpdateMyPing()
		{
			while (PhotonNetwork.inRoom)
			{
				int ping = PhotonNetwork.player.Ping;
				int ping2 = PhotonNetwork.GetPing();
				if (ping2 != ping)
				{
					PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Ping", ping2 } });
				}
				yield return new WaitForSeconds(3f);
			}
		}

		private void OnLeftRoom()
		{
			Gamemodes.CurrentMode.CleanUp();
			PhotonNetwork.SetPlayerCustomProperties(null);
			Activity presence = default(Activity);
			presence.Details = "Idle...";
			DiscordRPC.SetPresence(presence);
		}

		private void OnConnectionFail(DisconnectCause cause)
		{
			Logger.Warn($"OnConnectionFail ({cause})");
		}

		private void OnPhotonRoomJoinFailed(object[] codeAndMsg)
		{
			Logger.Error("OnPhotonRoomJoinFailed");
			foreach (object arg in codeAndMsg)
			{
				Logger.Error($" - {arg}");
			}
		}

		private void OnPhotonCreateRoomFailed(object[] codeAndMsg)
		{
			Logger.Error("OnPhotonCreateRoomFailed");
			foreach (object arg in codeAndMsg)
			{
				Logger.Error($" - {arg}");
			}
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (hasFocus && IN_GAME_MAIN_CAMERA.Gametype != GameType.Stop)
			{
				if (Minimap.Instance != null)
				{
					Minimap.WaitAndTryRecaptureInstance(0.5f);
				}
				if (IN_GAME_MAIN_CAMERA.CameraMode == CameraType.TPS)
				{
					Screen.lockCursor = false;
					Screen.lockCursor = true;
				}
			}
		}

		private void OnApplicationQuit()
		{
			WasQuitRequested = true;
			Properties.Save();
			DiscordRPC.Dispose();
		}
	}
}
