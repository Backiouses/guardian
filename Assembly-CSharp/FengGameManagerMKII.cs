using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Anarchy.Custom.Interfaces;
using Anarchy.Custom.Level;
using ExitGames.Client.Photon;
using Guardian;
using Guardian.AntiAbuse;
using Guardian.AntiAbuse.Validators;
using Guardian.Networking;
using Guardian.Utilities;
using Photon;
using UnityEngine;

public class FengGameManagerMKII : Photon.MonoBehaviour, IGameManager
{
	public static LevelInfo Level;

	public static ExitGames.Client.Photon.Hashtable BanHash;

	public static ExitGames.Client.Photon.Hashtable ImATitan;

	public static ExitGames.Client.Photon.Hashtable[] LinkHash;

	public static object[] Settings;

	public static string[] S = new string[27]
	{
		"verified343", "hair", "character_eye", "glass", "character_face", "character_head", "character_hand", "character_body", "character_arm", "character_leg",
		"character_chest", "character_cape", "character_brand", "character_3dmg", "r", "character_blade_l", "character_3dmg_gas_r", "character_blade_r", "3dmg_smoke", "HORSE",
		"hair", "body_001", "Cube", "Plane_031", "mikasa_asset", "character_cap_", "character_gun"
	};

	public static AssetBundle RCAssets;

	public static bool IsAssetLoaded;

	public static InputManagerRC InputRC;

	public static string CurrentScript;

	public static Material SkyMaterial;

	public static string OldScript;

	public static string CurrentLevel;

	public static string NameField;

	public static string UsernameField;

	public static string PasswordField;

	public static string PrivateServerField;

	public static ExitGames.Client.Photon.Hashtable IntVariables;

	public static ExitGames.Client.Photon.Hashtable HeroHash;

	public static ExitGames.Client.Photon.Hashtable BoolVariables;

	public static ExitGames.Client.Photon.Hashtable StringVariables;

	public static ExitGames.Client.Photon.Hashtable FloatVariables;

	public static ExitGames.Client.Photon.Hashtable GlobalVariables;

	public static ExitGames.Client.Photon.Hashtable RCRegions;

	public static ExitGames.Client.Photon.Hashtable RCEvents;

	public static ExitGames.Client.Photon.Hashtable RCVariableNames;

	public static ExitGames.Client.Photon.Hashtable PlayerVariables;

	public static ExitGames.Client.Photon.Hashtable TitanVariables;

	public static ExitGames.Client.Photon.Hashtable RCRegionTriggers;

	public static bool LogicLoaded;

	public static bool CustomLevelLoaded;

	public static string OldScriptLogic;

	public static string CurrentScriptLogic;

	public static List<int> IgnoreList;

	public static bool NoRestart;

	public static bool MasterRC;

	public static bool HasLogged;

	public static FengGameManagerMKII Instance;

	public static Dictionary<string, GameObject> CachedPrefabs;

	public static bool OnPrivateServer;

	public static string PrivateServerAuthPass;

	public FengCustomInputs inputManager;

	public int difficulty;

	private GameObject ui;

	public bool needChooseSide;

	public bool justSuicide;

	private ArrayList chatContent;

	private string myLastHero;

	private string myLastRespawnTag = "playerRespawn";

	public float myRespawnTime;

	public int titanScore;

	public int humanScore;

	public int PVPtitanScore;

	public int PVPhumanScore;

	private int PVPtitanScoreMax = 200;

	private int PVPhumanScoreMax = 200;

	private bool isWinning;

	private bool isLosing;

	private int teamWinner;

	private int[] teamScores;

	private float gameEndCD;

	private float gameEndTotalCDtime = 9f;

	public int wave = 1;

	private int highestWave = 1;

	public int time = 600;

	private float timeElapse;

	public float roundTime;

	private float _timeTotalServer;

	private float maxSpeed;

	private float currentSpeed;

	private bool startRacing;

	private bool endRacing;

	public GameObject checkpoint;

	private ArrayList racingResult;

	private bool gameTimesUp;

	public IN_GAME_MAIN_CAMERA mainCamera;

	public bool gameStart;

	private ArrayList heroes;

	private ArrayList eT;

	private ArrayList hooks;

	private ArrayList titans;

	private ArrayList fT;

	private ArrayList cT;

	private string localRacingResult;

	private int single_kills;

	private int single_maxDamage;

	private int single_totalDamage;

	private ArrayList killInfoGO = new ArrayList();

	public List<Vector3> playerSpawnsC;

	public List<Vector3> playerSpawnsM;

	public List<Vector3> titanSpawns;

	public List<PhotonPlayer> otherUsers;

	public List<string[]> levelCache;

	public List<TitanSpawner> titanSpawners;

	public int cyanKills;

	public int magentaKills;

	public Vector2 scroll;

	public Vector2 scroll2;

	public GameObject selectedObj;

	public bool isSpawning;

	public float updateTime;

	public Vector3 racingSpawnPoint;

	public bool racingSpawnPointSet;

	public List<GameObject> racingDoors;

	public List<float> restartCount;

	public Dictionary<int, CannonValues> allowedToCannon;

	public bool restartingMC;

	public bool restartingBomb;

	public bool restartingTitan;

	public bool restartingEren;

	public bool restartingHorse;

	public bool isRestarting;

	public List<GameObject> groundList;

	public Dictionary<string, Texture2D> assetCacheTextures;

	public bool isUnloading;

	public string playerList;

	public bool isRecompiling;

	public List<GameObject> spectateSprites;

	public Dictionary<string, int[]> PreservedPlayerKDR;

	public float qualitySlider;

	public float mouseSlider;

	public float distanceSlider;

	public float transparencySlider;

	public float retryTime;

	public bool isFirstLoad = true;

	public float pauseWaitTime;

	public List<HERO> Heroes = new List<HERO>();

	public List<TITAN_EREN> Erens = new List<TITAN_EREN>();

	public List<GameObject> Players = new List<GameObject>();

	public List<Bullet> Hooks = new List<Bullet>();

	public List<TITAN> Titans = new List<TITAN>();

	public List<FEMALE_TITAN> Annies = new List<FEMALE_TITAN>();

	public List<GameObject> AllTitans = new List<GameObject>();

	public List<COLOSSAL_TITAN> Colossals = new List<COLOSSAL_TITAN>();

	private Dictionary<int, VoteKick> VoteKicks = new Dictionary<int, VoteKick>();

	private long RoundStartTime;

	private long WaveStartTime;

	public static string ApplicationId => NetworkHelper.App.Id;

	public float timeTotalServer
	{
		get
		{
			if (!PhotonNetwork.isMasterClient || !GuardianClient.Properties.InfiniteRoom.Value)
			{
				return _timeTotalServer;
			}
			return time - MathHelper.Abs(time);
		}
		set
		{
			_timeTotalServer = value;
		}
	}

	[RPC]
	public void TheirPing(int ping, PhotonMessageInfo info)
	{
		info.sender.Ping = ping;
	}

	public void AddHero(HERO hero)
	{
		heroes.Add(hero);
		Heroes.Add(hero);
		Players.Add(hero.gameObject);
	}

	public void RemoveHero(HERO hero)
	{
		heroes.Remove(hero);
		Heroes.Remove(hero);
		Players.Remove(hero.gameObject);
	}

	public void AddHook(Bullet hook)
	{
		hooks.Add(hook);
		Hooks.Add(hook);
	}

	public void RemoveHook(Bullet hook)
	{
		hooks.Remove(hook);
		Hooks.Remove(hook);
	}

	public void AddEren(TITAN_EREN eren)
	{
		eT.Add(eren);
		Erens.Add(eren);
		Players.Add(eren.gameObject);
	}

	public void RemoveEren(TITAN_EREN eren)
	{
		eT.Remove(eren);
		Erens.Remove(eren);
		Players.Remove(eren.gameObject);
	}

	public void AddTitan(TITAN titan)
	{
		titans.Add(titan);
		Titans.Add(titan);
		AllTitans.Add(titan.gameObject);
	}

	public void RemoveTitan(TITAN titan)
	{
		titans.Remove(titan);
		Titans.Remove(titan);
		AllTitans.Remove(titan.gameObject);
	}

	public void AddAnnie(FEMALE_TITAN annie)
	{
		fT.Add(annie);
		Annies.Add(annie);
		AllTitans.Add(annie.gameObject);
	}

	public void RemoveAnnie(FEMALE_TITAN annie)
	{
		fT.Remove(annie);
		Annies.Remove(annie);
		AllTitans.Remove(annie.gameObject);
	}

	public void AddColossal(COLOSSAL_TITAN colossal)
	{
		cT.Add(colossal);
		Colossals.Add(colossal);
	}

	public void RemoveColossal(COLOSSAL_TITAN colossal)
	{
		cT.Remove(colossal);
		Colossals.Remove(colossal);
	}

	public void SetCamera(IN_GAME_MAIN_CAMERA camera)
	{
		mainCamera = camera;
	}

	private void LateUpdate()
	{
		if (!gameStart)
		{
			return;
		}
		foreach (HERO hero in heroes)
		{
			hero.lateUpdate2();
		}
		foreach (TITAN_EREN item in eT)
		{
			item.lateUpdate();
		}
		foreach (TITAN titan in titans)
		{
			titan.lateUpdate2();
		}
		foreach (FEMALE_TITAN item2 in fT)
		{
			item2.lateUpdate2();
		}
		Core2();
	}

	private void Update()
	{
		if (IN_GAME_MAIN_CAMERA.Gametype != 0)
		{
			if (PhotonNetwork.connected)
			{
				SetTextNetworkStatus(PhotonNetwork.connectionStateDetailed.ToString());
				AddTextNetworkStatus(" Ping: " + PhotonNetwork.GetPing() + "ms");
			}
			else
			{
				SetTextNetworkStatus("Disconnected");
			}
		}
		else
		{
			SetTextNetworkStatus("Singleplayer");
		}
		if (!gameStart)
		{
			return;
		}
		foreach (HERO hero in heroes)
		{
			hero.update2();
		}
		foreach (Bullet hook in hooks)
		{
			hook.update();
		}
		if (mainCamera != null)
		{
			mainCamera.SnapShotUpdate();
		}
		foreach (TITAN_EREN item in eT)
		{
			item.update();
		}
		foreach (TITAN titan in titans)
		{
			titan.update();
		}
		foreach (FEMALE_TITAN item2 in fT)
		{
			item2.update();
		}
		foreach (COLOSSAL_TITAN item3 in cT)
		{
			item3.update();
		}
		if (mainCamera != null)
		{
			mainCamera.update2();
		}
	}

	public void SpawnPlayer(string id, string tag = "playerRespawn")
	{
		if (IN_GAME_MAIN_CAMERA.Gamemode == GameMode.PvPCapture)
		{
			SpawnPlayerAt2(id, checkpoint);
			return;
		}
		myLastRespawnTag = tag;
		GameObject[] array = GameObject.FindGameObjectsWithTag(tag);
		GameObject pos = array[UnityEngine.Random.Range(0, array.Length)];
		SpawnPlayerAt2(id, pos);
	}

	public void NOTSpawnPlayer(string id)
	{
		myLastHero = id.ToUpper();
		ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
		{
			{
				PhotonPlayerProperty.IsDead,
				true
			},
			{
				PhotonPlayerProperty.IsTitan,
				1
			}
		};
		PhotonNetwork.player.SetCustomProperties(propertiesToSet);
		Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode == CameraType.TPS;
		Screen.showCursor = false;
		SetTextCenter("The game has started for 60 seconds.\n please wait for next round.\n Click Right Mouse Key to Enter or Exit the Spectator Mode.");
		mainCamera.enabled = true;
		mainCamera.SetMainObject(null);
		mainCamera.SetSpectorMode(val: true);
		mainCamera.gameOver = true;
	}

	public void NOTSpawnNonAITitan(string id)
	{
		myLastHero = id.ToUpper();
		ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
		{
			{
				PhotonPlayerProperty.IsDead,
				true
			},
			{
				PhotonPlayerProperty.IsTitan,
				2
			}
		};
		PhotonNetwork.player.SetCustomProperties(propertiesToSet);
		Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode == CameraType.TPS;
		Screen.showCursor = true;
		SetTextCenter("The game has started for 60 seconds.\n please wait for next round.\n Click Right Mouse Key to Enter or Exit the Spectator Mode.");
		mainCamera.enabled = true;
		mainCamera.SetMainObject(null);
		mainCamera.SetSpectorMode(val: true);
		mainCamera.gameOver = true;
	}

	public void SpawnNonAITitan(string id, string tag = "titanRespawn")
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(tag);
		GameObject gameObject = array[UnityEngine.Random.Range(0, array.Length)];
		myLastHero = id.ToUpper();
		GameObject gameObject2 = ((IN_GAME_MAIN_CAMERA.Gamemode != GameMode.PvPCapture) ? PhotonNetwork.Instantiate("TITAN_VER3.1", gameObject.transform.position, gameObject.transform.rotation, 0) : PhotonNetwork.Instantiate("TITAN_VER3.1", checkpoint.transform.position + new Vector3(UnityEngine.Random.Range(-20, 20), 2f, UnityEngine.Random.Range(-20, 20)), checkpoint.transform.rotation, 0));
		mainCamera.SetMainObjectTitan(gameObject2);
		gameObject2.GetComponent<TITAN>().nonAI = true;
		gameObject2.GetComponent<TITAN>().speed = 30f;
		gameObject2.GetComponent<TITAN_CONTROLLER>().enabled = true;
		if (id == "RANDOM" && UnityEngine.Random.Range(0, 100) < 7)
		{
			gameObject2.GetComponent<TITAN>().setAbnormalType2(TitanClass.Crawler, forceCrawler: true);
		}
		mainCamera.enabled = true;
		GameObject obj = GameObject.Find("MainCamera");
		obj.GetComponent<SpectatorMovement>().disable = true;
		obj.GetComponent<MouseLook>().disable = true;
		mainCamera.gameOver = false;
		ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
		{
			{
				PhotonPlayerProperty.IsDead,
				false
			},
			{
				PhotonPlayerProperty.IsTitan,
				2
			}
		};
		PhotonNetwork.player.SetCustomProperties(propertiesToSet);
		Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode == CameraType.TPS;
		Screen.showCursor = true;
		SetTextCenter(string.Empty);
	}

	public void OnCreatedRoom()
	{
		racingResult = new ArrayList();
		teamScores = new int[2];
		UnityEngine.MonoBehaviour.print("OnCreatedRoom");
	}

	public void OnDisconnectedFromPhoton()
	{
		UnityEngine.MonoBehaviour.print("OnDisconnectedFromPhoton");
		Screen.lockCursor = false;
		Screen.showCursor = true;
	}

	public void OnConnectionFail(DisconnectCause cause)
	{
		UnityEngine.MonoBehaviour.print("OnConnectionFail: " + cause);
		Screen.lockCursor = false;
		Screen.showCursor = true;
		IN_GAME_MAIN_CAMERA.Gametype = GameType.Stop;
		gameStart = false;
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], state: false);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], state: false);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], state: false);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], state: false);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[4], state: true);
		GameObject.Find("LabelDisconnectInfo").GetComponent<UILabel>().text = "OnConnectionFail: " + cause;
	}

	[RPC]
	private void RequireStatus(PhotonMessageInfo info = null)
	{
		if (FGMChecker.IsStatusRequestValid(info))
		{
			base.photonView.RPC("refreshStatus", PhotonTargets.Others, humanScore, titanScore, wave, highestWave, roundTime, timeTotalServer, startRacing, endRacing);
			base.photonView.RPC("refreshPVPStatus", PhotonTargets.Others, PVPhumanScore, PVPtitanScore);
			base.photonView.RPC("refreshPVPStatus_AHSS", PhotonTargets.Others, teamScores);
		}
	}

	[RPC]
	private void refreshStatus(int score1, int score2, int theWave, int theHighestWave, float time1, float time2, bool startRacin, bool shouldEndRacing, PhotonMessageInfo info)
	{
		if (FGMChecker.IsStatusRefreshValid(info))
		{
			humanScore = score1;
			titanScore = score2;
			wave = theWave;
			highestWave = theHighestWave;
			roundTime = time1;
			timeTotalServer = time2;
			startRacing = startRacin;
			endRacing = shouldEndRacing;
			if (startRacing && (bool)GameObject.Find("door"))
			{
				GameObject.Find("door").SetActive(value: false);
			}
		}
	}

	[RPC]
	private void refreshPVPStatus(int score1, int score2, PhotonMessageInfo info)
	{
		if (FGMChecker.IsPVPStatusRefreshValid(info))
		{
			PVPhumanScore = score1;
			PVPtitanScore = score2;
		}
	}

	[RPC]
	private void refreshPVPStatus_AHSS(int[] score1, PhotonMessageInfo info)
	{
		if (FGMChecker.IsAHSSStatusRefreshValid(info))
		{
			teamScores = score1;
		}
	}

	[RPC]
	public void someOneIsDead(int id = -1)
	{
		switch (IN_GAME_MAIN_CAMERA.Gamemode)
		{
		case GameMode.PvPCapture:
			if (id != 0)
			{
				PVPtitanScore += 2;
			}
			CheckPvPPoints();
			if (IN_GAME_MAIN_CAMERA.Gametype != 0 && PhotonNetwork.isMasterClient)
			{
				base.photonView.RPC("refreshPVPStatus", PhotonTargets.Others, PVPhumanScore, PVPtitanScore);
			}
			break;
		case GameMode.Endless:
			titanScore++;
			break;
		case GameMode.KillTitans:
		case GameMode.Survival:
		case GameMode.Colossal:
		case GameMode.Trost:
			if (AreAllPlayersDead())
			{
				FinishGame(isLoss: true);
			}
			break;
		case GameMode.TeamDeathmatch:
			if (RCSettings.PvPMode == 0 && RCSettings.BombMode == 0)
			{
				if (AreAllPlayersDead())
				{
					FinishGame(isLoss: true);
					teamWinner = 0;
				}
				if (IsTeamDead(1))
				{
					teamWinner = 2;
					FinishGame();
				}
				if (IsTeamDead(2))
				{
					teamWinner = 1;
					FinishGame();
				}
			}
			break;
		case GameMode.CageFight:
		case GameMode.Tutorial:
		case GameMode.Racing:
			break;
		}
	}

	public void CheckPvPPoints()
	{
		if (PVPtitanScore >= PVPtitanScoreMax)
		{
			PVPtitanScore = PVPtitanScoreMax;
			FinishGame(isLoss: true);
		}
		else if (PVPhumanScore >= PVPhumanScoreMax)
		{
			PVPhumanScore = PVPhumanScoreMax;
			FinishGame();
		}
	}

	public void FinishRaceMulti()
	{
		float num = roundTime - 20f;
		if (PhotonNetwork.isMasterClient)
		{
			getRacingResult(LoginFengKAI.Player.Name, num);
		}
		else
		{
			base.photonView.RPC("getRacingResult", PhotonTargets.MasterClient, LoginFengKAI.Player.Name, num);
		}
		FinishGame();
	}

	[RPC]
	private void getRacingResult(string player, float time)
	{
		RacingResult racingResult = new RacingResult();
		racingResult.name = player;
		racingResult.time = time;
		this.racingResult.Add(racingResult);
		RefreshRacingResult();
	}

	[RPC]
	private void netRefreshRacingResult(string result)
	{
		localRacingResult = result;
	}

	public GameObject SpawnTitanRandom(string place, int rate, bool punk = false)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(place);
		int num = UnityEngine.Random.Range(0, array.Length);
		GameObject gameObject = array[num];
		while (array[num] == null)
		{
			num = UnityEngine.Random.Range(0, array.Length);
			gameObject = array[num];
		}
		array[num] = null;
		return SpawnTitan(rate, gameObject.transform.position, gameObject.transform.rotation, punk);
	}

	public GameObject SpawnTitanRaw(Vector3 position, Quaternion rotation)
	{
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
		{
			return (GameObject)UnityEngine.Object.Instantiate(Resources.Load("TITAN_VER3.1"), position, rotation);
		}
		return PhotonNetwork.Instantiate("TITAN_VER3.1", position, rotation, 0);
	}

	public GameObject SpawnTitan(int rate, Vector3 position, Quaternion rotation, bool punk = false)
	{
		GameObject gameObject = SpawnTitanRaw(position, rotation);
		if (punk)
		{
			gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Punk, forceCrawler: false);
		}
		else if (UnityEngine.Random.Range(0, 100) < rate)
		{
			if (IN_GAME_MAIN_CAMERA.Difficulty == 2)
			{
				if (UnityEngine.Random.Range(0f, 1f) < 0.7f || Level.NoCrawlers)
				{
					gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Jumper, forceCrawler: false);
				}
				else
				{
					gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Crawler, forceCrawler: false);
				}
			}
		}
		else if (IN_GAME_MAIN_CAMERA.Difficulty == 2)
		{
			if (UnityEngine.Random.Range(0f, 1f) < 0.7f || Level.NoCrawlers)
			{
				gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Jumper, forceCrawler: false);
			}
			else
			{
				gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Crawler, forceCrawler: false);
			}
		}
		else if (UnityEngine.Random.Range(0, 100) < rate)
		{
			if (UnityEngine.Random.Range(0f, 1f) < 0.8f || Level.NoCrawlers)
			{
				gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Aberrant, forceCrawler: false);
			}
			else
			{
				gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Crawler, forceCrawler: false);
			}
		}
		else if (UnityEngine.Random.Range(0f, 1f) < 0.8f || Level.NoCrawlers)
		{
			gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Jumper, forceCrawler: false);
		}
		else
		{
			gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Crawler, forceCrawler: false);
		}
		((IN_GAME_MAIN_CAMERA.Gametype != 0) ? PhotonNetwork.Instantiate("FX/FXtitanSpawn", gameObject.transform.position, Quaternion.Euler(-90f, 0f, 0f), 0) : ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/FXtitanSpawn"), gameObject.transform.position, Quaternion.Euler(-90f, 0f, 0f)))).transform.localScale = gameObject.transform.localScale;
		return gameObject;
	}

	[RPC]
	public void titanGetKill(PhotonPlayer player, int damage, string name, PhotonMessageInfo info = null)
	{
		if (FGMChecker.IsTitanKillValid(info))
		{
			damage = Mathf.Max(10, damage);
			base.photonView.RPC("netShowDamage", player, damage);
			base.photonView.RPC("oneTitanDown", PhotonTargets.MasterClient, name, false);
			SendKillInfo(isKillerTitan: false, (string)player.customProperties[PhotonPlayerProperty.Name], isVictimTitan: true, name, damage);
			UpdatePlayerKillInfo(damage, player);
		}
	}

	public void UpdatePlayerKillInfo(int damage, PhotonPlayer player = null)
	{
		if (player != null)
		{
			player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
			{
				{
					PhotonPlayerProperty.Kills,
					(int)player.customProperties[PhotonPlayerProperty.Kills] + 1
				},
				{
					PhotonPlayerProperty.MaxDamage,
					Mathf.Max(damage, (int)player.customProperties[PhotonPlayerProperty.MaxDamage])
				},
				{
					PhotonPlayerProperty.TotalDamage,
					(int)player.customProperties[PhotonPlayerProperty.TotalDamage] + damage
				}
			});
		}
		else
		{
			single_kills++;
			single_maxDamage = Mathf.Max(damage, single_maxDamage);
			single_totalDamage += damage;
		}
	}

	[RPC]
	public void oneTitanDown(string titanName, bool onPlayerLeave)
	{
		if (IN_GAME_MAIN_CAMERA.Gametype != 0 && !PhotonNetwork.isMasterClient)
		{
			return;
		}
		switch (IN_GAME_MAIN_CAMERA.Gamemode)
		{
		case GameMode.PvPCapture:
			switch (titanName)
			{
			case "Titan":
				PVPhumanScore++;
				break;
			case "Aberrant":
				PVPhumanScore += 2;
				break;
			case "Jumper":
				PVPhumanScore += 3;
				break;
			case "Crawler":
				PVPhumanScore += 4;
				break;
			case "Female Titan":
				PVPhumanScore += 10;
				break;
			default:
				if (titanName.Length != 0)
				{
					PVPhumanScore += 3;
				}
				break;
			}
			CheckPvPPoints();
			base.photonView.RPC("refreshPVPStatus", PhotonTargets.Others, PVPhumanScore, PVPtitanScore);
			break;
		case GameMode.KillTitans:
			if (AreAllTitansDead())
			{
				FinishGame();
			}
			break;
		case GameMode.Survival:
		{
			if (!AreAllTitansDead())
			{
				break;
			}
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
			{
				long num = GameHelper.CurrentTimeMillis();
				if (GuardianClient.Properties.AnnounceWaveTime.Value)
				{
					GameHelper.Broadcast($"This wave lasted for <b>{(float)(num - WaveStartTime) / 1000f}</b> second(s)!");
				}
				WaveStartTime = num;
			}
			if (++wave > RCSettings.GetMaxWave())
			{
				FinishGame();
				break;
			}
			if ((Level.RespawnMode == RespawnMode.NewRound || (Level.Name.StartsWith("Custom") && RCSettings.GameType == 1)) && IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
			{
				PhotonPlayer[] array = PhotonNetwork.playerList;
				foreach (PhotonPlayer photonPlayer in array)
				{
					if (GExtensions.AsInt(photonPlayer.customProperties[PhotonPlayerProperty.IsTitan]) != 2)
					{
						base.photonView.RPC("respawnHeroInNewRound", photonPlayer);
					}
				}
			}
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
			{
				GameHelper.Broadcast("Wave ".AsColor("AAFF00") + $"{wave} / {RCSettings.GetMaxWave()}");
			}
			if (wave > highestWave)
			{
				highestWave = wave;
			}
			if (PhotonNetwork.isMasterClient)
			{
				RequireStatus();
			}
			int abnormal2 = 90;
			if (difficulty == 1)
			{
				abnormal2 = 70;
			}
			if (!Level.Punks || wave % 5 != 0)
			{
				SpawnTitanCustom("titanRespawn", abnormal2, wave + 2, punk: false);
			}
			else
			{
				SpawnTitanCustom("titanRespawn", abnormal2, wave / 5, punk: true);
			}
			break;
		}
		case GameMode.Endless:
			if (!onPlayerLeave)
			{
				humanScore++;
				int abnormal = 90;
				if (difficulty == 1)
				{
					abnormal = 70;
				}
				SpawnTitanCustom("titanRespawn", abnormal, 1, punk: false);
			}
			break;
		}
	}

	[RPC]
	private void respawnHeroInNewRound(PhotonMessageInfo info)
	{
		if (!info.sender.isMasterClient)
		{
			GuardianClient.Logger.Info($"Non-MC revive from #{info.sender.Id}.");
		}
		if (!needChooseSide && mainCamera.gameOver)
		{
			SpawnPlayer(myLastHero, myLastRespawnTag);
			mainCamera.gameOver = false;
			SetTextCenter(string.Empty);
		}
	}

	private bool AreAllTitansDead()
	{
		foreach (TITAN titan in titans)
		{
			if (!titan.hasDie)
			{
				return false;
			}
		}
		return fT.Count == 0;
	}

	[RPC]
	public void netShowDamage(int speed, PhotonMessageInfo info = null)
	{
		if (FGMChecker.IsNetShowDamageValid(info))
		{
			ShowDamage(speed);
		}
	}

	public void ShowDamage(int speed)
	{
		GameObject.Find("Stylish").GetComponent<StylishComponent>().Style(speed);
		GameObject gameObject = GameObject.Find("LabelScore");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text = speed.ToString();
			gameObject.transform.localScale = Vector3.zero;
			speed = (int)((float)speed * 0.1f);
			speed = Mathf.Max(40, speed);
			speed = Mathf.Min(150, speed);
			iTween.Stop(gameObject);
			iTween.ScaleTo(gameObject, iTween.Hash("x", speed, "y", speed, "z", speed, "easetype", iTween.EaseType.easeOutElastic, "time", 1f));
			iTween.ScaleTo(gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "easetype", iTween.EaseType.easeInBounce, "time", 0.5f, "delay", 2f));
		}
	}

	public void SendKillInfo(bool isKillerTitan, string killer, bool isVictimTitan, string victim, int damage = 0)
	{
		base.photonView.RPC("updateKillInfo", PhotonTargets.All, isKillerTitan, killer, isVictimTitan, victim, damage);
	}

	[RPC]
	private void showChatContent(string content, PhotonMessageInfo info)
	{
		if (FGMChecker.IsChatContentShowValid(info))
		{
			chatContent.Add(content);
			if (chatContent.Count > 10)
			{
				chatContent.RemoveAt(0);
			}
			UILabel component = GameObject.Find("LabelChatContent").GetComponent<UILabel>();
			component.text = string.Empty;
			for (int i = 0; i < chatContent.Count; i++)
			{
				component.text += chatContent[i];
			}
		}
	}

	public void SetTextTopCenter(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoTopCenter");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text = content;
		}
	}

	public void AddTextTopCenter(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoTopCenter");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text += content;
		}
	}

	public void SetTextNetworkStatus(string content)
	{
		GameObject gameObject = GameObject.Find("LabelNetworkStatus");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text = content;
		}
	}

	public void AddTextNetworkStatus(string content)
	{
		GameObject gameObject = GameObject.Find("LabelNetworkStatus");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text += content;
		}
	}

	public void SetTextTopLeft(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoTopLeft");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text = content;
		}
	}

	public void SetTextTopRight(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoTopRight");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text = content;
		}
	}

	public void AddTextTopRight(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoTopRight");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text += content;
		}
	}

	public void SetTextCenter(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoCenter");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text = content;
		}
	}

	public void AddTextCenter(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoCenter");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text += content;
		}
	}

	public void SetTextBottomRight(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoBottomRight");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text = content;
		}
	}

	public void AddTextBottomRight(string content)
	{
		GameObject gameObject = GameObject.Find("LabelInfoBottomRight");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<UILabel>().text += content;
		}
	}

	public static GameObject InstantiateCustomAsset(string key)
	{
		key = key.Substring(8);
		return (GameObject)RCAssets.Load(key);
	}

	private void Awake()
	{
		Instance = this;
		base.gameObject.AddComponent<CustomAnarchyLevel>().GameManager = this;
	}

	private void Start()
	{
		base.gameObject.name = "MultiplayerManager";
		HeroCostume.Init();
		CharacterMaterials.InitData();
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		heroes = new ArrayList();
		eT = new ArrayList();
		titans = new ArrayList();
		fT = new ArrayList();
		cT = new ArrayList();
		hooks = new ArrayList();
		if (NameField == null)
		{
			NameField = "GUEST" + UnityEngine.Random.Range(0, 100000);
		}
		if (PrivateServerField == null)
		{
			PrivateServerField = string.Empty;
		}
		UsernameField = string.Empty;
		PasswordField = string.Empty;
		ResetGameSettings();
		BanHash = new ExitGames.Client.Photon.Hashtable();
		ImATitan = new ExitGames.Client.Photon.Hashtable();
		OldScript = string.Empty;
		CurrentLevel = string.Empty;
		if (CurrentScript == null)
		{
			CurrentScript = string.Empty;
		}
		titanSpawns = new List<Vector3>();
		playerSpawnsC = new List<Vector3>();
		playerSpawnsM = new List<Vector3>();
		otherUsers = new List<PhotonPlayer>();
		levelCache = new List<string[]>();
		titanSpawners = new List<TitanSpawner>();
		restartCount = new List<float>();
		IgnoreList = new List<int>();
		groundList = new List<GameObject>();
		NoRestart = false;
		MasterRC = false;
		isSpawning = false;
		IntVariables = new ExitGames.Client.Photon.Hashtable();
		HeroHash = new ExitGames.Client.Photon.Hashtable();
		BoolVariables = new ExitGames.Client.Photon.Hashtable();
		StringVariables = new ExitGames.Client.Photon.Hashtable();
		FloatVariables = new ExitGames.Client.Photon.Hashtable();
		GlobalVariables = new ExitGames.Client.Photon.Hashtable();
		RCRegions = new ExitGames.Client.Photon.Hashtable();
		RCEvents = new ExitGames.Client.Photon.Hashtable();
		RCVariableNames = new ExitGames.Client.Photon.Hashtable();
		RCRegionTriggers = new ExitGames.Client.Photon.Hashtable();
		PlayerVariables = new ExitGames.Client.Photon.Hashtable();
		TitanVariables = new ExitGames.Client.Photon.Hashtable();
		LogicLoaded = false;
		CustomLevelLoaded = false;
		OldScriptLogic = string.Empty;
		CurrentScriptLogic = string.Empty;
		retryTime = 0f;
		playerList = string.Empty;
		updateTime = 0f;
		LoadConfig();
		List<string> list = new List<string> { "PanelLogin", "LOGIN" };
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = (GameObject)array[i];
			foreach (string item in list)
			{
				if (gameObject.name.Contains(item))
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
		}
		ChangeQuality.SetCurrentQuality();
		base.gameObject.AddComponent<GuardianClient>();
	}

	public float GetRoundTime()
	{
		return roundTime;
	}

	public string GetLevelName()
	{
		return Level.Name;
	}

	public bool IsCustomMapLoaded()
	{
		return CustomLevelLoaded;
	}

	[RPC]
	private void Chat(string message, string sender, PhotonMessageInfo info)
	{
		if (PhotonNetwork.isMasterClient && message.StartsWith("/kick #") && message.Length > 7 && int.TryParse(message.Substring(7), out var result2))
		{
			if (!VoteKicks.TryGetValue(result2, out var value))
			{
				VoteKicks.Add(result2, value = new VoteKick());
				value.Init(result2);
			}
			value.AddVote(info.sender.Id);
			int num = MathHelper.Floor((float)PhotonNetwork.otherPlayers.Length / 2f);
			if (value.GetVotes() >= num)
			{
				GuardianClient.Commands.Find("kick").Execute(InRoomChat.Instance, new string[5]
				{
					value.Id.ToString(),
					"Voted",
					"out",
					"by",
					"room"
				});
			}
			else
			{
				GameHelper.Broadcast($"Vote-kick for {value.Id}, {value.GetVotes()} out of {num} vote(s) so far!");
			}
		}
		else
		{
			if (info.sender.Muted)
			{
				return;
			}
			if (GuardianClient.Properties.TranslateIncoming.Value && !info.sender.isLocal)
			{
				StartCoroutine(Translator.Translate(message, GuardianClient.Properties.IncomingLanguage.Value, GuardianClient.SystemLanguage, delegate(string[] result)
				{
					if (result.Length > 1 && !result[0].Equals(GuardianClient.SystemLanguage, StringComparison.OrdinalIgnoreCase))
					{
						message = "[gt] ".AsColor("0099ff") + result[1];
					}
					if (sender.Length == 0)
					{
						InRoomChat.Instance.AddMessage(("[" + info.sender.Id + "]").AsColor("FFCC00"), message);
					}
					else
					{
						InRoomChat.Instance.AddMessage(("[" + info.sender.Id + "] ").AsColor("FFCC00") + sender, message);
					}
				}));
			}
			else if (sender.Length == 0)
			{
				InRoomChat.Instance.AddMessage(("[" + info.sender.Id + "]").AsColor("FFCC00"), message);
			}
			else
			{
				InRoomChat.Instance.AddMessage(("[" + info.sender.Id + "] ").AsColor("FFCC00") + sender, message);
			}
		}
	}

	public void OnJoinedRoom()
	{
		PhotonNetwork.room.expectedMaxPlayers = PhotonNetwork.room.maxPlayers;
		PhotonNetwork.room.expectedJoinability = PhotonNetwork.room.open;
		PhotonNetwork.room.expectedVisibility = PhotonNetwork.room.visible;
		string[] array = PhotonNetwork.room.name.Split('`');
		LevelInfo info = LevelInfo.GetInfo(array[1]);
		playerList = string.Empty;
		UnityEngine.MonoBehaviour.print("OnJoinedRoom " + PhotonNetwork.room.name + " >>>> " + info.Map);
		gameTimesUp = false;
		int num;
		switch (array[2].ToLower())
		{
		case "normal":
			num = 0;
			break;
		case "hard":
			num = 1;
			break;
		case "abnormal":
			num = 2;
			break;
		default:
			num = -1;
			break;
		}
		difficulty = num;
		IN_GAME_MAIN_CAMERA.Difficulty = difficulty;
		time = int.Parse(array[3]) * 60;
		DayLight value2;
		if (PhotonNetwork.room.customProperties.ContainsKey("Lighting") && PhotonNetwork.room.customProperties["Lighting"] is string input)
		{
			if (GExtensions.TryParseEnum<DayLight>(input, out var value))
			{
				IN_GAME_MAIN_CAMERA.Lighting = value;
			}
		}
		else if (GExtensions.TryParseEnum<DayLight>(array[4], out value2))
		{
			IN_GAME_MAIN_CAMERA.Lighting = value2;
		}
		if (PhotonNetwork.room.customProperties.ContainsKey("Map") && PhotonNetwork.room.customProperties["Map"] is string text)
		{
			info = LevelInfo.GetInfo(text);
		}
		Level = info;
		IN_GAME_MAIN_CAMERA.Gamemode = info.Mode;
		PhotonNetwork.LoadLevel(info.Map);
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		string value3 = LoginFengKAI.Player.Name;
		if (LoginFengKAI.LoginState != LoginState.LoggedIn && NameField.StripNGUI().Length > 0)
		{
			value3 = (LoginFengKAI.Player.Name = NameField);
		}
		hashtable.Add(PhotonPlayerProperty.Name, value3);
		hashtable.Add(PhotonPlayerProperty.Guild, LoginFengKAI.Player.Guild);
		hashtable.Add(PhotonPlayerProperty.Kills, 0);
		hashtable.Add(PhotonPlayerProperty.MaxDamage, 0);
		hashtable.Add(PhotonPlayerProperty.TotalDamage, 0);
		hashtable.Add(PhotonPlayerProperty.Deaths, 0);
		hashtable.Add(PhotonPlayerProperty.IsDead, true);
		hashtable.Add(PhotonPlayerProperty.IsTitan, 0);
		hashtable.Add(PhotonPlayerProperty.RCTeam, 0);
		hashtable.Add(PhotonPlayerProperty.CurrentLevel, string.Empty);
		PhotonNetwork.player.SetCustomProperties(hashtable);
		humanScore = 0;
		titanScore = 0;
		PVPtitanScore = 0;
		PVPhumanScore = 0;
		wave = 1;
		highestWave = 1;
		localRacingResult = string.Empty;
		needChooseSide = true;
		chatContent = new ArrayList();
		killInfoGO = new ArrayList();
		InRoomChat.Messages = new List<InRoomChat.Message>();
		assetCacheTextures = new Dictionary<string, Texture2D>();
		isFirstLoad = true;
		if (OnPrivateServer)
		{
			ServerRequestAuthentication(PrivateServerAuthPass);
		}
	}

	public void OnLeftRoom()
	{
		if (Application.loadedLevel != 0)
		{
			Time.timeScale = 1f;
			if (PhotonNetwork.connected)
			{
				PhotonNetwork.Disconnect();
			}
			ResetSettings(isLeave: true);
			LoadConfig();
			IN_GAME_MAIN_CAMERA.Gametype = GameType.Stop;
			gameStart = false;
			Screen.lockCursor = false;
			Screen.showCursor = true;
			inputManager.menuOn = false;
			DestroyAllExistingCloths();
			UnityEngine.Object.Destroy(GameObject.Find("MultiplayerManager"));
			Application.LoadLevel("menu");
		}
	}

	public void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
		if (!NoRestart)
		{
			if (PhotonNetwork.isMasterClient)
			{
				restartingMC = true;
				if (RCSettings.InfectionMode > 0)
				{
					restartingTitan = true;
				}
				if (RCSettings.BombMode > 0)
				{
					restartingBomb = true;
				}
				if (RCSettings.HorseMode > 0)
				{
					restartingHorse = true;
				}
				if (RCSettings.BanEren == 0)
				{
					restartingEren = true;
				}
			}
			ResetSettings(isLeave: false);
			if (!Level.PlayerTitans)
			{
				ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable { 
				{
					PhotonPlayerProperty.IsTitan,
					1
				} };
				PhotonNetwork.player.SetCustomProperties(propertiesToSet);
			}
			if (!gameTimesUp && PhotonNetwork.isMasterClient)
			{
				RestartGame(masterClientSwitched: true);
				base.photonView.RPC("setMasterRC", PhotonTargets.All);
			}
		}
		NoRestart = false;
	}

	[RPC]
	private void RPCLoadLevel(PhotonMessageInfo info)
	{
		if (info.sender.isMasterClient)
		{
			DestroyAllExistingCloths();
			PhotonNetwork.LoadLevel(Level.Map);
		}
		else if (PhotonNetwork.isMasterClient)
		{
			KickPlayer(info.sender, ban: true, "False restart.");
		}
		else
		{
			if (MasterRC)
			{
				return;
			}
			restartCount.Add(Time.time);
			foreach (float item in restartCount)
			{
				if (Time.time - item > 60f)
				{
					restartCount.Remove(item);
				}
			}
			if (restartCount.Count < 6)
			{
				DestroyAllExistingCloths();
				PhotonNetwork.LoadLevel(Level.Map);
			}
		}
	}

	private void OnLevelWasLoaded(int level)
	{
		if (level == 0 || Application.loadedLevelName == "characterCreation" || Application.loadedLevelName == "SnapShot")
		{
			return;
		}
		ChangeQuality.SetCurrentQuality();
		difficulty = IN_GAME_MAIN_CAMERA.Difficulty;
		GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
		foreach (GameObject gameObject in array)
		{
			if (gameObject.GetPhotonView() == null || !gameObject.GetPhotonView().owner.isMasterClient)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
		isWinning = false;
		gameStart = true;
		SetTextCenter(string.Empty);
		GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("MainCamera_mono"), GameObject.Find("cameraDefaultPosition").transform.position, GameObject.Find("cameraDefaultPosition").transform.rotation);
		UnityEngine.Object.Destroy(GameObject.Find("cameraDefaultPosition"));
		gameObject2.name = "MainCamera";
		SetCamera(gameObject2.GetComponent<IN_GAME_MAIN_CAMERA>());
		Screen.lockCursor = true;
		Screen.showCursor = true;
		ui = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("UI_IN_GAME"));
		ui.name = "UI_IN_GAME";
		ui.SetActive(value: true);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], state: true);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], state: false);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], state: false);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], state: false);
		Cache();
		LoadSkin();
		gameObject2.GetComponent<IN_GAME_MAIN_CAMERA>().SetHUDPosition();
		gameObject2.GetComponent<IN_GAME_MAIN_CAMERA>().SetLighting(IN_GAME_MAIN_CAMERA.Lighting);
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
		{
			single_kills = 0;
			single_maxDamage = 0;
			single_totalDamage = 0;
			gameObject2.GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
			gameObject2.GetComponent<SpectatorMovement>().disable = true;
			gameObject2.GetComponent<MouseLook>().disable = true;
			IN_GAME_MAIN_CAMERA.Gamemode = Level.Mode;
			SpawnPlayer(IN_GAME_MAIN_CAMERA.SingleCharacter.ToUpper());
			Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode == CameraType.TPS;
			Screen.showCursor = false;
			int abnormal = 90;
			if (difficulty == 1)
			{
				abnormal = 70;
			}
			SpawnTitanCustom("titanRespawn", abnormal, Level.Enemies, punk: false);
			return;
		}
		PVPcheckPoint.chkPts = new ArrayList();
		gameObject2.GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
		gameObject2.GetComponent<CameraShake>().enabled = false;
		IN_GAME_MAIN_CAMERA.Gametype = GameType.Multiplayer;
		if (Level.Mode == GameMode.Trost)
		{
			GameObject.Find("playerRespawn").SetActive(value: false);
			UnityEngine.Object.Destroy(GameObject.Find("playerRespawn"));
			GameObject.Find("rock").animation["lift"].speed = 0f;
			GameObject.Find("door_fine").SetActive(value: false);
			GameObject.Find("door_broke").SetActive(value: true);
			UnityEngine.Object.Destroy(GameObject.Find("ppl"));
		}
		else if (Level.Mode == GameMode.Colossal)
		{
			GameObject.Find("playerRespawnTrost").SetActive(value: false);
			UnityEngine.Object.Destroy(GameObject.Find("playerRespawnTrost"));
		}
		if (needChooseSide)
		{
			AddTextTopCenter("\n\nPRESS 1 TO ENTER GAME");
		}
		else if ((int)Settings[245] == 0)
		{
			Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode == CameraType.TPS;
			if (IN_GAME_MAIN_CAMERA.Gamemode == GameMode.PvPCapture)
			{
				if (GExtensions.AsInt(PhotonNetwork.player.customProperties[PhotonPlayerProperty.IsTitan]) == 2)
				{
					checkpoint = GameObject.Find("PVPchkPtT");
				}
				else
				{
					checkpoint = GameObject.Find("PVPchkPtH");
				}
			}
			if (GExtensions.AsInt(PhotonNetwork.player.customProperties[PhotonPlayerProperty.IsTitan]) == 2)
			{
				SpawnNonAITitan2(myLastHero);
			}
			else
			{
				SpawnPlayer(myLastHero, myLastRespawnTag);
			}
		}
		if (Level.Mode == GameMode.Colossal)
		{
			UnityEngine.Object.Destroy(GameObject.Find("rock"));
		}
		if (PhotonNetwork.isMasterClient)
		{
			switch (Level.Mode)
			{
			case GameMode.Trost:
			{
				if (AreAllPlayersDead())
				{
					break;
				}
				PhotonNetwork.Instantiate("TITAN_EREN_trost", new Vector3(-200f, 0f, -194f), Quaternion.Euler(0f, 180f, 0f), 0).GetComponent<TITAN_EREN>().rockLift = true;
				int rate = 90;
				if (difficulty == 1)
				{
					rate = 70;
				}
				GameObject gameObject4 = GameObject.Find("titanRespawnTrost");
				if (!(gameObject4 != null))
				{
					break;
				}
				array = GameObject.FindGameObjectsWithTag("titanRespawn");
				foreach (GameObject gameObject5 in array)
				{
					if (gameObject5.transform.parent.gameObject == gameObject4)
					{
						SpawnTitan(rate, gameObject5.transform.position, gameObject5.transform.rotation);
					}
				}
				break;
			}
			case GameMode.Colossal:
				if (!AreAllPlayersDead())
				{
					PhotonNetwork.Instantiate("COLOSSAL_TITAN", -Vector3.up * 10000f, Quaternion.Euler(0f, 180f, 0f), 0);
				}
				break;
			case GameMode.KillTitans:
			case GameMode.Endless:
			case GameMode.Survival:
			{
				if (Level.Name == "Annie" || Level.Name == "Annie II")
				{
					GameObject gameObject6 = GameObject.Find("titanRespawn");
					PhotonNetwork.Instantiate("FEMALE_TITAN", gameObject6.transform.position, gameObject6.transform.rotation, 0);
					break;
				}
				int abnormal2 = 90;
				if (difficulty == 1)
				{
					abnormal2 = 70;
				}
				SpawnTitanCustom("titanRespawn", abnormal2, Level.Enemies, punk: false);
				break;
			}
			case GameMode.PvPCapture:
				if (Level.Map == "OutSide")
				{
					array = GameObject.FindGameObjectsWithTag("titanRespawn");
					foreach (GameObject gameObject3 in array)
					{
						SpawnTitanRaw(gameObject3.transform.position, gameObject3.transform.rotation).GetComponent<TITAN>().setAbnormalType2(TitanClass.Crawler, forceCrawler: true);
					}
				}
				break;
			}
			RoundStartTime = GameHelper.CurrentTimeMillis();
			WaveStartTime = RoundStartTime;
		}
		else
		{
			base.photonView.RPC("RequireStatus", PhotonTargets.MasterClient);
		}
		if (!Level.HasSupply)
		{
			UnityEngine.Object.Destroy(GameObject.Find("aot_supply"));
		}
		if (Level.Lava)
		{
			UnityEngine.Object.Instantiate(Resources.Load("levelBottom"), new Vector3(0f, -29.5f, 0f), Quaternion.Euler(0f, 0f, 0f));
			GameObject obj = GameObject.Find("aot_supply");
			GameObject gameObject7 = GameObject.Find("aot_supply_lava_position");
			obj.transform.position = gameObject7.transform.position;
			obj.transform.rotation = gameObject7.transform.rotation;
		}
		if ((int)Settings[245] == 1)
		{
			EnterSpecMode(enter: true);
		}
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (PhotonNetwork.isMasterClient)
		{
			if (BanHash.ContainsValue(GExtensions.AsString(player.customProperties[PhotonPlayerProperty.Name])))
			{
				KickPlayer(player, ban: false, "Banned.");
			}
			else
			{
				int num = GExtensions.AsInt(player.customProperties[PhotonPlayerProperty.StatAccel]);
				int num2 = GExtensions.AsInt(player.customProperties[PhotonPlayerProperty.StatBlade]);
				int num3 = GExtensions.AsInt(player.customProperties[PhotonPlayerProperty.StatGas]);
				int num4 = GExtensions.AsInt(player.customProperties[PhotonPlayerProperty.StatSpeed]);
				if (num > 150 || num2 > 125 || num3 > 150 || num4 > 140)
				{
					KickPlayer(player, ban: true, "Excessive stats.");
					return;
				}
				if (RCSettings.AsoPreserveKDR == 1)
				{
					StartCoroutine(CoWaitAndReloadKD(player));
				}
				if (Level.Name.StartsWith("Custom"))
				{
					StartCoroutine(CoLoadCustomLevel(new List<PhotonPlayer> { player }));
				}
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				if (RCSettings.BombMode == 1)
				{
					hashtable.Add("bomb", 1);
				}
				if (RCSettings.GlobalDisableMinimap == 1)
				{
					hashtable.Add("globalDisableMinimap", 1);
				}
				if (RCSettings.TeamMode > 0)
				{
					hashtable.Add("team", RCSettings.TeamMode);
				}
				if (RCSettings.PointMode > 0)
				{
					hashtable.Add("point", RCSettings.PointMode);
				}
				if (RCSettings.DisableRock > 0)
				{
					hashtable.Add("rock", RCSettings.DisableRock);
				}
				if (RCSettings.ExplodeMode > 0)
				{
					hashtable.Add("explode", RCSettings.ExplodeMode);
				}
				if (RCSettings.HealthMode > 0)
				{
					hashtable.Add("healthMode", RCSettings.HealthMode);
					hashtable.Add("healthLower", RCSettings.HealthLower);
					hashtable.Add("healthUpper", RCSettings.HealthUpper);
				}
				if (RCSettings.InfectionMode > 0)
				{
					hashtable.Add("infection", RCSettings.InfectionMode);
				}
				if (RCSettings.BanEren == 1)
				{
					hashtable.Add("eren", RCSettings.BanEren);
				}
				if (RCSettings.MoreTitans > 0)
				{
					hashtable.Add("titanc", RCSettings.MoreTitans);
				}
				if (RCSettings.MinimumDamage > 0)
				{
					hashtable.Add("damage", RCSettings.MinimumDamage);
				}
				if (RCSettings.SizeMode > 0)
				{
					hashtable.Add("sizeMode", RCSettings.SizeMode);
					hashtable.Add("sizeLower", RCSettings.SizeLower);
					hashtable.Add("sizeUpper", RCSettings.SizeUpper);
				}
				if (RCSettings.SpawnMode > 0)
				{
					hashtable.Add("spawnMode", RCSettings.SpawnMode);
					hashtable.Add("nRate", RCSettings.NormalRate);
					hashtable.Add("aRate", RCSettings.AberrantRate);
					hashtable.Add("jRate", RCSettings.JumperRate);
					hashtable.Add("cRate", RCSettings.CrawlerRate);
					hashtable.Add("pRate", RCSettings.PunkRate);
				}
				if (RCSettings.WaveModeOn > 0)
				{
					hashtable.Add("waveModeOn", 1);
					hashtable.Add("waveModeNum", RCSettings.WaveModeNum);
				}
				if (RCSettings.FriendlyMode > 0)
				{
					hashtable.Add("friendly", 1);
				}
				if (RCSettings.PvPMode > 0)
				{
					hashtable.Add("pvp", RCSettings.PvPMode);
				}
				if (RCSettings.MaxWave > 0)
				{
					hashtable.Add("maxwave", RCSettings.MaxWave);
				}
				if (RCSettings.EndlessMode > 0)
				{
					hashtable.Add("endless", RCSettings.EndlessMode);
				}
				if (RCSettings.Motd.Length > 0)
				{
					hashtable.Add("motd", RCSettings.Motd);
				}
				if (RCSettings.HorseMode > 0)
				{
					hashtable.Add("horse", RCSettings.HorseMode);
				}
				if (RCSettings.AhssReload > 0)
				{
					hashtable.Add("ahssReload", RCSettings.AhssReload);
				}
				if (RCSettings.PunkWaves > 0)
				{
					hashtable.Add("punkWaves", RCSettings.PunkWaves);
				}
				if (RCSettings.DeadlyCannons > 0)
				{
					hashtable.Add("deadlycannons", RCSettings.DeadlyCannons);
				}
				if (RCSettings.RacingStatic > 0)
				{
					hashtable.Add("asoracing", RCSettings.RacingStatic);
				}
				if (IgnoreList != null && IgnoreList.Count > 0)
				{
					base.photonView.RPC("ignorePlayerArray", player, IgnoreList.ToArray());
				}
				base.photonView.RPC("settingRPC", player, hashtable);
				base.photonView.RPC("setMasterRC", player);
				if (Time.timeScale <= 0.1f && pauseWaitTime > 3f)
				{
					base.photonView.RPC("pauseRPC", player, true);
					base.photonView.RPC("Chat", player, "MasterClient has paused the game.".AsColor("FFCC00"), string.Empty);
				}
			}
		}
		RecompilePlayerList(0.1f);
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		VoteKicks.Remove(player.Id);
		if (!gameTimesUp)
		{
			oneTitanDown(string.Empty, onPlayerLeave: true);
			someOneIsDead(0);
		}
		if (IgnoreList.Contains(player.Id))
		{
			IgnoreList.Remove(player.Id);
		}
		InstantiateTracker.Instance.TryRemovePlayer(player.Id);
		if (PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("verifyPlayerHasLeft", PhotonTargets.All, player.Id);
		}
		if (RCSettings.AsoPreserveKDR == 1)
		{
			string key = GExtensions.AsString(player.customProperties[PhotonPlayerProperty.Name]);
			if (PreservedPlayerKDR.ContainsKey(key))
			{
				PreservedPlayerKDR.Remove(key);
			}
			int[] value = new int[4]
			{
				GExtensions.AsInt(player.customProperties[PhotonPlayerProperty.Kills]),
				GExtensions.AsInt(player.customProperties[PhotonPlayerProperty.Deaths]),
				GExtensions.AsInt(player.customProperties[PhotonPlayerProperty.MaxDamage]),
				GExtensions.AsInt(player.customProperties[PhotonPlayerProperty.TotalDamage])
			};
			PreservedPlayerKDR.Add(key, value);
		}
		RecompilePlayerList(0.1f);
	}

	public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
	{
		RecompilePlayerList(0.1f);
		PhotonPlayer photonPlayer = playerAndUpdatedProps[0] as PhotonPlayer;
		ExitGames.Client.Photon.Hashtable hashtable = (ExitGames.Client.Photon.Hashtable)playerAndUpdatedProps[1];
		if (hashtable.ContainsKey("sender") && hashtable["sender"] is PhotonPlayer photonPlayer2 && photonPlayer.isLocal && !photonPlayer2.isLocal)
		{
			ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable();
			if (hashtable.ContainsKey(PhotonPlayerProperty.Name) && photonPlayer.Username != LoginFengKAI.Player.Name)
			{
				hashtable2.Add(PhotonPlayerProperty.Name, LoginFengKAI.Player.Name);
			}
			if (hashtable.ContainsKey(PhotonPlayerProperty.Guild) && photonPlayer.Guild != LoginFengKAI.Player.Guild)
			{
				hashtable2.Add(PhotonPlayerProperty.Guild, LoginFengKAI.Player.Guild);
			}
			if (hashtable.ContainsKey(PhotonPlayerProperty.StatSpeed) && photonPlayer.SpeedStat > 140)
			{
				hashtable2.Add(PhotonPlayerProperty.StatSpeed, 100);
			}
			if (hashtable.ContainsKey(PhotonPlayerProperty.StatBlade) && photonPlayer.BladeStat > 125)
			{
				hashtable2.Add(PhotonPlayerProperty.StatBlade, 100);
			}
			if (hashtable.ContainsKey(PhotonPlayerProperty.StatGas) && photonPlayer.GasStat > 150)
			{
				hashtable2.Add(PhotonPlayerProperty.StatGas, 100);
			}
			if (hashtable.ContainsKey(PhotonPlayerProperty.StatAccel) && photonPlayer.AccelStat > 150)
			{
				hashtable2.Add(PhotonPlayerProperty.StatAccel, 100);
			}
			if (hashtable2.Count >= 1)
			{
				PhotonNetwork.player.SetCustomProperties(hashtable2);
			}
		}
	}

	public void OnPhotonCustomRoomPropertiesChanged()
	{
		if (!PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.room.expectedMaxPlayers = PhotonNetwork.room.maxPlayers;
			PhotonNetwork.room.expectedJoinability = PhotonNetwork.room.open;
			PhotonNetwork.room.expectedVisibility = PhotonNetwork.room.visible;
		}
	}

	[RPC]
	private void showResult(string players, string kills, string deaths, string maxDamage, string totalDamage, string gameResult, PhotonMessageInfo info)
	{
		if (!gameTimesUp && info.sender.isMasterClient)
		{
			gameTimesUp = true;
			NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], state: false);
			NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], state: false);
			NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], state: true);
			NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], state: false);
			GameObject.Find("LabelName").GetComponent<UILabel>().text = players;
			GameObject.Find("LabelKill").GetComponent<UILabel>().text = kills;
			GameObject.Find("LabelDead").GetComponent<UILabel>().text = deaths;
			GameObject.Find("LabelMaxDmg").GetComponent<UILabel>().text = maxDamage;
			GameObject.Find("LabelTotalDmg").GetComponent<UILabel>().text = totalDamage;
			GameObject.Find("LabelResultTitle").GetComponent<UILabel>().text = gameResult;
			Screen.lockCursor = false;
			Screen.showCursor = true;
			IN_GAME_MAIN_CAMERA.Gametype = GameType.Stop;
			gameStart = false;
		}
		else if (!info.sender.isMasterClient && PhotonNetwork.player.isMasterClient)
		{
			KickPlayer(info.sender, ban: true, "false game end.");
		}
	}

	[RPC]
	private void restartGameByClient()
	{
	}

	[RPC]
	private void updateKillInfo(bool isKillerTitan, string killer, bool isVictimTitan, string victim, int damage, PhotonMessageInfo info)
	{
		if (!FGMChecker.IsKillInfoUpdateValid(isKillerTitan, isVictimTitan, damage, info))
		{
			return;
		}
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("UI/KillInfo"));
		for (int i = 0; i < killInfoGO.Count; i++)
		{
			GameObject gameObject2 = (GameObject)killInfoGO[i];
			if (gameObject2 != null)
			{
				gameObject2.GetComponent<KillInfoComponent>().MoveOn();
			}
		}
		if (killInfoGO.Count > 4)
		{
			GameObject gameObject3 = (GameObject)killInfoGO[0];
			if (gameObject3 != null)
			{
				gameObject3.GetComponent<KillInfoComponent>().EndLifeTime();
			}
			killInfoGO.RemoveAt(0);
		}
		gameObject.transform.parent = ui.GetComponent<UIReferArray>().panels[0].transform;
		gameObject.GetComponent<KillInfoComponent>().Show(isKillerTitan, killer, isVictimTitan, victim, damage);
		killInfoGO.Add(gameObject);
		if ((int)Settings[244] == 1)
		{
			InRoomChat.Instance.AddLine(("(" + roundTime.ToString("F2") + ") ").AsColor("FFCC00") + killer.NGUIToUnity() + " killed " + victim.NGUIToUnity() + " for " + damage + " damage.");
		}
	}

	[RPC]
	private void netGameLose(int score, PhotonMessageInfo info)
	{
		isLosing = true;
		titanScore = score;
		gameEndCD = gameEndTotalCDtime;
		if ((int)Settings[244] == 1)
		{
			InRoomChat.Instance.AddLine(("(" + roundTime.ToString("F2") + ") ").AsColor("FFCC00") + "Round ended (game lose).");
		}
		if (!info.sender.isMasterClient && !info.sender.isLocal && PhotonNetwork.isMasterClient)
		{
			InRoomChat.Instance.AddLine("Round end sent from #".AsColor("FFCC00") + info.sender.Id);
		}
	}

	[RPC]
	private void netGameWin(int score, PhotonMessageInfo info)
	{
		humanScore = score;
		isWinning = true;
		switch (IN_GAME_MAIN_CAMERA.Gamemode)
		{
		case GameMode.TeamDeathmatch:
			teamWinner = score;
			teamScores[teamWinner - 1]++;
			gameEndCD = gameEndTotalCDtime;
			break;
		case GameMode.Racing:
			if (RCSettings.RacingStatic == 1)
			{
				gameEndCD = 1000f;
			}
			else
			{
				gameEndCD = 20f;
			}
			break;
		default:
			gameEndCD = gameEndTotalCDtime;
			break;
		}
		if ((int)Settings[244] == 1)
		{
			InRoomChat.Instance.AddLine(("(" + roundTime.ToString("F2") + ") ").AsColor("FFCC00") + "Round ended (game win).");
		}
		if (!info.sender.isMasterClient && !info.sender.isLocal)
		{
			InRoomChat.Instance.AddLine("Round end sent from #".AsColor("FFCC00") + info.sender.Id);
		}
	}

	public void OnJoinedLobby()
	{
		UIMainReferences component = GameObject.Find("UIRefer").GetComponent<UIMainReferences>();
		NGUITools.SetActive(component.panelMultiStart, state: false);
		NGUITools.SetActive(component.panelMultiROOM, state: true);
		NGUITools.SetActive(component.PanelMultiJoinPrivate, state: false);
	}

	public void RestartGame(bool masterClientSwitched = false)
	{
		if (!gameTimesUp)
		{
			PVPtitanScore = 0;
			PVPhumanScore = 0;
			startRacing = false;
			endRacing = false;
			checkpoint = null;
			timeElapse = 0f;
			roundTime = 0f;
			isWinning = false;
			isLosing = false;
			wave = 1;
			myRespawnTime = 0f;
			killInfoGO = new ArrayList();
			racingResult = new ArrayList();
			SetTextCenter(string.Empty);
			isRestarting = true;
			DestroyAllExistingCloths();
			PhotonNetwork.DestroyAll();
			ExitGames.Client.Photon.Hashtable hashtable = CheckGameGUI();
			base.photonView.RPC("settingRPC", PhotonTargets.Others, hashtable);
			base.photonView.RPC("RPCLoadLevel", PhotonTargets.All);
			SetGameSettings(hashtable);
			if (masterClientSwitched)
			{
				GameHelper.Broadcast("MasterClient has switched to " + ((string)PhotonNetwork.player.customProperties[PhotonPlayerProperty.Name]).NGUIToUnity().AsBold());
				VoteKicks = new Dictionary<int, VoteKick>();
			}
		}
	}

	private void Core2()
	{
		if ((int)Settings[64] >= 100)
		{
			CoreEditor();
			return;
		}
		if (IN_GAME_MAIN_CAMERA.Gametype != 0 && needChooseSide)
		{
			if (inputManager.isInputDown[InputCode.Flare1])
			{
				if (NGUITools.GetActive(ui.GetComponent<UIReferArray>().panels[3]))
				{
					Screen.lockCursor = true;
					Screen.showCursor = true;
					NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], state: true);
					NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], state: false);
					NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], state: false);
					NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], state: false);
					Camera.main.GetComponent<SpectatorMovement>().disable = false;
					Camera.main.GetComponent<MouseLook>().disable = false;
				}
				else
				{
					Screen.lockCursor = false;
					Screen.showCursor = true;
					NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[0], state: false);
					NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], state: false);
					NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], state: false);
					NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], state: true);
					Camera.main.GetComponent<SpectatorMovement>().disable = true;
					Camera.main.GetComponent<MouseLook>().disable = true;
				}
			}
			if (inputManager.isInputDown[15] && !inputManager.menuOn)
			{
				Screen.showCursor = true;
				Screen.lockCursor = false;
				Camera.main.GetComponent<SpectatorMovement>().disable = true;
				Camera.main.GetComponent<MouseLook>().disable = true;
				inputManager.menuOn = true;
			}
		}
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Stop)
		{
			return;
		}
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
		{
			CoreAdd();
			SetTextTopLeft(playerList);
			if (Camera.main != null && IN_GAME_MAIN_CAMERA.Gamemode != GameMode.Racing && mainCamera.gameOver && !needChooseSide && (int)Settings[245] == 0)
			{
				SetTextCenter("Press [F7D358]" + inputManager.inputString[InputCode.Flare1] + "[-] to spectate the next player. \nPress [F7D358]" + inputManager.inputString[InputCode.Flare2] + "[-] to spectate the previous player.\nPress [F7D358]" + inputManager.inputString[InputCode.Attack1] + "[-] to enter the spectator mode.\n\n\n\n");
				if (Level.RespawnMode == RespawnMode.Deathmatch || RCSettings.EndlessMode > 0 || ((RCSettings.BombMode == 1 || RCSettings.PvPMode > 0) && RCSettings.PointMode > 0))
				{
					myRespawnTime += Time.deltaTime;
					int num = 10;
					if (GExtensions.AsInt(PhotonNetwork.player.customProperties[PhotonPlayerProperty.IsTitan]) == 2)
					{
						num = 15;
					}
					if (RCSettings.EndlessMode > 0)
					{
						num = RCSettings.EndlessMode;
					}
					AddTextCenter("Respawn in " + (num - (int)myRespawnTime) + "s.");
					if (myRespawnTime > (float)num)
					{
						myRespawnTime = 0f;
						if (GExtensions.AsInt(PhotonNetwork.player.customProperties[PhotonPlayerProperty.IsTitan]) == 2)
						{
							SpawnNonAITitan2(myLastHero);
						}
						else
						{
							StartCoroutine(CoWaitAndRespawn1(0.1f, myLastRespawnTag));
						}
						mainCamera.gameOver = false;
						SetTextCenter(string.Empty);
					}
				}
			}
			if (isLosing && IN_GAME_MAIN_CAMERA.Gamemode != GameMode.Racing)
			{
				if (IN_GAME_MAIN_CAMERA.Gamemode == GameMode.Survival)
				{
					SetTextCenter("Survived " + wave + " Waves!\nGame Restart in " + (int)gameEndCD + "s");
				}
				else
				{
					SetTextCenter("Humanity Failed!\nGame Restart in " + (int)gameEndCD + "s");
				}
				if (gameEndCD <= 0f)
				{
					gameEndCD = 0f;
					if (PhotonNetwork.isMasterClient)
					{
						RestartRC();
					}
					SetTextCenter(string.Empty);
				}
				else
				{
					gameEndCD -= Time.deltaTime;
				}
			}
			if (isWinning)
			{
				switch (IN_GAME_MAIN_CAMERA.Gamemode)
				{
				case GameMode.Racing:
					SetTextCenter(localRacingResult + "\n\nGame Restart in " + (int)gameEndCD + "s");
					break;
				case GameMode.Survival:
					SetTextCenter("Survived All Waves!\nGame Restart in " + (int)gameEndCD + "s");
					break;
				case GameMode.TeamDeathmatch:
					if (RCSettings.PvPMode == 0 && RCSettings.BombMode == 0)
					{
						SetTextCenter("Team " + teamWinner + " Wins!\nGame Restart in " + (int)gameEndCD + "s");
					}
					else
					{
						SetTextCenter("Round Ended!\nGame Restart in " + (int)gameEndCD + "s");
					}
					break;
				default:
					SetTextCenter("Humanity Wins!\nGame Restart in " + (int)gameEndCD + "s");
					break;
				}
				if (gameEndCD <= 0f)
				{
					gameEndCD = 0f;
					if (PhotonNetwork.isMasterClient)
					{
						RestartRC();
					}
					SetTextCenter(string.Empty);
				}
				else
				{
					gameEndCD -= Time.deltaTime;
				}
			}
			timeTotalServer += Time.deltaTime;
		}
		else if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
		{
			if (IN_GAME_MAIN_CAMERA.Gamemode == GameMode.Racing)
			{
				if (!isLosing)
				{
					currentSpeed = mainCamera.main_object.rigidbody.velocity.magnitude;
					maxSpeed = Mathf.Max(maxSpeed, currentSpeed);
					SetTextTopLeft("Current Speed: " + (int)currentSpeed + "\nMax Speed: " + maxSpeed);
				}
			}
			else
			{
				SetTextTopLeft("Kills: " + single_kills + "\nMax Damage: " + single_maxDamage + "\nTotal Damage: " + single_totalDamage);
				if (isLosing)
				{
					if (IN_GAME_MAIN_CAMERA.Gamemode == GameMode.Survival)
					{
						SetTextCenter("Survived " + wave + " Waves!\nPress " + inputManager.inputString[InputCode.Restart] + " to restart.");
					}
					else
					{
						SetTextCenter("Humanity Fail!\nPress " + inputManager.inputString[InputCode.Restart] + " to restart.");
					}
				}
			}
			if (isWinning)
			{
				switch (IN_GAME_MAIN_CAMERA.Gamemode)
				{
				case GameMode.Racing:
					SetTextCenter(timeTotalServer * 10f * 0.1f - 5f + "!\nPress " + inputManager.inputString[InputCode.Restart] + " to restart.");
					break;
				case GameMode.Survival:
					SetTextCenter("Survived All Waves!\nPress " + inputManager.inputString[InputCode.Restart] + " to restart.");
					break;
				default:
					SetTextCenter("Humanity Wins!\nPress " + inputManager.inputString[InputCode.Restart] + " to restart.");
					break;
				}
			}
			if (IN_GAME_MAIN_CAMERA.Gamemode == GameMode.Racing)
			{
				if (!isWinning)
				{
					timeTotalServer += Time.deltaTime;
				}
			}
			else if (!isLosing && !isWinning)
			{
				timeTotalServer += Time.deltaTime;
			}
		}
		timeElapse += Time.deltaTime;
		roundTime += Time.deltaTime;
		if (IN_GAME_MAIN_CAMERA.Gamemode == GameMode.Racing)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
			{
				if (!isWinning)
				{
					SetTextTopCenter("Time: " + (timeTotalServer * 10f * 0.1f - 5f));
				}
				if (timeTotalServer < 5f)
				{
					SetTextCenter("RACE START IN " + (5f - timeTotalServer));
				}
				else if (!startRacing)
				{
					SetTextCenter(string.Empty);
					startRacing = true;
					endRacing = false;
					GameObject.Find("door").SetActive(value: false);
				}
			}
			else
			{
				SetTextTopCenter("Time: " + ((roundTime >= 20f) ? (roundTime * 10f * 0.1f - 20f).ToString() : "WAITING"));
				if (roundTime < 20f)
				{
					SetTextCenter("RACE START IN " + (20f - roundTime) + ((localRacingResult.Length > 0) ? ("\nLast Round\n" + localRacingResult) : string.Empty));
				}
				else if (!startRacing)
				{
					SetTextCenter(string.Empty);
					startRacing = true;
					endRacing = false;
					GameObject gameObject = GameObject.Find("door");
					if (gameObject != null)
					{
						gameObject.SetActive(value: false);
					}
					if (racingDoors != null && CustomLevelLoaded)
					{
						foreach (GameObject racingDoor in racingDoors)
						{
							racingDoor.SetActive(value: false);
						}
						racingDoors = null;
					}
				}
				else if (racingDoors != null && CustomLevelLoaded)
				{
					foreach (GameObject racingDoor2 in racingDoors)
					{
						racingDoor2.SetActive(value: false);
					}
					racingDoors = null;
				}
			}
			if (mainCamera.gameOver && !needChooseSide && CustomLevelLoaded)
			{
				myRespawnTime += Time.deltaTime;
				if (myRespawnTime > 1.5f)
				{
					myRespawnTime = 0f;
					if (checkpoint != null)
					{
						StartCoroutine(CoWaitAndRespawn2(0.1f, checkpoint));
					}
					else
					{
						StartCoroutine(CoWaitAndRespawn1(0.1f, myLastRespawnTag));
					}
					mainCamera.gameOver = false;
					SetTextCenter(string.Empty);
				}
			}
		}
		if (timeElapse > 1f)
		{
			timeElapse -= 1f;
			string text = string.Empty;
			switch (IN_GAME_MAIN_CAMERA.Gamemode)
			{
			case GameMode.Endless:
				text = "Time: " + (int)((float)time - timeTotalServer);
				break;
			case GameMode.KillTitans:
			case GameMode.None:
				text = "Titan Left: " + AllTitans.Count + " | Time: " + (int)((IN_GAME_MAIN_CAMERA.Gametype != 0) ? ((float)time - timeTotalServer) : timeTotalServer);
				break;
			case GameMode.Survival:
				text = "Titan Left: " + AllTitans.Count + " | Wave: " + wave;
				break;
			case GameMode.Colossal:
				text = "Time: " + (int)((float)time - timeTotalServer) + "\nDefeat the Colossal Titan\nand prevent abnormal titans from reaching the North gate!";
				break;
			case GameMode.PvPCapture:
			{
				string text2 = "| ";
				for (int i = 0; i < PVPcheckPoint.chkPts.Count; i++)
				{
					text2 = text2 + (PVPcheckPoint.chkPts[i] as PVPcheckPoint).GetState() + " ";
				}
				text = $"[{ColorSet.TitanPlayer}]{PVPtitanScoreMax - PVPtitanScore} [-]{text2}| [{ColorSet.Human}]{PVPhumanScoreMax - PVPhumanScore}\n[-]Time: {(int)((float)time - timeTotalServer)}";
				break;
			}
			}
			if (RCSettings.TeamMode > 0)
			{
				text = text + "\n[00FFFF]Cyan: " + Convert.ToString(cyanKills) + " | [FF00FF]Magenta: " + Convert.ToString(magentaKills) + "[FFFFFF]";
			}
			SetTextTopCenter(text);
			text = string.Empty;
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
			{
				if (IN_GAME_MAIN_CAMERA.Gamemode == GameMode.Survival)
				{
					text = "Time: " + (int)timeTotalServer;
				}
			}
			else
			{
				switch (IN_GAME_MAIN_CAMERA.Gamemode)
				{
				case GameMode.KillTitans:
				case GameMode.Endless:
				case GameMode.Colossal:
				case GameMode.PvPCapture:
					text = "Humanity: " + humanScore + " | Titan: " + titanScore;
					break;
				case GameMode.Survival:
					text = "Time: " + (int)((float)time - timeTotalServer);
					break;
				case GameMode.TeamDeathmatch:
				{
					for (int j = 0; j < teamScores.Length; j++)
					{
						text += ((j == 0) ? string.Empty : " | ");
						text = text + "Team " + (j + 1) + ": " + teamScores[j];
					}
					text = text + "\nTime: " + (int)((float)time - timeTotalServer);
					break;
				}
				}
			}
			SetTextTopRight(text);
			string text3;
			switch (IN_GAME_MAIN_CAMERA.Difficulty)
			{
			case -1:
				text3 = "[FFCC00]Training";
				break;
			case 0:
				text3 = "[00FF00]Normal";
				break;
			case 1:
				text3 = "[FFFF00]Hard";
				break;
			case 2:
				text3 = "[FF0000]Abnormal";
				break;
			default:
				text3 = "[000000]Unknown";
				break;
			}
			string text4 = text3;
			AddTextTopRight("\n" + Level.Name + "(" + text4 + "[-])");
			AddTextTopRight("\nCamera([AAFF00]" + IN_GAME_MAIN_CAMERA.CameraMode.ToString() + "[-])");
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
			{
				string[] array = PhotonNetwork.room.name.Split('`');
				string text5 = "\n\n" + array[0] + "\n";
				int playerCount = PhotonNetwork.room.playerCount;
				int maxPlayers = PhotonNetwork.room.maxPlayers;
				text5 = ((PhotonNetwork.room.open && (maxPlayers == 0 || playerCount < maxPlayers) && PhotonNetwork.room.open) ? (text5 + "[AAFF00]") : (text5 + "[FF4444]"));
				text5 += $"({playerCount}/{maxPlayers})";
				if (!PhotonNetwork.room.visible)
				{
					text5 += " [ff6600](hidden)[-]";
				}
				AddTextTopRight(text5);
				if (needChooseSide)
				{
					AddTextTopCenter("\n\nPRESS 1 TO ENTER GAME");
				}
			}
			if (GuardianClient.Properties.Interpolation.Value)
			{
				AddTextTopCenter("\nInterpolation is [00FF00]ON[-]");
			}
			else
			{
				AddTextTopCenter("\nInterpolation is [FF0000]OFF[-]");
			}
		}
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && killInfoGO.Count > 0 && killInfoGO[0] == null)
		{
			killInfoGO.RemoveAt(0);
		}
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer || PhotonNetwork.offlineMode || !PhotonNetwork.isMasterClient || timeTotalServer < (float)time)
		{
			return;
		}
		IN_GAME_MAIN_CAMERA.Gametype = GameType.Stop;
		gameStart = false;
		Screen.lockCursor = false;
		Screen.showCursor = true;
		string text6 = string.Empty;
		string text7 = string.Empty;
		string text8 = string.Empty;
		string text9 = string.Empty;
		string text10 = string.Empty;
		PhotonPlayer[] array2 = PhotonNetwork.playerList;
		foreach (PhotonPlayer photonPlayer in array2)
		{
			text6 = text6 + photonPlayer.customProperties[PhotonPlayerProperty.Name]?.ToString() + "\n";
			text7 = text7 + photonPlayer.customProperties[PhotonPlayerProperty.Kills]?.ToString() + "\n";
			text8 = text8 + photonPlayer.customProperties[PhotonPlayerProperty.Deaths]?.ToString() + "\n";
			text9 = text9 + photonPlayer.customProperties[PhotonPlayerProperty.MaxDamage]?.ToString() + "\n";
			text10 = text10 + photonPlayer.customProperties[PhotonPlayerProperty.TotalDamage]?.ToString() + "\n";
		}
		string text11 = string.Empty;
		switch (IN_GAME_MAIN_CAMERA.Gamemode)
		{
		case GameMode.TeamDeathmatch:
		{
			for (int l = 0; l < teamScores.Length; l++)
			{
				text11 = string.Concat(text11, ((l == 0) ? string.Empty : " | ") + "Team" + (l + 1) + " " + teamScores[l]);
			}
			break;
		}
		case GameMode.Survival:
			text11 = "Highest Wave: " + highestWave;
			break;
		default:
			text11 = "Humanity " + humanScore + " | Titan " + titanScore;
			break;
		}
		base.photonView.RPC("showResult", PhotonTargets.AllBuffered, text6, text7, text8, text9, text10, text11);
	}

	public void SpawnPlayerAt2(string id, GameObject pos)
	{
		if (LogicLoaded && CustomLevelLoaded)
		{
			Vector3 position = pos.transform.position;
			if (racingSpawnPointSet)
			{
				position = racingSpawnPoint;
			}
			else if (Level.Name.StartsWith("Custom"))
			{
				if (GExtensions.AsInt(PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCTeam]) == 0)
				{
					List<Vector3> list = new List<Vector3>();
					foreach (Vector3 item in playerSpawnsC)
					{
						list.Add(item);
					}
					foreach (Vector3 item2 in playerSpawnsM)
					{
						list.Add(item2);
					}
					if (list.Count > 0)
					{
						position = list[UnityEngine.Random.Range(0, list.Count)];
					}
				}
				else if (GExtensions.AsInt(PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCTeam]) == 1)
				{
					if (playerSpawnsC.Count > 0)
					{
						position = playerSpawnsC[UnityEngine.Random.Range(0, playerSpawnsC.Count)];
					}
				}
				else if (GExtensions.AsInt(PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCTeam]) == 2 && playerSpawnsM.Count > 0)
				{
					position = playerSpawnsM[UnityEngine.Random.Range(0, playerSpawnsM.Count)];
				}
			}
			GameObject gameObject = GameObject.Find("MainCamera");
			myLastHero = id.ToUpper();
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
			{
				if (IN_GAME_MAIN_CAMERA.SingleCharacter == "TITAN_EREN")
				{
					mainCamera.SetMainObject((GameObject)UnityEngine.Object.Instantiate(Resources.Load("TITAN_EREN"), pos.transform.position, pos.transform.rotation));
				}
				else
				{
					mainCamera.SetMainObject((GameObject)UnityEngine.Object.Instantiate(Resources.Load("AOTTG_HERO 1"), pos.transform.position, pos.transform.rotation));
					HERO component = mainCamera.main_object.GetComponent<HERO>();
					HERO_SETUP component2 = component.GetComponent<HERO_SETUP>();
					if (IN_GAME_MAIN_CAMERA.SingleCharacter == "SET 1" || IN_GAME_MAIN_CAMERA.SingleCharacter == "SET 2" || IN_GAME_MAIN_CAMERA.SingleCharacter == "SET 3")
					{
						HeroCostume heroCostume = CostumeConverter.FromLocalData(IN_GAME_MAIN_CAMERA.SingleCharacter);
						CostumeConverter.ToLocalData(heroCostume, IN_GAME_MAIN_CAMERA.SingleCharacter);
						component2.Init();
						if (heroCostume != null)
						{
							component2.myCostume = heroCostume;
							component2.myCostume.stat = heroCostume.stat;
						}
						else
						{
							heroCostume = (component2.myCostume = HeroCostume.CostumeOptions[3]);
							component2.myCostume.stat = HeroStat.GetInfo(heroCostume.name.ToUpper());
						}
						component2.CreateCharacterComponent();
						component.SetStat2();
						component.SetSkillHUDPosition2();
					}
					else
					{
						for (int i = 0; i < HeroCostume.Costumes.Length; i++)
						{
							if (HeroCostume.Costumes[i].name.ToUpper() == IN_GAME_MAIN_CAMERA.SingleCharacter.ToUpper())
							{
								int num = HeroCostume.Costumes[i].id + CheckBoxCostume.CostumeSet - 1;
								if (HeroCostume.Costumes[num].name != HeroCostume.Costumes[i].name)
								{
									num = HeroCostume.Costumes[i].id + 1;
								}
								component2.Init();
								component2.myCostume = HeroCostume.Costumes[num];
								component2.myCostume.stat = HeroStat.GetInfo(HeroCostume.Costumes[num].name.ToUpper());
								component2.CreateCharacterComponent();
								component.SetStat2();
								component.SetSkillHUDPosition2();
								break;
							}
						}
					}
				}
			}
			else
			{
				mainCamera.SetMainObject(PhotonNetwork.Instantiate("AOTTG_HERO 1", position, pos.transform.rotation, 0));
				HERO component3 = mainCamera.main_object.GetComponent<HERO>();
				HERO_SETUP component4 = component3.GetComponent<HERO_SETUP>();
				id = id.ToUpper();
				switch (id)
				{
				case "SET 1":
				case "SET 2":
				case "SET 3":
				{
					HeroCostume heroCostume2 = CostumeConverter.FromLocalData(id);
					CostumeConverter.ToLocalData(heroCostume2, id);
					component4.Init();
					if (heroCostume2 != null)
					{
						component4.myCostume = heroCostume2;
						component4.myCostume.stat = heroCostume2.stat;
					}
					else
					{
						heroCostume2 = (component4.myCostume = HeroCostume.CostumeOptions[3]);
						component4.myCostume.stat = HeroStat.GetInfo(heroCostume2.name.ToUpper());
					}
					component4.CreateCharacterComponent();
					component3.SetStat2();
					component3.SetSkillHUDPosition2();
					break;
				}
				default:
				{
					for (int j = 0; j < HeroCostume.Costumes.Length; j++)
					{
						if (HeroCostume.Costumes[j].name.ToUpper() == id.ToUpper())
						{
							int num2 = HeroCostume.Costumes[j].id;
							if (id.ToUpper() != "AHSS")
							{
								num2 += CheckBoxCostume.CostumeSet - 1;
							}
							if (HeroCostume.Costumes[num2].name != HeroCostume.Costumes[j].name)
							{
								num2 = HeroCostume.Costumes[j].id + 1;
							}
							component4.Init();
							component4.myCostume = HeroCostume.Costumes[num2];
							component4.myCostume.stat = HeroStat.GetInfo(HeroCostume.Costumes[num2].name.ToUpper());
							component4.CreateCharacterComponent();
							component3.SetStat2();
							component3.SetSkillHUDPosition2();
							break;
						}
					}
					break;
				}
				}
				CostumeConverter.ToPhotonData(component4.myCostume, PhotonNetwork.player);
				if (IN_GAME_MAIN_CAMERA.Gamemode == GameMode.PvPCapture)
				{
					mainCamera.main_object.transform.position += new Vector3(UnityEngine.Random.Range(-20, 20), 2f, UnityEngine.Random.Range(-20, 20));
				}
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable.Add("dead", false);
				hashtable.Add(PhotonPlayerProperty.IsTitan, 1);
				PhotonNetwork.player.SetCustomProperties(hashtable);
			}
			mainCamera.enabled = true;
			mainCamera.SetHUDPosition();
			gameObject.GetComponent<SpectatorMovement>().disable = true;
			gameObject.GetComponent<MouseLook>().disable = true;
			mainCamera.gameOver = false;
			Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode == CameraType.TPS;
			Screen.showCursor = false;
			isLosing = false;
			SetTextCenter(string.Empty);
		}
		else
		{
			NOTSpawnPlayerRC(id);
		}
	}

	public void SpawnNonAITitan2(string id, string tag = "titanRespawn")
	{
		if (LogicLoaded && CustomLevelLoaded)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag(tag);
			GameObject gameObject = array[UnityEngine.Random.Range(0, array.Length)];
			Vector3 position = gameObject.transform.position;
			if (Level.Name.StartsWith("Custom") && titanSpawns.Count > 0)
			{
				position = titanSpawns[UnityEngine.Random.Range(0, titanSpawns.Count)];
			}
			myLastHero = id.ToUpper();
			GameObject gameObject2 = ((IN_GAME_MAIN_CAMERA.Gamemode != GameMode.PvPCapture) ? PhotonNetwork.Instantiate("TITAN_VER3.1", position, gameObject.transform.rotation, 0) : PhotonNetwork.Instantiate("TITAN_VER3.1", checkpoint.transform.position + new Vector3(UnityEngine.Random.Range(-20, 20), 2f, UnityEngine.Random.Range(-20, 20)), checkpoint.transform.rotation, 0));
			mainCamera.SetMainObjectTitan(gameObject2);
			gameObject2.GetComponent<TITAN>().nonAI = true;
			gameObject2.GetComponent<TITAN>().speed = 30f;
			gameObject2.GetComponent<TITAN_CONTROLLER>().enabled = true;
			if (id == "RANDOM" && UnityEngine.Random.Range(0, 100) < 7)
			{
				gameObject2.GetComponent<TITAN>().setAbnormalType2(TitanClass.Crawler, forceCrawler: true);
			}
			mainCamera.enabled = true;
			GameObject obj = GameObject.Find("MainCamera");
			obj.GetComponent<SpectatorMovement>().disable = true;
			obj.GetComponent<MouseLook>().disable = true;
			mainCamera.gameOver = false;
			ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
			{
				{ "dead", false },
				{
					PhotonPlayerProperty.IsTitan,
					2
				}
			};
			PhotonNetwork.player.SetCustomProperties(propertiesToSet);
			Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode == CameraType.TPS;
			Screen.showCursor = true;
			SetTextCenter(string.Empty);
		}
		else
		{
			NOTSpawnNonAITitanRC(id);
		}
	}

	public void FinishGame(bool isLoss = false)
	{
		if (isLosing || isWinning || (IN_GAME_MAIN_CAMERA.Gametype != 0 && !PhotonNetwork.isMasterClient))
		{
			return;
		}
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && GuardianClient.Properties.AnnounceRoundTime.Value)
		{
			float num = (float)(GameHelper.CurrentTimeMillis() - RoundStartTime) / 1000f;
			GameHelper.Broadcast($"This round lasted for <b>{num}</b> second(s)!");
		}
		if (isLoss)
		{
			isLosing = true;
			titanScore++;
			gameEndCD = gameEndTotalCDtime;
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
			{
				base.photonView.RPC("netGameLose", PhotonTargets.Others, titanScore);
				if ((int)Settings[244] == 1)
				{
					InRoomChat.Instance.AddLine(("(" + roundTime.ToString("F2") + ") ").AsColor("FFCC00") + "Round ended (game lose).");
				}
			}
			return;
		}
		isWinning = true;
		humanScore++;
		int num2;
		switch (IN_GAME_MAIN_CAMERA.Gamemode)
		{
		case GameMode.Racing:
			num2 = 0;
			gameEndCD = ((RCSettings.RacingStatic == 1) ? 1000f : 20f);
			break;
		case GameMode.TeamDeathmatch:
			teamScores[teamWinner - 1]++;
			num2 = teamWinner;
			gameEndCD = gameEndTotalCDtime;
			break;
		default:
			humanScore++;
			num2 = humanScore;
			gameEndCD = gameEndTotalCDtime;
			break;
		}
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
		{
			base.photonView.RPC("netGameWin", PhotonTargets.Others, num2);
			if ((int)Settings[244] == 1)
			{
				InRoomChat.Instance.AddLine(("(" + roundTime.ToString("F2") + ") ").AsColor("FFCC00") + "Round ended (game win).");
			}
		}
	}

	public bool AreAllPlayersDead()
	{
		PhotonPlayer[] array = PhotonNetwork.playerList;
		foreach (PhotonPlayer photonPlayer in array)
		{
			if (!photonPlayer.IsTitan && !photonPlayer.IsDead)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsTeamDead(int team)
	{
		PhotonPlayer[] array = PhotonNetwork.playerList;
		foreach (PhotonPlayer photonPlayer in array)
		{
			if (!photonPlayer.IsTitan && photonPlayer.Team == team && !photonPlayer.IsDead)
			{
				return false;
			}
		}
		return true;
	}

	private void RefreshRacingResult()
	{
		localRacingResult = "Result\n";
		IComparer comparer = new RacingResultComparer();
		this.racingResult.Sort(comparer);
		int num = Mathf.Min(this.racingResult.Count, 10);
		for (int i = 0; i < num; i++)
		{
			RacingResult racingResult = this.racingResult[i] as RacingResult;
			localRacingResult = localRacingResult + "[FFFFFF]#" + (i + 1) + ": " + racingResult.name + " - " + racingResult.time * 100f * 0.01f + "s\n";
		}
		base.photonView.RPC("netRefreshRacingResult", PhotonTargets.All, localRacingResult);
	}

	public void RestartGameSingle()
	{
		startRacing = false;
		endRacing = false;
		checkpoint = null;
		single_kills = 0;
		single_maxDamage = 0;
		single_totalDamage = 0;
		timeElapse = 0f;
		roundTime = 0f;
		timeTotalServer = 0f;
		isWinning = false;
		isLosing = false;
		wave = 1;
		myRespawnTime = 0f;
		SetTextCenter(string.Empty);
		DestroyAllExistingCloths();
		Application.LoadLevel(Application.loadedLevel);
	}

	public IEnumerator CoWaitAndRespawn1(float time, string str)
	{
		yield return new WaitForSeconds(time);
		SpawnPlayer(myLastHero, str);
	}

	public IEnumerator CoWaitAndRespawn2(float time, GameObject pos)
	{
		yield return new WaitForSeconds(time);
		SpawnPlayerAt2(myLastHero, pos);
	}

	public void DestroyAllExistingCloths()
	{
		Cloth[] array = UnityEngine.Object.FindObjectsOfType<Cloth>();
		for (int i = 0; i < array.Length; i++)
		{
			ClothFactory.DisposeObject(array[i].gameObject);
		}
	}

	public IEnumerator CoWaitAndResetRestarts()
	{
		yield return new WaitForSeconds(10f);
		restartingBomb = false;
		restartingEren = false;
		restartingHorse = false;
		restartingMC = false;
		restartingTitan = false;
	}

	public IEnumerator CoWaitAndReloadKD(PhotonPlayer player)
	{
		yield return new WaitForSeconds(5f);
		string key = GExtensions.AsString(player.customProperties[PhotonPlayerProperty.Name]);
		if (PreservedPlayerKDR.ContainsKey(key))
		{
			int[] array = PreservedPlayerKDR[key];
			PreservedPlayerKDR.Remove(key);
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add(PhotonPlayerProperty.Kills, array[0]);
			hashtable.Add(PhotonPlayerProperty.Deaths, array[1]);
			hashtable.Add(PhotonPlayerProperty.MaxDamage, array[2]);
			hashtable.Add(PhotonPlayerProperty.TotalDamage, array[3]);
			player.SetCustomProperties(hashtable);
		}
	}

	public void EnterSpecMode(bool enter)
	{
		if (enter)
		{
			spectateSprites = new List<GameObject>();
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = (GameObject)array[i];
				if (!(gameObject.GetComponent<UISprite>() != null) || !gameObject.activeInHierarchy)
				{
					continue;
				}
				string text = gameObject.name;
				if (text.Contains("blade") || text.Contains("bullet") || text.Contains("gas") || text.Contains("flare") || text.Contains("skill_cd"))
				{
					if (!spectateSprites.Contains(gameObject))
					{
						spectateSprites.Add(gameObject);
					}
					gameObject.SetActive(value: false);
				}
			}
			string[] array2 = new string[2] { "Flare", "LabelInfoBottomRight" };
			for (int i = 0; i < array2.Length; i++)
			{
				GameObject gameObject2 = GameObject.Find(array2[i]);
				if (gameObject2 != null)
				{
					if (!spectateSprites.Contains(gameObject2))
					{
						spectateSprites.Add(gameObject2);
					}
					gameObject2.SetActive(value: false);
				}
			}
			foreach (HERO hero in heroes)
			{
				if (hero.photonView.isMine)
				{
					PhotonNetwork.Destroy(hero.photonView);
				}
			}
			if (GExtensions.AsInt(PhotonNetwork.player.customProperties[PhotonPlayerProperty.IsTitan]) == 2 && !GExtensions.AsBool(PhotonNetwork.player.customProperties[PhotonPlayerProperty.IsDead]))
			{
				foreach (TITAN titan in titans)
				{
					if (titan.photonView.isMine && titan.nonAI)
					{
						PhotonNetwork.Destroy(titan.photonView);
					}
				}
			}
			NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], state: false);
			NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], state: false);
			NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], state: false);
			needChooseSide = false;
			mainCamera.enabled = true;
			if (IN_GAME_MAIN_CAMERA.CameraMode == CameraType.Original)
			{
				Screen.lockCursor = false;
				Screen.showCursor = false;
			}
			GameObject gameObject3 = GameObject.FindGameObjectWithTag("Player");
			if (gameObject3 != null && gameObject3.GetComponent<HERO>() != null)
			{
				mainCamera.SetMainObject(gameObject3);
			}
			else
			{
				mainCamera.SetMainObject(null);
			}
			mainCamera.SetSpectorMode(val: false);
			mainCamera.gameOver = true;
			Screen.lockCursor = !Screen.lockCursor;
			Screen.lockCursor = !Screen.lockCursor;
			return;
		}
		if (GameObject.Find("cross1") != null)
		{
			GameObject.Find("cross1").transform.localPosition = Vector3.up * 5000f;
		}
		if (spectateSprites != null)
		{
			foreach (GameObject spectateSprite in spectateSprites)
			{
				if (spectateSprite != null)
				{
					spectateSprite.SetActive(value: true);
				}
			}
		}
		spectateSprites = new List<GameObject>();
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[1], state: false);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[2], state: false);
		NGUITools.SetActive(ui.GetComponent<UIReferArray>().panels[3], state: false);
		needChooseSide = true;
		mainCamera.SetMainObject(null);
		mainCamera.SetSpectorMode(val: true);
		mainCamera.gameOver = true;
	}

	public void RecompilePlayerList(float time)
	{
		if (!isRecompiling)
		{
			isRecompiling = true;
			StartCoroutine(CoWaitAndRecompilePlayerList(time));
		}
	}

	private string GetPlayerTextForList(PhotonPlayer player)
	{
		string text = "[FFFFFF]";
		if (IgnoreList.Contains(player.Id))
		{
			text += "[990000]X[-] ";
		}
		text = (player.isMasterClient ? (text + "[AAFF00]") : ((!player.isLocal) ? (text + "[FFCC00]") : (text + "[0099FF]")));
		text = text + player.Id + " ";
		bool flag = false;
		if (player.customProperties[PhotonPlayerProperty.IsDead] == null)
		{
			text += ((player.Id < 0) ? "[FFCC00](Joining) " : "[FF0000](Invis) ");
		}
		else
		{
			flag = GExtensions.AsBool(player.customProperties[PhotonPlayerProperty.IsDead]);
		}
		text = ((GExtensions.AsInt(player.customProperties[PhotonPlayerProperty.IsTitan]) == 2) ? (text + "[" + (flag ? ColorSet.Red : ColorSet.TitanPlayer) + "][T] ") : ((GExtensions.AsInt(player.customProperties[PhotonPlayerProperty.Team]) != 2) ? (text + "[" + (flag ? ColorSet.Red : ColorSet.Human) + "][H] ") : (text + "[" + (flag ? ColorSet.Red : ColorSet.AHSS) + "][A] ")));
		text = text + "[FFFFFF]" + GExtensions.AsString(player.customProperties[PhotonPlayerProperty.Name]);
		int num = GExtensions.AsInt(player.customProperties[PhotonPlayerProperty.Kills]);
		int num2 = GExtensions.AsInt(player.customProperties[PhotonPlayerProperty.Deaths]);
		int num3 = GExtensions.AsInt(player.customProperties[PhotonPlayerProperty.MaxDamage]);
		int num4 = GExtensions.AsInt(player.customProperties[PhotonPlayerProperty.TotalDamage]);
		text = string.Concat(text, " [AAAAAA]:[FFFFFF] " + num + " / " + num2 + " / " + num3 + " / " + num4);
		if (GuardianClient.Properties.ShowPlayerMods.Value)
		{
			List<string> mods = ModDetector.GetMods(player);
			if (mods.Count > 0)
			{
				text = text + " " + mods[0];
			}
		}
		if (GuardianClient.Properties.ShowPlayerPings.Value && player.Ping >= 0)
		{
			text = text + " [FFFFFF][" + player.Ping + "ms]";
		}
		return text + "\n";
	}

	public IEnumerator CoWaitAndRecompilePlayerList(float time)
	{
		yield return new WaitForSeconds(time);
		string empty = string.Empty;
		Comparison<PhotonPlayer> comparator = (PhotonPlayer p1, PhotonPlayer p2) => p1.Id - p2.Id;
		PhotonPlayer[] array = PhotonNetwork.playerList.Sorted(comparator);
		if (RCSettings.TeamMode != 0)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			Dictionary<int, PhotonPlayer> dictionary = new Dictionary<int, PhotonPlayer>();
			Dictionary<int, PhotonPlayer> dictionary2 = new Dictionary<int, PhotonPlayer>();
			Dictionary<int, PhotonPlayer> dictionary3 = new Dictionary<int, PhotonPlayer>();
			PhotonPlayer[] array2 = array;
			foreach (PhotonPlayer photonPlayer in array2)
			{
				if (photonPlayer.customProperties[PhotonPlayerProperty.IsDead] != null && !IgnoreList.Contains(photonPlayer.Id))
				{
					switch (GExtensions.AsInt(photonPlayer.customProperties[PhotonPlayerProperty.RCTeam]))
					{
					case 0:
						dictionary3.Add(photonPlayer.Id, photonPlayer);
						break;
					case 1:
						dictionary.Add(photonPlayer.Id, photonPlayer);
						num += GExtensions.AsInt(photonPlayer.customProperties[PhotonPlayerProperty.Kills]);
						num3 += GExtensions.AsInt(photonPlayer.customProperties[PhotonPlayerProperty.Deaths]);
						num5 += GExtensions.AsInt(photonPlayer.customProperties[PhotonPlayerProperty.MaxDamage]);
						num7 += GExtensions.AsInt(photonPlayer.customProperties[PhotonPlayerProperty.TotalDamage]);
						break;
					case 2:
						dictionary2.Add(photonPlayer.Id, photonPlayer);
						num2 += GExtensions.AsInt(photonPlayer.customProperties[PhotonPlayerProperty.Kills]);
						num4 += GExtensions.AsInt(photonPlayer.customProperties[PhotonPlayerProperty.Deaths]);
						num6 += GExtensions.AsInt(photonPlayer.customProperties[PhotonPlayerProperty.MaxDamage]);
						num8 += GExtensions.AsInt(photonPlayer.customProperties[PhotonPlayerProperty.TotalDamage]);
						break;
					}
				}
			}
			cyanKills = num;
			magentaKills = num2;
			if (PhotonNetwork.isMasterClient)
			{
				if (RCSettings.TeamMode == 2)
				{
					array2 = array;
					foreach (PhotonPlayer photonPlayer2 in array2)
					{
						int num9 = 0;
						if (dictionary.Count > dictionary2.Count + 1)
						{
							num9 = 2;
							if (dictionary.ContainsKey(photonPlayer2.Id))
							{
								dictionary.Remove(photonPlayer2.Id);
							}
							if (!dictionary2.ContainsKey(photonPlayer2.Id))
							{
								dictionary2.Add(photonPlayer2.Id, photonPlayer2);
							}
						}
						else if (dictionary2.Count > dictionary.Count + 1)
						{
							num9 = 1;
							if (!dictionary.ContainsKey(photonPlayer2.Id))
							{
								dictionary.Add(photonPlayer2.Id, photonPlayer2);
							}
							if (dictionary2.ContainsKey(photonPlayer2.Id))
							{
								dictionary2.Remove(photonPlayer2.Id);
							}
						}
						if (num9 > 0)
						{
							base.photonView.RPC("setTeamRPC", photonPlayer2, num9);
						}
					}
				}
				else if (RCSettings.TeamMode == 3)
				{
					array2 = array;
					foreach (PhotonPlayer photonPlayer3 in array2)
					{
						int num10 = 0;
						int num11 = GExtensions.AsInt(photonPlayer3.customProperties[PhotonPlayerProperty.RCTeam]);
						if (num11 <= 0)
						{
							continue;
						}
						switch (num11)
						{
						case 1:
						{
							int num13 = GExtensions.AsInt(photonPlayer3.customProperties[PhotonPlayerProperty.Kills]);
							if (num2 + num13 + 7 < num - num13)
							{
								num10 = 2;
								num2 += num13;
								num -= num13;
							}
							break;
						}
						case 2:
						{
							int num12 = GExtensions.AsInt(photonPlayer3.customProperties[PhotonPlayerProperty.Kills]);
							if (num + num12 + 7 < num2 - num12)
							{
								num10 = 1;
								num += num12;
								num2 -= num12;
							}
							break;
						}
						}
						if (num10 > 0)
						{
							base.photonView.RPC("setTeamRPC", photonPlayer3, num10);
						}
					}
				}
			}
			empty = empty + "[00FFFF]TEAM CYAN[FFFFFF]: " + cyanKills + " [AAAAAA]/[-] " + num3 + " [AAAAAA]/[-] " + num5 + " [AAAAAA]/[-] " + num7 + "\n";
			foreach (PhotonPlayer value in dictionary.Values)
			{
				if (GExtensions.AsInt(value.customProperties[PhotonPlayerProperty.RCTeam]) == 1)
				{
					empty += GetPlayerTextForList(value);
				}
			}
			empty = empty + " \n[FF00FF]TEAM MAGENTA[FFFFFF]: " + magentaKills + " [AAAAAA]/[-] " + num4 + " [AAAAAA]/[-] " + num6 + " [AAAAAA]/[-] " + num8 + "\n";
			foreach (PhotonPlayer value2 in dictionary2.Values)
			{
				if (GExtensions.AsInt(value2.customProperties[PhotonPlayerProperty.RCTeam]) == 2)
				{
					empty += GetPlayerTextForList(value2);
				}
			}
			empty += " \n[00FF00]INDIVIDUAL\n";
			foreach (PhotonPlayer value3 in dictionary3.Values)
			{
				if (GExtensions.AsInt(value3.customProperties[PhotonPlayerProperty.RCTeam]) == 0)
				{
					empty += GetPlayerTextForList(value3);
				}
			}
		}
		else
		{
			empty += GetPlayerTextForList(PhotonNetwork.player);
			PhotonPlayer[] array2 = PhotonNetwork.otherPlayers.Sorted(comparator);
			foreach (PhotonPlayer player in array2)
			{
				empty += GetPlayerTextForList(player);
			}
		}
		playerList = empty;
		if (PhotonNetwork.isMasterClient && !isWinning && !isLosing && roundTime >= 5f)
		{
			if (RCSettings.InfectionMode > 0)
			{
				int num14 = 0;
				for (int j = 0; j < PhotonNetwork.playerList.Length; j++)
				{
					PhotonPlayer photonPlayer4 = PhotonNetwork.playerList[j];
					if (IgnoreList.Contains(photonPlayer4.Id) || photonPlayer4.customProperties[PhotonPlayerProperty.IsDead] == null || photonPlayer4.customProperties[PhotonPlayerProperty.IsTitan] == null)
					{
						continue;
					}
					if (GExtensions.AsInt(photonPlayer4.customProperties[PhotonPlayerProperty.IsTitan]) == 1)
					{
						if (GExtensions.AsBool(photonPlayer4.customProperties[PhotonPlayerProperty.IsDead]) && GExtensions.AsInt(photonPlayer4.customProperties[PhotonPlayerProperty.Deaths]) > 0)
						{
							if (!ImATitan.ContainsKey(photonPlayer4.Id))
							{
								ImATitan.Add(photonPlayer4.Id, 2);
							}
							ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
							hashtable.Add(PhotonPlayerProperty.IsTitan, 2);
							photonPlayer4.SetCustomProperties(hashtable);
							base.photonView.RPC("spawnTitanRPC", photonPlayer4);
						}
						else
						{
							if (!ImATitan.ContainsKey(photonPlayer4.Id))
							{
								continue;
							}
							foreach (HERO hero in heroes)
							{
								if (hero.photonView.owner == photonPlayer4)
								{
									hero.MarkDead();
									hero.photonView.RPC("netDie2", PhotonTargets.All, -1, "No Switching.");
								}
							}
						}
					}
					else if (GExtensions.AsInt(photonPlayer4.customProperties[PhotonPlayerProperty.IsTitan]) == 2 && !GExtensions.AsBool(photonPlayer4.customProperties[PhotonPlayerProperty.IsDead]))
					{
						num14++;
					}
				}
				if (num14 <= 0 && IN_GAME_MAIN_CAMERA.Gamemode != 0)
				{
					FinishGame();
				}
			}
			else if (RCSettings.PointMode > 0)
			{
				if (RCSettings.TeamMode > 0)
				{
					if (cyanKills >= RCSettings.PointMode)
					{
						base.photonView.RPC("Chat", PhotonTargets.All, "Team Cyan wins!".AsColor("00FFFF"), string.Empty);
						FinishGame();
					}
					else if (magentaKills >= RCSettings.PointMode)
					{
						base.photonView.RPC("Chat", PhotonTargets.All, "Team Magenta wins!".AsColor("FF00FF"), string.Empty);
						FinishGame();
					}
				}
				else if (RCSettings.TeamMode == 0)
				{
					for (int k = 0; k < PhotonNetwork.playerList.Length; k++)
					{
						PhotonPlayer photonPlayer5 = PhotonNetwork.playerList[k];
						if (GExtensions.AsInt(photonPlayer5.customProperties[PhotonPlayerProperty.Kills]) >= RCSettings.PointMode)
						{
							base.photonView.RPC("Chat", PhotonTargets.All, GExtensions.AsString(photonPlayer5.customProperties[PhotonPlayerProperty.Name]).NGUIToUnity() + " wins!".AsColor("FFCC00"), string.Empty);
							FinishGame();
						}
					}
				}
			}
			else if (RCSettings.PointMode <= 0 && (RCSettings.BombMode == 1 || RCSettings.PvPMode > 0))
			{
				if (RCSettings.TeamMode > 0 && PhotonNetwork.playerList.Length > 1)
				{
					int num15 = 0;
					int num16 = 0;
					int num17 = 0;
					int num18 = 0;
					for (int l = 0; l < PhotonNetwork.playerList.Length; l++)
					{
						PhotonPlayer photonPlayer6 = PhotonNetwork.playerList[l];
						if (IgnoreList.Contains(photonPlayer6.Id) || photonPlayer6.customProperties[PhotonPlayerProperty.RCTeam] == null || photonPlayer6.customProperties[PhotonPlayerProperty.IsDead] == null)
						{
							continue;
						}
						if (GExtensions.AsInt(photonPlayer6.customProperties[PhotonPlayerProperty.RCTeam]) == 1)
						{
							num17++;
							if (!GExtensions.AsBool(photonPlayer6.customProperties[PhotonPlayerProperty.IsDead]))
							{
								num15++;
							}
						}
						else if (GExtensions.AsInt(photonPlayer6.customProperties[PhotonPlayerProperty.RCTeam]) == 2)
						{
							num18++;
							if (!GExtensions.AsBool(photonPlayer6.customProperties[PhotonPlayerProperty.IsDead]))
							{
								num16++;
							}
						}
					}
					if (num17 > 0 && num18 > 0)
					{
						if (num15 == 0)
						{
							base.photonView.RPC("Chat", PhotonTargets.All, "Team Magenta wins!".AsColor("FF00FF"), string.Empty);
							FinishGame();
						}
						else if (num16 == 0)
						{
							base.photonView.RPC("Chat", PhotonTargets.All, "Team Cyan wins!".AsColor("00FFFF"), string.Empty);
							FinishGame();
						}
					}
				}
				else if (RCSettings.TeamMode == 0 && PhotonNetwork.playerList.Length > 1)
				{
					int num19 = 0;
					string str = "Nobody";
					_ = PhotonNetwork.playerList[0];
					for (int m = 0; m < PhotonNetwork.playerList.Length; m++)
					{
						PhotonPlayer photonPlayer7 = PhotonNetwork.playerList[m];
						if (photonPlayer7.customProperties[PhotonPlayerProperty.IsDead] != null && !GExtensions.AsBool(photonPlayer7.customProperties[PhotonPlayerProperty.IsDead]))
						{
							str = GExtensions.AsString(photonPlayer7.customProperties[PhotonPlayerProperty.Name]).NGUIToUnity();
							num19++;
						}
					}
					if (num19 <= 1)
					{
						base.photonView.RPC("Chat", PhotonTargets.All, str.NGUIToUnity() + " wins.".AsColor("FFCC00"), string.Empty);
						FinishGame();
					}
				}
			}
		}
		isRecompiling = false;
	}

	public void UnloadAssets()
	{
		if (!isUnloading)
		{
			isUnloading = true;
			StartCoroutine(CoWaitAndUnloadAssets(10f));
		}
	}

	public void UnloadAssetsEditor()
	{
		if (!isUnloading)
		{
			isUnloading = true;
			StartCoroutine(CoWaitAndUnloadAssets(30f));
		}
	}

	public IEnumerator CoWaitAndUnloadAssets(float time)
	{
		yield return new WaitForSeconds(time);
		Resources.UnloadUnusedAssets();
		isUnloading = false;
	}

	public Texture2D LoadTextureRC(string tex)
	{
		if (assetCacheTextures == null)
		{
			assetCacheTextures = new Dictionary<string, Texture2D>();
		}
		if (assetCacheTextures.ContainsKey(tex))
		{
			return assetCacheTextures[tex];
		}
		Texture2D texture2D = (Texture2D)RCAssets.Load(tex);
		assetCacheTextures.Add(tex, texture2D);
		return texture2D;
	}

	[RPC]
	public void verifyPlayerHasLeft(int id, PhotonMessageInfo info)
	{
		if (info.sender.isMasterClient)
		{
			PhotonPlayer photonPlayer = PhotonPlayer.Find(id);
			if (photonPlayer != null)
			{
				string value = GExtensions.AsString(photonPlayer.customProperties[PhotonPlayerProperty.Name]);
				BanHash.Add(id, value);
			}
		}
	}

	public static void ServerRequestAuthentication(string authPassword)
	{
		if (!string.IsNullOrEmpty(authPassword))
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = authPassword;
			PhotonNetwork.RaiseEvent(198, hashtable, sendReliable: true, new RaiseEventOptions());
		}
	}

	public static void ServerCloseConnection(PhotonPlayer targetPlayer, bool requestIpBan, string inGameName = null)
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.TargetActors = new int[1] { targetPlayer.Id };
		RaiseEventOptions options = raiseEventOptions;
		if (requestIpBan)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = true;
			if (inGameName != null && inGameName.Length > 0)
			{
				hashtable[(byte)1] = inGameName;
			}
			PhotonNetwork.RaiseEvent(203, hashtable, sendReliable: true, options);
		}
		else
		{
			PhotonNetwork.RaiseEvent(203, null, sendReliable: true, options);
		}
	}

	public static void ServerRequestUnban(string bannedAddress)
	{
		if (!string.IsNullOrEmpty(bannedAddress))
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[(byte)0] = bannedAddress;
			PhotonNetwork.RaiseEvent(199, hashtable, sendReliable: true, new RaiseEventOptions());
		}
	}

	private ExitGames.Client.Photon.Hashtable CheckGameGUI()
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		if ((int)Settings[200] > 0)
		{
			Settings[192] = 0;
			Settings[193] = 0;
			Settings[226] = 0;
			Settings[220] = 0;
			int result = 1;
			if (!int.TryParse((string)Settings[201], out result) || result > PhotonNetwork.countOfPlayers || result < 0)
			{
				Settings[201] = "1";
			}
			hashtable.Add("infection", result);
			if (RCSettings.InfectionMode != result)
			{
				ImATitan.Clear();
				for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
				{
					PhotonNetwork.playerList[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
					{
						PhotonPlayerProperty.IsTitan,
						1
					} });
				}
				int num = PhotonNetwork.playerList.Length;
				int num2 = result;
				for (int j = 0; j < PhotonNetwork.playerList.Length; j++)
				{
					PhotonPlayer photonPlayer = PhotonNetwork.playerList[j];
					if (num > 0 && UnityEngine.Random.Range(0f, 1f) <= (float)num2 / (float)num)
					{
						ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable();
						hashtable2.Add(PhotonPlayerProperty.IsTitan, 2);
						photonPlayer.SetCustomProperties(hashtable2);
						ImATitan.Add(photonPlayer.Id, 2);
						num2--;
					}
					num--;
				}
			}
		}
		if ((int)Settings[192] > 0)
		{
			hashtable.Add("bomb", (int)Settings[192]);
		}
		if ((int)Settings[235] > 0)
		{
			hashtable.Add("globalDisableMinimap", (int)Settings[235]);
		}
		if ((int)Settings[193] > 0)
		{
			hashtable.Add("team", (int)Settings[193]);
			if (RCSettings.TeamMode != (int)Settings[193])
			{
				int num3 = 1;
				for (int k = 0; k < PhotonNetwork.playerList.Length; k++)
				{
					PhotonPlayer targetPlayer = PhotonNetwork.playerList[k];
					switch (num3)
					{
					case 1:
						base.photonView.RPC("setTeamRPC", targetPlayer, 1);
						num3 = 2;
						break;
					case 2:
						base.photonView.RPC("setTeamRPC", targetPlayer, 2);
						num3 = 1;
						break;
					}
				}
			}
		}
		if ((int)Settings[226] > 0)
		{
			int result2 = 50;
			if (!int.TryParse((string)Settings[227], out result2) || result2 > 1000 || result2 < 0)
			{
				Settings[227] = "50";
			}
			hashtable.Add("point", result2);
		}
		if ((int)Settings[194] > 0)
		{
			hashtable.Add("rock", (int)Settings[194]);
		}
		if ((int)Settings[195] > 0)
		{
			int result3 = 30;
			if (!int.TryParse((string)Settings[196], out result3) || result3 > 100 || result3 < 0)
			{
				Settings[196] = "30";
			}
			hashtable.Add("explode", result3);
		}
		if ((int)Settings[197] > 0)
		{
			int result4 = 100;
			int result5 = 200;
			if (!int.TryParse((string)Settings[198], out result4) || result4 > 100000 || result4 < 0)
			{
				Settings[198] = "100";
			}
			if (!int.TryParse((string)Settings[199], out result5) || result5 > 100000 || result5 < 0)
			{
				Settings[199] = "200";
			}
			hashtable.Add("healthMode", (int)Settings[197]);
			hashtable.Add("healthLower", result4);
			hashtable.Add("healthUpper", result5);
		}
		if ((int)Settings[202] > 0)
		{
			hashtable.Add("eren", (int)Settings[202]);
		}
		if ((int)Settings[203] > 0)
		{
			int result6 = 1;
			if (!int.TryParse((string)Settings[204], out result6) || result6 > 50 || result6 < 0)
			{
				Settings[204] = "1";
			}
			hashtable.Add("titanc", result6);
		}
		if ((int)Settings[205] > 0)
		{
			int result7 = 1000;
			if (!int.TryParse((string)Settings[206], out result7) || result7 > 100000 || result7 < 0)
			{
				Settings[206] = "1000";
			}
			hashtable.Add("damage", result7);
		}
		if ((int)Settings[207] > 0)
		{
			float result8 = 1f;
			float result9 = 3f;
			if (!float.TryParse((string)Settings[208], out result8) || !(result8 <= 100f) || !(result8 >= 0f))
			{
				Settings[208] = "1.0";
			}
			if (!float.TryParse((string)Settings[209], out result9) || !(result9 <= 100f) || !(result9 >= 0f))
			{
				Settings[209] = "3.0";
			}
			hashtable.Add("sizeMode", (int)Settings[207]);
			hashtable.Add("sizeLower", result8);
			hashtable.Add("sizeUpper", result9);
		}
		if ((int)Settings[210] > 0)
		{
			float result10 = 20f;
			float result11 = 20f;
			float result12 = 20f;
			float result13 = 20f;
			float result14 = 20f;
			if (!float.TryParse((string)Settings[211], out result10) || !(result10 >= 0f))
			{
				Settings[211] = "20.0";
			}
			if (!float.TryParse((string)Settings[212], out result11) || !(result11 >= 0f))
			{
				Settings[212] = "20.0";
			}
			if (!float.TryParse((string)Settings[213], out result12) || !(result12 >= 0f))
			{
				Settings[213] = "20.0";
			}
			if (!float.TryParse((string)Settings[214], out result13) || !(result13 >= 0f))
			{
				Settings[214] = "20.0";
			}
			if (!float.TryParse((string)Settings[215], out result14) || !(result14 >= 0f))
			{
				Settings[215] = "20.0";
			}
			if (result10 + result11 + result12 + result13 + result14 > 100f)
			{
				Settings[211] = "20.0";
				Settings[212] = "20.0";
				Settings[213] = "20.0";
				Settings[214] = "20.0";
				Settings[215] = "20.0";
				result10 = 20f;
				result11 = 20f;
				result12 = 20f;
				result13 = 20f;
				result14 = 20f;
			}
			hashtable.Add("spawnMode", (int)Settings[210]);
			hashtable.Add("nRate", result10);
			hashtable.Add("aRate", result11);
			hashtable.Add("jRate", result12);
			hashtable.Add("cRate", result13);
			hashtable.Add("pRate", result14);
		}
		if ((int)Settings[216] > 0)
		{
			hashtable.Add("horse", (int)Settings[216]);
		}
		if ((int)Settings[217] > 0)
		{
			int result15 = 1;
			if (!int.TryParse((string)Settings[218], out result15) || result15 > 50)
			{
				Settings[218] = "1";
			}
			hashtable.Add("waveModeOn", (int)Settings[217]);
			hashtable.Add("waveModeNum", result15);
		}
		if ((int)Settings[219] > 0)
		{
			hashtable.Add("friendly", (int)Settings[219]);
		}
		if ((int)Settings[220] > 0)
		{
			hashtable.Add("pvp", (int)Settings[220]);
		}
		if ((int)Settings[221] > 0)
		{
			int result16 = 20;
			if (!int.TryParse((string)Settings[222], out result16) || result16 > 1000000 || result16 < 0)
			{
				Settings[222] = "20";
			}
			hashtable.Add("maxwave", result16);
		}
		if ((int)Settings[223] > 0)
		{
			int result17 = 5;
			if (!int.TryParse((string)Settings[224], out result17) || result17 > 1000000 || result17 < 5)
			{
				Settings[224] = "5";
			}
			hashtable.Add("endless", result17);
		}
		if ((string)Settings[225] != string.Empty)
		{
			hashtable.Add("motd", (string)Settings[225]);
		}
		if ((int)Settings[228] > 0)
		{
			hashtable.Add("ahssReload", (int)Settings[228]);
		}
		if ((int)Settings[229] > 0)
		{
			hashtable.Add("punkWaves", (int)Settings[229]);
		}
		if ((int)Settings[261] > 0)
		{
			hashtable.Add("deadlycannons", (int)Settings[261]);
		}
		if (RCSettings.RacingStatic > 0)
		{
			hashtable.Add("asoracing", 1);
		}
		return hashtable;
	}

	private IEnumerator CoLogin()
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("userid", UsernameField);
		wWWForm.AddField("password", PasswordField);
		using (WWW www = new WWW("http://fenglee.com/game/aog/require_user_info.php", wWWForm))
		{
			yield return www;
			if (www.error == null && !www.text.Contains("Error,please sign in again."))
			{
				string[] array = www.text.Split('|');
				LoginFengKAI.Player.Name = UsernameField;
				LoginFengKAI.Player.Guild = array[0];
				LoginFengKAI.LoginState = LoginState.LoggedIn;
			}
			else
			{
				LoginFengKAI.LoginState = LoginState.Failed;
			}
		}
	}

	private IEnumerator CoSetGuild()
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("name", LoginFengKAI.Player.Name);
		wWWForm.AddField("guildname", LoginFengKAI.Player.Guild);
		using (WWW www = new WWW("http://fenglee.com/game/aog/change_guild_name.php", wWWForm))
		{
			yield return www;
			if (www.error != null)
			{
				UnityEngine.MonoBehaviour.print(www.error);
			}
		}
	}

	public void CompileScript(string str)
	{
		string[] array = str.Replace(" ", string.Empty).Split(new string[2] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		int num = 0;
		int num2 = 0;
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == "{")
			{
				num++;
				continue;
			}
			if (array[i] == "}")
			{
				num2++;
				continue;
			}
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			string text = array[i];
			for (int j = 0; j < text.Length; j++)
			{
				switch (text[j])
				{
				case '(':
					num3++;
					break;
				case ')':
					num4++;
					break;
				case '"':
					num5++;
					break;
				}
			}
			if (num3 != num4)
			{
				InRoomChat.Instance.AddLine("Script Error: Parentheses not equal! (line " + (i + 1) + ")");
				flag = true;
			}
			if (num5 % 2 != 0)
			{
				InRoomChat.Instance.AddLine("Script Error: Quotations not equal! (line " + (i + 1) + ")");
				flag = true;
			}
		}
		if (num != num2)
		{
			InRoomChat.Instance.AddLine("Script Error: Bracket count not equivalent!");
			flag = true;
		}
		if (flag)
		{
			return;
		}
		try
		{
			for (int k = 0; k < array.Length; k++)
			{
				if (!array[k].StartsWith("On") || !(array[k + 1] == "{"))
				{
					continue;
				}
				int num6 = k;
				int num7 = k + 2;
				int num8 = 0;
				for (int l = k + 2; l < array.Length; l++)
				{
					if (array[l] == "{")
					{
						num8++;
					}
					if (array[l] == "}")
					{
						if (num8 > 0)
						{
							num8--;
							continue;
						}
						num7 = l - 1;
						l = array.Length;
					}
				}
				hashtable.Add(num6, num7);
				k = num7;
			}
			foreach (int key in hashtable.Keys)
			{
				string text2 = array[key];
				int num10 = (int)hashtable[key];
				string[] array2 = new string[num10 - key + 1];
				int num11 = 0;
				for (int m = key; m <= num10; m++)
				{
					array2[num11] = array[m];
					num11++;
				}
				RCEvent rCEvent = ParseBlock(array2, 0, 0, null);
				if (text2.StartsWith("OnPlayerEnterRegion"))
				{
					int num12 = text2.IndexOf('[');
					int num13 = text2.IndexOf(']');
					string text3 = text2.Substring(num12 + 2, num13 - num12 - 3);
					num12 = text2.IndexOf('(');
					num13 = text2.IndexOf(')');
					string value = text2.Substring(num12 + 2, num13 - num12 - 3);
					if (RCRegionTriggers.ContainsKey(text3))
					{
						RegionTrigger regionTrigger = (RegionTrigger)RCRegionTriggers[text3];
						regionTrigger.playerEventEnter = rCEvent;
						regionTrigger.myName = text3;
						RCRegionTriggers[text3] = regionTrigger;
					}
					else
					{
						RegionTrigger regionTrigger2 = new RegionTrigger();
						regionTrigger2.playerEventEnter = rCEvent;
						regionTrigger2.myName = text3;
						RCRegionTriggers.Add(text3, regionTrigger2);
					}
					RCVariableNames.Add("OnPlayerEnterRegion[" + text3 + "]", value);
				}
				else if (text2.StartsWith("OnPlayerLeaveRegion"))
				{
					int num14 = text2.IndexOf('[');
					int num15 = text2.IndexOf(']');
					string text4 = text2.Substring(num14 + 2, num15 - num14 - 3);
					num14 = text2.IndexOf('(');
					num15 = text2.IndexOf(')');
					string value2 = text2.Substring(num14 + 2, num15 - num14 - 3);
					if (RCRegionTriggers.ContainsKey(text4))
					{
						RegionTrigger regionTrigger3 = (RegionTrigger)RCRegionTriggers[text4];
						regionTrigger3.playerEventExit = rCEvent;
						regionTrigger3.myName = text4;
						RCRegionTriggers[text4] = regionTrigger3;
					}
					else
					{
						RegionTrigger regionTrigger4 = new RegionTrigger();
						regionTrigger4.playerEventExit = rCEvent;
						regionTrigger4.myName = text4;
						RCRegionTriggers.Add(text4, regionTrigger4);
					}
					RCVariableNames.Add("OnPlayerExitRegion[" + text4 + "]", value2);
				}
				else if (text2.StartsWith("OnTitanEnterRegion"))
				{
					int num16 = text2.IndexOf('[');
					int num17 = text2.IndexOf(']');
					string text5 = text2.Substring(num16 + 2, num17 - num16 - 3);
					num16 = text2.IndexOf('(');
					num17 = text2.IndexOf(')');
					string value3 = text2.Substring(num16 + 2, num17 - num16 - 3);
					if (RCRegionTriggers.ContainsKey(text5))
					{
						RegionTrigger regionTrigger5 = (RegionTrigger)RCRegionTriggers[text5];
						regionTrigger5.titanEventEnter = rCEvent;
						regionTrigger5.myName = text5;
						RCRegionTriggers[text5] = regionTrigger5;
					}
					else
					{
						RegionTrigger regionTrigger6 = new RegionTrigger();
						regionTrigger6.titanEventEnter = rCEvent;
						regionTrigger6.myName = text5;
						RCRegionTriggers.Add(text5, regionTrigger6);
					}
					RCVariableNames.Add("OnTitanEnterRegion[" + text5 + "]", value3);
				}
				else if (text2.StartsWith("OnTitanLeaveRegion"))
				{
					int num18 = text2.IndexOf('[');
					int num19 = text2.IndexOf(']');
					string text6 = text2.Substring(num18 + 2, num19 - num18 - 3);
					num18 = text2.IndexOf('(');
					num19 = text2.IndexOf(')');
					string value4 = text2.Substring(num18 + 2, num19 - num18 - 3);
					if (RCRegionTriggers.ContainsKey(text6))
					{
						RegionTrigger regionTrigger7 = (RegionTrigger)RCRegionTriggers[text6];
						regionTrigger7.titanEventExit = rCEvent;
						regionTrigger7.myName = text6;
						RCRegionTriggers[text6] = regionTrigger7;
					}
					else
					{
						RegionTrigger regionTrigger8 = new RegionTrigger();
						regionTrigger8.titanEventExit = rCEvent;
						regionTrigger8.myName = text6;
						RCRegionTriggers.Add(text6, regionTrigger8);
					}
					RCVariableNames.Add("OnTitanExitRegion[" + text6 + "]", value4);
				}
				else if (text2.StartsWith("OnFirstLoad()"))
				{
					RCEvents.Add("OnFirstLoad", rCEvent);
				}
				else if (text2.StartsWith("OnRoundStart()"))
				{
					RCEvents.Add("OnRoundStart", rCEvent);
				}
				else if (text2.StartsWith("OnUpdate()"))
				{
					RCEvents.Add("OnUpdate", rCEvent);
				}
				else if (text2.StartsWith("OnTitanDie"))
				{
					int num20 = text2.IndexOf('(');
					int num21 = text2.LastIndexOf(')');
					string[] array3 = text2.Substring(num20 + 1, num21 - num20 - 1).Split(',');
					array3[0] = array3[0].Substring(1, array3[0].Length - 2);
					array3[1] = array3[1].Substring(1, array3[1].Length - 2);
					RCVariableNames.Add("OnTitanDie", array3);
					RCEvents.Add("OnTitanDie", rCEvent);
				}
				else if (text2.StartsWith("OnPlayerDieByTitan"))
				{
					RCEvents.Add("OnPlayerDieByTitan", rCEvent);
					int num22 = text2.IndexOf('(');
					int num23 = text2.LastIndexOf(')');
					string[] array4 = text2.Substring(num22 + 1, num23 - num22 - 1).Split(',');
					array4[0] = array4[0].Substring(1, array4[0].Length - 2);
					array4[1] = array4[1].Substring(1, array4[1].Length - 2);
					RCVariableNames.Add("OnPlayerDieByTitan", array4);
				}
				else if (text2.StartsWith("OnPlayerDieByPlayer"))
				{
					RCEvents.Add("OnPlayerDieByPlayer", rCEvent);
					int num24 = text2.IndexOf('(');
					int num25 = text2.LastIndexOf(')');
					string[] array5 = text2.Substring(num24 + 1, num25 - num24 - 1).Split(',');
					array5[0] = array5[0].Substring(1, array5[0].Length - 2);
					array5[1] = array5[1].Substring(1, array5[1].Length - 2);
					RCVariableNames.Add("OnPlayerDieByPlayer", array5);
				}
				else if (text2.StartsWith("OnChatInput"))
				{
					RCEvents.Add("OnChatInput", rCEvent);
					int num26 = text2.IndexOf('(');
					int num27 = text2.LastIndexOf(')');
					string text7 = text2.Substring(num26 + 1, num27 - num26 - 1);
					RCVariableNames.Add("OnChatInput", text7.Substring(1, text7.Length - 2));
				}
			}
		}
		catch (UnityException ex)
		{
			InRoomChat.Instance.AddLine(ex.Message);
		}
	}

	public RCEvent ParseBlock(string[] stringArray, int eventClass, int eventType, RCCondition condition)
	{
		List<RCAction> list = new List<RCAction>();
		RCEvent rCEvent = new RCEvent(null, null, 0, 0);
		for (int i = 0; i < stringArray.Length; i++)
		{
			if (stringArray[i].StartsWith("If") && stringArray[i + 1] == "{")
			{
				int num = i + 2;
				int num2 = i + 2;
				int num3 = 0;
				for (int j = i + 2; j < stringArray.Length; j++)
				{
					if (stringArray[j] == "{")
					{
						num3++;
					}
					if (stringArray[j] == "}")
					{
						if (num3 > 0)
						{
							num3--;
							continue;
						}
						num2 = j - 1;
						j = stringArray.Length;
					}
				}
				string[] array = new string[num2 - num + 1];
				int num4 = 0;
				for (int k = num; k <= num2; k++)
				{
					array[num4] = stringArray[k];
					num4++;
				}
				int num5 = stringArray[i].IndexOf("(");
				int num6 = stringArray[i].LastIndexOf(")");
				string text = stringArray[i].Substring(num5 + 1, num6 - num5 - 1);
				int num7 = ConditionType(text);
				int num8 = text.IndexOf('.');
				text = text.Substring(num8 + 1);
				int sentOperand = OperandType(text, num7);
				num5 = text.IndexOf('(');
				num6 = text.LastIndexOf(")");
				text = text.Substring(num5 + 1, num6 - num5 - 1);
				string[] array2 = text.Split(',');
				RCCondition condition2 = new RCCondition(sentOperand, num7, ReturnHelper(array2[0]), ReturnHelper(array2[1]));
				RCEvent rCEvent2 = ParseBlock(array, 1, 0, condition2);
				RCAction item = new RCAction(0, 0, rCEvent2, null);
				rCEvent = rCEvent2;
				list.Add(item);
				i = num2;
			}
			else if (stringArray[i].StartsWith("While") && stringArray[i + 1] == "{")
			{
				int num9 = i + 2;
				int num10 = i + 2;
				int num11 = 0;
				for (int l = i + 2; l < stringArray.Length; l++)
				{
					if (stringArray[l] == "{")
					{
						num11++;
					}
					if (stringArray[l] == "}")
					{
						if (num11 > 0)
						{
							num11--;
							continue;
						}
						num10 = l - 1;
						l = stringArray.Length;
					}
				}
				string[] array3 = new string[num10 - num9 + 1];
				int num12 = 0;
				for (int m = num9; m <= num10; m++)
				{
					array3[num12] = stringArray[m];
					num12++;
				}
				int num13 = stringArray[i].IndexOf("(");
				int num14 = stringArray[i].LastIndexOf(")");
				string text2 = stringArray[i].Substring(num13 + 1, num14 - num13 - 1);
				int num15 = ConditionType(text2);
				int num16 = text2.IndexOf('.');
				text2 = text2.Substring(num16 + 1);
				int sentOperand2 = OperandType(text2, num15);
				num13 = text2.IndexOf('(');
				num14 = text2.LastIndexOf(")");
				text2 = text2.Substring(num13 + 1, num14 - num13 - 1);
				string[] array4 = text2.Split(',');
				RCCondition condition3 = new RCCondition(sentOperand2, num15, ReturnHelper(array4[0]), ReturnHelper(array4[1]));
				RCEvent next = ParseBlock(array3, 3, 0, condition3);
				RCAction item2 = new RCAction(0, 0, next, null);
				list.Add(item2);
				i = num10;
			}
			else if (stringArray[i].StartsWith("ForeachTitan") && stringArray[i + 1] == "{")
			{
				int num17 = i + 2;
				int num18 = i + 2;
				int num19 = 0;
				for (int n = i + 2; n < stringArray.Length; n++)
				{
					if (stringArray[n] == "{")
					{
						num19++;
					}
					if (stringArray[n] == "}")
					{
						if (num19 > 0)
						{
							num19--;
							continue;
						}
						num18 = n - 1;
						n = stringArray.Length;
					}
				}
				string[] array5 = new string[num18 - num17 + 1];
				int num20 = 0;
				for (int num21 = num17; num21 <= num18; num21++)
				{
					array5[num20] = stringArray[num21];
					num20++;
				}
				int num22 = stringArray[i].IndexOf("(");
				int num23 = stringArray[i].LastIndexOf(")");
				string foreachVariableName = stringArray[i].Substring(num22 + 2, num23 - num22 - 3);
				int eventType2 = 0;
				RCEvent rCEvent3 = ParseBlock(array5, 2, eventType2, null);
				rCEvent3.foreachVariableName = foreachVariableName;
				RCAction item3 = new RCAction(0, 0, rCEvent3, null);
				list.Add(item3);
				i = num18;
			}
			else if (stringArray[i].StartsWith("ForeachPlayer") && stringArray[i + 1] == "{")
			{
				int num24 = i + 2;
				int num25 = i + 2;
				int num26 = 0;
				for (int num27 = i + 2; num27 < stringArray.Length; num27++)
				{
					if (stringArray[num27] == "{")
					{
						num26++;
					}
					if (stringArray[num27] == "}")
					{
						if (num26 > 0)
						{
							num26--;
							continue;
						}
						num25 = num27 - 1;
						num27 = stringArray.Length;
					}
				}
				string[] array6 = new string[num25 - num24 + 1];
				int num28 = 0;
				for (int num29 = num24; num29 <= num25; num29++)
				{
					array6[num28] = stringArray[num29];
					num28++;
				}
				int num30 = stringArray[i].IndexOf("(");
				int num31 = stringArray[i].LastIndexOf(")");
				string foreachVariableName2 = stringArray[i].Substring(num30 + 2, num31 - num30 - 3);
				int eventType3 = 1;
				RCEvent rCEvent4 = ParseBlock(array6, 2, eventType3, null);
				rCEvent4.foreachVariableName = foreachVariableName2;
				RCAction item4 = new RCAction(0, 0, rCEvent4, null);
				list.Add(item4);
				i = num25;
			}
			else if (stringArray[i].StartsWith("Else") && stringArray[i + 1] == "{")
			{
				int num32 = i + 2;
				int num33 = i + 2;
				int num34 = 0;
				for (int num35 = i + 2; num35 < stringArray.Length; num35++)
				{
					if (stringArray[num35] == "{")
					{
						num34++;
					}
					if (stringArray[num35] == "}")
					{
						if (num34 > 0)
						{
							num34--;
							continue;
						}
						num33 = num35 - 1;
						num35 = stringArray.Length;
					}
				}
				string[] array7 = new string[num33 - num32 + 1];
				int num36 = 0;
				for (int num37 = num32; num37 <= num33; num37++)
				{
					array7[num36] = stringArray[num37];
					num36++;
				}
				if (stringArray[i] == "Else")
				{
					RCEvent next2 = ParseBlock(array7, 0, 0, null);
					RCAction @else = new RCAction(0, 0, next2, null);
					rCEvent.SetElse(@else);
					i = num33;
				}
				else if (stringArray[i].StartsWith("Else If"))
				{
					int num38 = stringArray[i].IndexOf("(");
					int num39 = stringArray[i].LastIndexOf(")");
					string text3 = stringArray[i].Substring(num38 + 1, num39 - num38 - 1);
					int num40 = ConditionType(text3);
					int num41 = text3.IndexOf('.');
					text3 = text3.Substring(num41 + 1);
					int sentOperand3 = OperandType(text3, num40);
					num38 = text3.IndexOf('(');
					num39 = text3.LastIndexOf(")");
					text3 = text3.Substring(num38 + 1, num39 - num38 - 1);
					string[] array8 = text3.Split(',');
					RCCondition condition4 = new RCCondition(sentOperand3, num40, ReturnHelper(array8[0]), ReturnHelper(array8[1]));
					RCEvent next3 = ParseBlock(array7, 1, 0, condition4);
					RCAction else2 = new RCAction(0, 0, next3, null);
					rCEvent.SetElse(else2);
					i = num33;
				}
			}
			else if (stringArray[i].StartsWith("VariableInt"))
			{
				int category = 1;
				int num42 = stringArray[i].IndexOf('.');
				int num43 = stringArray[i].IndexOf('(');
				int num44 = stringArray[i].LastIndexOf(')');
				string text4 = stringArray[i].Substring(num42 + 1, num43 - num42 - 1);
				string[] array9 = stringArray[i].Substring(num43 + 1, num44 - num43 - 1).Split(',');
				if (text4.StartsWith("SetRandom"))
				{
					RCActionHelper rCActionHelper = ReturnHelper(array9[0]);
					RCActionHelper rCActionHelper2 = ReturnHelper(array9[1]);
					RCActionHelper rCActionHelper3 = ReturnHelper(array9[2]);
					RCAction item5 = new RCAction(category, 12, null, new RCActionHelper[3] { rCActionHelper, rCActionHelper2, rCActionHelper3 });
					list.Add(item5);
				}
				else if (text4.StartsWith("Set"))
				{
					RCActionHelper rCActionHelper4 = ReturnHelper(array9[0]);
					RCActionHelper rCActionHelper5 = ReturnHelper(array9[1]);
					RCAction item6 = new RCAction(category, 0, null, new RCActionHelper[2] { rCActionHelper4, rCActionHelper5 });
					list.Add(item6);
				}
				else if (text4.StartsWith("Add"))
				{
					RCActionHelper rCActionHelper6 = ReturnHelper(array9[0]);
					RCActionHelper rCActionHelper7 = ReturnHelper(array9[1]);
					RCAction item7 = new RCAction(category, 1, null, new RCActionHelper[2] { rCActionHelper6, rCActionHelper7 });
					list.Add(item7);
				}
				else if (text4.StartsWith("Subtract"))
				{
					RCActionHelper rCActionHelper8 = ReturnHelper(array9[0]);
					RCActionHelper rCActionHelper9 = ReturnHelper(array9[1]);
					RCAction item8 = new RCAction(category, 2, null, new RCActionHelper[2] { rCActionHelper8, rCActionHelper9 });
					list.Add(item8);
				}
				else if (text4.StartsWith("Multiply"))
				{
					RCActionHelper rCActionHelper10 = ReturnHelper(array9[0]);
					RCActionHelper rCActionHelper11 = ReturnHelper(array9[1]);
					RCAction item9 = new RCAction(category, 3, null, new RCActionHelper[2] { rCActionHelper10, rCActionHelper11 });
					list.Add(item9);
				}
				else if (text4.StartsWith("Divide"))
				{
					RCActionHelper rCActionHelper12 = ReturnHelper(array9[0]);
					RCActionHelper rCActionHelper13 = ReturnHelper(array9[1]);
					RCAction item10 = new RCAction(category, 4, null, new RCActionHelper[2] { rCActionHelper12, rCActionHelper13 });
					list.Add(item10);
				}
				else if (text4.StartsWith("Modulo"))
				{
					RCActionHelper rCActionHelper14 = ReturnHelper(array9[0]);
					RCActionHelper rCActionHelper15 = ReturnHelper(array9[1]);
					RCAction item11 = new RCAction(category, 5, null, new RCActionHelper[2] { rCActionHelper14, rCActionHelper15 });
					list.Add(item11);
				}
				else if (text4.StartsWith("Power"))
				{
					RCActionHelper rCActionHelper16 = ReturnHelper(array9[0]);
					RCActionHelper rCActionHelper17 = ReturnHelper(array9[1]);
					RCAction item12 = new RCAction(category, 6, null, new RCActionHelper[2] { rCActionHelper16, rCActionHelper17 });
					list.Add(item12);
				}
			}
			else if (stringArray[i].StartsWith("VariableBool"))
			{
				int category2 = 2;
				int num45 = stringArray[i].IndexOf('.');
				int num46 = stringArray[i].IndexOf('(');
				int num47 = stringArray[i].LastIndexOf(')');
				string text5 = stringArray[i].Substring(num45 + 1, num46 - num45 - 1);
				string[] array10 = stringArray[i].Substring(num46 + 1, num47 - num46 - 1).Split(',');
				if (text5.StartsWith("SetToOpposite"))
				{
					RCActionHelper rCActionHelper18 = ReturnHelper(array10[0]);
					RCAction item13 = new RCAction(category2, 11, null, new RCActionHelper[1] { rCActionHelper18 });
					list.Add(item13);
				}
				else if (text5.StartsWith("SetRandom"))
				{
					RCActionHelper rCActionHelper19 = ReturnHelper(array10[0]);
					RCAction item14 = new RCAction(category2, 12, null, new RCActionHelper[1] { rCActionHelper19 });
					list.Add(item14);
				}
				else if (text5.StartsWith("Set"))
				{
					RCActionHelper rCActionHelper20 = ReturnHelper(array10[0]);
					RCActionHelper rCActionHelper21 = ReturnHelper(array10[1]);
					RCAction item15 = new RCAction(category2, 0, null, new RCActionHelper[2] { rCActionHelper20, rCActionHelper21 });
					list.Add(item15);
				}
			}
			else if (stringArray[i].StartsWith("VariableString"))
			{
				int category3 = 3;
				int num48 = stringArray[i].IndexOf('.');
				int num49 = stringArray[i].IndexOf('(');
				int num50 = stringArray[i].LastIndexOf(')');
				string text6 = stringArray[i].Substring(num48 + 1, num49 - num48 - 1);
				string[] array11 = stringArray[i].Substring(num49 + 1, num50 - num49 - 1).Split(',');
				if (text6.StartsWith("Set"))
				{
					RCActionHelper rCActionHelper22 = ReturnHelper(array11[0]);
					RCActionHelper rCActionHelper23 = ReturnHelper(array11[1]);
					RCAction item16 = new RCAction(category3, 0, null, new RCActionHelper[2] { rCActionHelper22, rCActionHelper23 });
					list.Add(item16);
				}
				else if (text6.StartsWith("Concat"))
				{
					RCActionHelper[] array12 = new RCActionHelper[array11.Length];
					for (int num51 = 0; num51 < array11.Length; num51++)
					{
						array12[num51] = ReturnHelper(array11[num51]);
					}
					RCAction item17 = new RCAction(category3, 7, null, array12);
					list.Add(item17);
				}
				else if (text6.StartsWith("Append"))
				{
					RCActionHelper rCActionHelper24 = ReturnHelper(array11[0]);
					RCActionHelper rCActionHelper25 = ReturnHelper(array11[1]);
					RCAction item18 = new RCAction(category3, 8, null, new RCActionHelper[2] { rCActionHelper24, rCActionHelper25 });
					list.Add(item18);
				}
				else if (text6.StartsWith("Replace"))
				{
					RCActionHelper rCActionHelper26 = ReturnHelper(array11[0]);
					RCActionHelper rCActionHelper27 = ReturnHelper(array11[1]);
					RCActionHelper rCActionHelper28 = ReturnHelper(array11[2]);
					RCAction item19 = new RCAction(category3, 10, null, new RCActionHelper[3] { rCActionHelper26, rCActionHelper27, rCActionHelper28 });
					list.Add(item19);
				}
				else if (text6.StartsWith("Remove"))
				{
					RCActionHelper rCActionHelper29 = ReturnHelper(array11[0]);
					RCActionHelper rCActionHelper30 = ReturnHelper(array11[1]);
					RCAction item20 = new RCAction(category3, 9, null, new RCActionHelper[2] { rCActionHelper29, rCActionHelper30 });
					list.Add(item20);
				}
			}
			else if (stringArray[i].StartsWith("VariableFloat"))
			{
				int category4 = 4;
				int num52 = stringArray[i].IndexOf('.');
				int num53 = stringArray[i].IndexOf('(');
				int num54 = stringArray[i].LastIndexOf(')');
				string text7 = stringArray[i].Substring(num52 + 1, num53 - num52 - 1);
				string[] array13 = stringArray[i].Substring(num53 + 1, num54 - num53 - 1).Split(',');
				if (text7.StartsWith("SetRandom"))
				{
					RCActionHelper rCActionHelper31 = ReturnHelper(array13[0]);
					RCActionHelper rCActionHelper32 = ReturnHelper(array13[1]);
					RCActionHelper rCActionHelper33 = ReturnHelper(array13[2]);
					RCAction item21 = new RCAction(category4, 12, null, new RCActionHelper[3] { rCActionHelper31, rCActionHelper32, rCActionHelper33 });
					list.Add(item21);
				}
				else if (text7.StartsWith("Set"))
				{
					RCActionHelper rCActionHelper34 = ReturnHelper(array13[0]);
					RCActionHelper rCActionHelper35 = ReturnHelper(array13[1]);
					RCAction item22 = new RCAction(category4, 0, null, new RCActionHelper[2] { rCActionHelper34, rCActionHelper35 });
					list.Add(item22);
				}
				else if (text7.StartsWith("Add"))
				{
					RCActionHelper rCActionHelper36 = ReturnHelper(array13[0]);
					RCActionHelper rCActionHelper37 = ReturnHelper(array13[1]);
					RCAction item23 = new RCAction(category4, 1, null, new RCActionHelper[2] { rCActionHelper36, rCActionHelper37 });
					list.Add(item23);
				}
				else if (text7.StartsWith("Subtract"))
				{
					RCActionHelper rCActionHelper38 = ReturnHelper(array13[0]);
					RCActionHelper rCActionHelper39 = ReturnHelper(array13[1]);
					RCAction item24 = new RCAction(category4, 2, null, new RCActionHelper[2] { rCActionHelper38, rCActionHelper39 });
					list.Add(item24);
				}
				else if (text7.StartsWith("Multiply"))
				{
					RCActionHelper rCActionHelper40 = ReturnHelper(array13[0]);
					RCActionHelper rCActionHelper41 = ReturnHelper(array13[1]);
					RCAction item25 = new RCAction(category4, 3, null, new RCActionHelper[2] { rCActionHelper40, rCActionHelper41 });
					list.Add(item25);
				}
				else if (text7.StartsWith("Divide"))
				{
					RCActionHelper rCActionHelper42 = ReturnHelper(array13[0]);
					RCActionHelper rCActionHelper43 = ReturnHelper(array13[1]);
					RCAction item26 = new RCAction(category4, 4, null, new RCActionHelper[2] { rCActionHelper42, rCActionHelper43 });
					list.Add(item26);
				}
				else if (text7.StartsWith("Modulo"))
				{
					RCActionHelper rCActionHelper44 = ReturnHelper(array13[0]);
					RCActionHelper rCActionHelper45 = ReturnHelper(array13[1]);
					RCAction item27 = new RCAction(category4, 5, null, new RCActionHelper[2] { rCActionHelper44, rCActionHelper45 });
					list.Add(item27);
				}
				else if (text7.StartsWith("Power"))
				{
					RCActionHelper rCActionHelper46 = ReturnHelper(array13[0]);
					RCActionHelper rCActionHelper47 = ReturnHelper(array13[1]);
					RCAction item28 = new RCAction(category4, 6, null, new RCActionHelper[2] { rCActionHelper46, rCActionHelper47 });
					list.Add(item28);
				}
			}
			else if (stringArray[i].StartsWith("VariablePlayer"))
			{
				int category5 = 5;
				int num55 = stringArray[i].IndexOf('.');
				int num56 = stringArray[i].IndexOf('(');
				int num57 = stringArray[i].LastIndexOf(')');
				string text8 = stringArray[i].Substring(num55 + 1, num56 - num55 - 1);
				string[] array14 = stringArray[i].Substring(num56 + 1, num57 - num56 - 1).Split(',');
				if (text8.StartsWith("Set"))
				{
					RCActionHelper rCActionHelper48 = ReturnHelper(array14[0]);
					RCActionHelper rCActionHelper49 = ReturnHelper(array14[1]);
					RCAction item29 = new RCAction(category5, 0, null, new RCActionHelper[2] { rCActionHelper48, rCActionHelper49 });
					list.Add(item29);
				}
			}
			else if (stringArray[i].StartsWith("VariableTitan"))
			{
				int category6 = 6;
				int num58 = stringArray[i].IndexOf('.');
				int num59 = stringArray[i].IndexOf('(');
				int num60 = stringArray[i].LastIndexOf(')');
				string text9 = stringArray[i].Substring(num58 + 1, num59 - num58 - 1);
				string[] array15 = stringArray[i].Substring(num59 + 1, num60 - num59 - 1).Split(',');
				if (text9.StartsWith("Set"))
				{
					RCActionHelper rCActionHelper50 = ReturnHelper(array15[0]);
					RCActionHelper rCActionHelper51 = ReturnHelper(array15[1]);
					RCAction item30 = new RCAction(category6, 0, null, new RCActionHelper[2] { rCActionHelper50, rCActionHelper51 });
					list.Add(item30);
				}
			}
			else if (stringArray[i].StartsWith("Player"))
			{
				int category7 = 7;
				int num61 = stringArray[i].IndexOf('.');
				int num62 = stringArray[i].IndexOf('(');
				int num63 = stringArray[i].LastIndexOf(')');
				string text10 = stringArray[i].Substring(num61 + 1, num62 - num61 - 1);
				string[] array16 = stringArray[i].Substring(num62 + 1, num63 - num62 - 1).Split(',');
				if (text10.StartsWith("KillPlayer"))
				{
					RCActionHelper rCActionHelper52 = ReturnHelper(array16[0]);
					RCActionHelper rCActionHelper53 = ReturnHelper(array16[1]);
					RCAction item31 = new RCAction(category7, 0, null, new RCActionHelper[2] { rCActionHelper52, rCActionHelper53 });
					list.Add(item31);
				}
				else if (text10.StartsWith("SpawnPlayerAt"))
				{
					RCActionHelper rCActionHelper54 = ReturnHelper(array16[0]);
					RCActionHelper rCActionHelper55 = ReturnHelper(array16[1]);
					RCActionHelper rCActionHelper56 = ReturnHelper(array16[2]);
					RCActionHelper rCActionHelper57 = ReturnHelper(array16[3]);
					RCAction item32 = new RCAction(category7, 2, null, new RCActionHelper[4] { rCActionHelper54, rCActionHelper55, rCActionHelper56, rCActionHelper57 });
					list.Add(item32);
				}
				else if (text10.StartsWith("SpawnPlayer"))
				{
					RCActionHelper rCActionHelper58 = ReturnHelper(array16[0]);
					RCAction item33 = new RCAction(category7, 1, null, new RCActionHelper[1] { rCActionHelper58 });
					list.Add(item33);
				}
				else if (text10.StartsWith("MovePlayer"))
				{
					RCActionHelper rCActionHelper59 = ReturnHelper(array16[0]);
					RCActionHelper rCActionHelper60 = ReturnHelper(array16[1]);
					RCActionHelper rCActionHelper61 = ReturnHelper(array16[2]);
					RCActionHelper rCActionHelper62 = ReturnHelper(array16[3]);
					RCAction item34 = new RCAction(category7, 3, null, new RCActionHelper[4] { rCActionHelper59, rCActionHelper60, rCActionHelper61, rCActionHelper62 });
					list.Add(item34);
				}
				else if (text10.StartsWith("SetKills"))
				{
					RCActionHelper rCActionHelper63 = ReturnHelper(array16[0]);
					RCActionHelper rCActionHelper64 = ReturnHelper(array16[1]);
					RCAction item35 = new RCAction(category7, 4, null, new RCActionHelper[2] { rCActionHelper63, rCActionHelper64 });
					list.Add(item35);
				}
				else if (text10.StartsWith("SetDeaths"))
				{
					RCActionHelper rCActionHelper65 = ReturnHelper(array16[0]);
					RCActionHelper rCActionHelper66 = ReturnHelper(array16[1]);
					RCAction item36 = new RCAction(category7, 5, null, new RCActionHelper[2] { rCActionHelper65, rCActionHelper66 });
					list.Add(item36);
				}
				else if (text10.StartsWith("SetMaxDmg"))
				{
					RCActionHelper rCActionHelper67 = ReturnHelper(array16[0]);
					RCActionHelper rCActionHelper68 = ReturnHelper(array16[1]);
					RCAction item37 = new RCAction(category7, 6, null, new RCActionHelper[2] { rCActionHelper67, rCActionHelper68 });
					list.Add(item37);
				}
				else if (text10.StartsWith("SetTotalDmg"))
				{
					RCActionHelper rCActionHelper69 = ReturnHelper(array16[0]);
					RCActionHelper rCActionHelper70 = ReturnHelper(array16[1]);
					RCAction item38 = new RCAction(category7, 7, null, new RCActionHelper[2] { rCActionHelper69, rCActionHelper70 });
					list.Add(item38);
				}
				else if (text10.StartsWith("SetName"))
				{
					RCActionHelper rCActionHelper71 = ReturnHelper(array16[0]);
					RCActionHelper rCActionHelper72 = ReturnHelper(array16[1]);
					RCAction item39 = new RCAction(category7, 8, null, new RCActionHelper[2] { rCActionHelper71, rCActionHelper72 });
					list.Add(item39);
				}
				else if (text10.StartsWith("SetGuildName"))
				{
					RCActionHelper rCActionHelper73 = ReturnHelper(array16[0]);
					RCActionHelper rCActionHelper74 = ReturnHelper(array16[1]);
					RCAction item40 = new RCAction(category7, 9, null, new RCActionHelper[2] { rCActionHelper73, rCActionHelper74 });
					list.Add(item40);
				}
				else if (text10.StartsWith("SetTeam"))
				{
					RCActionHelper rCActionHelper75 = ReturnHelper(array16[0]);
					RCActionHelper rCActionHelper76 = ReturnHelper(array16[1]);
					RCAction item41 = new RCAction(category7, 10, null, new RCActionHelper[2] { rCActionHelper75, rCActionHelper76 });
					list.Add(item41);
				}
				else if (text10.StartsWith("SetCustomInt"))
				{
					RCActionHelper rCActionHelper77 = ReturnHelper(array16[0]);
					RCActionHelper rCActionHelper78 = ReturnHelper(array16[1]);
					RCAction item42 = new RCAction(category7, 11, null, new RCActionHelper[2] { rCActionHelper77, rCActionHelper78 });
					list.Add(item42);
				}
				else if (text10.StartsWith("SetCustomBool"))
				{
					RCActionHelper rCActionHelper79 = ReturnHelper(array16[0]);
					RCActionHelper rCActionHelper80 = ReturnHelper(array16[1]);
					RCAction item43 = new RCAction(category7, 12, null, new RCActionHelper[2] { rCActionHelper79, rCActionHelper80 });
					list.Add(item43);
				}
				else if (text10.StartsWith("SetCustomString"))
				{
					RCActionHelper rCActionHelper81 = ReturnHelper(array16[0]);
					RCActionHelper rCActionHelper82 = ReturnHelper(array16[1]);
					RCAction item44 = new RCAction(category7, 13, null, new RCActionHelper[2] { rCActionHelper81, rCActionHelper82 });
					list.Add(item44);
				}
				else if (text10.StartsWith("SetCustomFloat"))
				{
					RCActionHelper rCActionHelper83 = ReturnHelper(array16[0]);
					RCActionHelper rCActionHelper84 = ReturnHelper(array16[1]);
					RCAction item45 = new RCAction(category7, 14, null, new RCActionHelper[2] { rCActionHelper83, rCActionHelper84 });
					list.Add(item45);
				}
			}
			else if (stringArray[i].StartsWith("Titan"))
			{
				int category8 = 8;
				int num64 = stringArray[i].IndexOf('.');
				int num65 = stringArray[i].IndexOf('(');
				int num66 = stringArray[i].LastIndexOf(')');
				string text11 = stringArray[i].Substring(num64 + 1, num65 - num64 - 1);
				string[] array17 = stringArray[i].Substring(num65 + 1, num66 - num65 - 1).Split(',');
				if (text11.StartsWith("KillTitan"))
				{
					RCActionHelper rCActionHelper85 = ReturnHelper(array17[0]);
					RCActionHelper rCActionHelper86 = ReturnHelper(array17[1]);
					RCActionHelper rCActionHelper87 = ReturnHelper(array17[2]);
					RCAction item46 = new RCAction(category8, 0, null, new RCActionHelper[3] { rCActionHelper85, rCActionHelper86, rCActionHelper87 });
					list.Add(item46);
				}
				else if (text11.StartsWith("SpawnTitanAt"))
				{
					RCActionHelper rCActionHelper88 = ReturnHelper(array17[0]);
					RCActionHelper rCActionHelper89 = ReturnHelper(array17[1]);
					RCActionHelper rCActionHelper90 = ReturnHelper(array17[2]);
					RCActionHelper rCActionHelper91 = ReturnHelper(array17[3]);
					RCActionHelper rCActionHelper92 = ReturnHelper(array17[4]);
					RCActionHelper rCActionHelper93 = ReturnHelper(array17[5]);
					RCActionHelper rCActionHelper94 = ReturnHelper(array17[6]);
					RCAction item47 = new RCAction(category8, 2, null, new RCActionHelper[7] { rCActionHelper88, rCActionHelper89, rCActionHelper90, rCActionHelper91, rCActionHelper92, rCActionHelper93, rCActionHelper94 });
					list.Add(item47);
				}
				else if (text11.StartsWith("SpawnTitan"))
				{
					RCActionHelper rCActionHelper95 = ReturnHelper(array17[0]);
					RCActionHelper rCActionHelper96 = ReturnHelper(array17[1]);
					RCActionHelper rCActionHelper97 = ReturnHelper(array17[2]);
					RCActionHelper rCActionHelper98 = ReturnHelper(array17[3]);
					RCAction item48 = new RCAction(category8, 1, null, new RCActionHelper[4] { rCActionHelper95, rCActionHelper96, rCActionHelper97, rCActionHelper98 });
					list.Add(item48);
				}
				else if (text11.StartsWith("SetHealth"))
				{
					RCActionHelper rCActionHelper99 = ReturnHelper(array17[0]);
					RCActionHelper rCActionHelper100 = ReturnHelper(array17[1]);
					RCAction item49 = new RCAction(category8, 3, null, new RCActionHelper[2] { rCActionHelper99, rCActionHelper100 });
					list.Add(item49);
				}
				else if (text11.StartsWith("MoveTitan"))
				{
					RCActionHelper rCActionHelper101 = ReturnHelper(array17[0]);
					RCActionHelper rCActionHelper102 = ReturnHelper(array17[1]);
					RCActionHelper rCActionHelper103 = ReturnHelper(array17[2]);
					RCActionHelper rCActionHelper104 = ReturnHelper(array17[3]);
					RCAction item50 = new RCAction(category8, 4, null, new RCActionHelper[4] { rCActionHelper101, rCActionHelper102, rCActionHelper103, rCActionHelper104 });
					list.Add(item50);
				}
			}
			else if (stringArray[i].StartsWith("Game"))
			{
				int category9 = 9;
				int num67 = stringArray[i].IndexOf('.');
				int num68 = stringArray[i].IndexOf('(');
				int num69 = stringArray[i].LastIndexOf(')');
				string text12 = stringArray[i].Substring(num67 + 1, num68 - num67 - 1);
				string[] array18 = stringArray[i].Substring(num68 + 1, num69 - num68 - 1).Split(',');
				if (text12.StartsWith("PrintMessage"))
				{
					RCActionHelper rCActionHelper105 = ReturnHelper(array18[0]);
					RCAction item51 = new RCAction(category9, 0, null, new RCActionHelper[1] { rCActionHelper105 });
					list.Add(item51);
				}
				else if (text12.StartsWith("LoseGame"))
				{
					RCActionHelper rCActionHelper106 = ReturnHelper(array18[0]);
					RCAction item52 = new RCAction(category9, 2, null, new RCActionHelper[1] { rCActionHelper106 });
					list.Add(item52);
				}
				else if (text12.StartsWith("WinGame"))
				{
					RCActionHelper rCActionHelper107 = ReturnHelper(array18[0]);
					RCAction item53 = new RCAction(category9, 1, null, new RCActionHelper[1] { rCActionHelper107 });
					list.Add(item53);
				}
				else if (text12.StartsWith("Restart"))
				{
					RCActionHelper rCActionHelper108 = ReturnHelper(array18[0]);
					RCAction item54 = new RCAction(category9, 3, null, new RCActionHelper[1] { rCActionHelper108 });
					list.Add(item54);
				}
			}
		}
		return new RCEvent(condition, list, eventClass, eventType);
	}

	public RCActionHelper ReturnHelper(string str)
	{
		string[] array = str.Split('.');
		if (float.TryParse(str, out var _))
		{
			array = new string[1] { str };
		}
		List<RCActionHelper> list = new List<RCActionHelper>();
		int sentType = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (list.Count == 0)
			{
				string text = array[i];
				int result2;
				float result3;
				if (text.StartsWith("\"") && text.EndsWith("\""))
				{
					RCActionHelper item = new RCActionHelper(0, 0, text.Substring(1, text.Length - 2));
					list.Add(item);
					sentType = 2;
				}
				else if (int.TryParse(text, out result2))
				{
					RCActionHelper item2 = new RCActionHelper(0, 0, result2);
					list.Add(item2);
					sentType = 0;
				}
				else if (float.TryParse(text, out result3))
				{
					RCActionHelper item3 = new RCActionHelper(0, 0, result3);
					list.Add(item3);
					sentType = 3;
				}
				else if (text.ToLower() == "true" || text.ToLower() == "false")
				{
					RCActionHelper item4 = new RCActionHelper(0, 0, Convert.ToBoolean(text.ToLower()));
					list.Add(item4);
					sentType = 1;
				}
				else if (text.StartsWith("Variable"))
				{
					int num = text.IndexOf('(');
					int num2 = text.LastIndexOf(')');
					if (text.StartsWith("VariableInt"))
					{
						text = text.Substring(num + 1, num2 - num - 1);
						RCActionHelper item5 = new RCActionHelper(1, 0, ReturnHelper(text));
						list.Add(item5);
						sentType = 0;
					}
					else if (text.StartsWith("VariableBool"))
					{
						text = text.Substring(num + 1, num2 - num - 1);
						RCActionHelper item6 = new RCActionHelper(1, 1, ReturnHelper(text));
						list.Add(item6);
						sentType = 1;
					}
					else if (text.StartsWith("VariableString"))
					{
						text = text.Substring(num + 1, num2 - num - 1);
						RCActionHelper item7 = new RCActionHelper(1, 2, ReturnHelper(text));
						list.Add(item7);
						sentType = 2;
					}
					else if (text.StartsWith("VariableFloat"))
					{
						text = text.Substring(num + 1, num2 - num - 1);
						RCActionHelper item8 = new RCActionHelper(1, 3, ReturnHelper(text));
						list.Add(item8);
						sentType = 3;
					}
					else if (text.StartsWith("VariablePlayer"))
					{
						text = text.Substring(num + 1, num2 - num - 1);
						RCActionHelper item9 = new RCActionHelper(1, 4, ReturnHelper(text));
						list.Add(item9);
						sentType = 4;
					}
					else if (text.StartsWith("VariableTitan"))
					{
						text = text.Substring(num + 1, num2 - num - 1);
						RCActionHelper item10 = new RCActionHelper(1, 5, ReturnHelper(text));
						list.Add(item10);
						sentType = 5;
					}
				}
				else if (text.StartsWith("Region"))
				{
					int num3 = text.IndexOf('(');
					int num4 = text.LastIndexOf(')');
					if (text.StartsWith("RegionRandomX"))
					{
						text = text.Substring(num3 + 1, num4 - num3 - 1);
						RCActionHelper item11 = new RCActionHelper(4, 0, ReturnHelper(text));
						list.Add(item11);
						sentType = 3;
					}
					else if (text.StartsWith("RegionRandomY"))
					{
						text = text.Substring(num3 + 1, num4 - num3 - 1);
						RCActionHelper item12 = new RCActionHelper(4, 1, ReturnHelper(text));
						list.Add(item12);
						sentType = 3;
					}
					else if (text.StartsWith("RegionRandomZ"))
					{
						text = text.Substring(num3 + 1, num4 - num3 - 1);
						RCActionHelper item13 = new RCActionHelper(4, 2, ReturnHelper(text));
						list.Add(item13);
						sentType = 3;
					}
				}
			}
			else
			{
				if (list.Count <= 0)
				{
					continue;
				}
				string text2 = array[i];
				if (list[list.Count - 1].helperClass == 1)
				{
					switch (list[list.Count - 1].helperType)
					{
					case 4:
						if (text2.StartsWith("GetTeam()"))
						{
							RCActionHelper item18 = new RCActionHelper(2, 1, null);
							list.Add(item18);
							sentType = 0;
						}
						else if (text2.StartsWith("GetType()"))
						{
							RCActionHelper item19 = new RCActionHelper(2, 0, null);
							list.Add(item19);
							sentType = 0;
						}
						else if (text2.StartsWith("GetIsAlive()"))
						{
							RCActionHelper item20 = new RCActionHelper(2, 2, null);
							list.Add(item20);
							sentType = 1;
						}
						else if (text2.StartsWith("GetTitan()"))
						{
							RCActionHelper item21 = new RCActionHelper(2, 3, null);
							list.Add(item21);
							sentType = 0;
						}
						else if (text2.StartsWith("GetKills()"))
						{
							RCActionHelper item22 = new RCActionHelper(2, 4, null);
							list.Add(item22);
							sentType = 0;
						}
						else if (text2.StartsWith("GetDeaths()"))
						{
							RCActionHelper item23 = new RCActionHelper(2, 5, null);
							list.Add(item23);
							sentType = 0;
						}
						else if (text2.StartsWith("GetMaxDmg()"))
						{
							RCActionHelper item24 = new RCActionHelper(2, 6, null);
							list.Add(item24);
							sentType = 0;
						}
						else if (text2.StartsWith("GetTotalDmg()"))
						{
							RCActionHelper item25 = new RCActionHelper(2, 7, null);
							list.Add(item25);
							sentType = 0;
						}
						else if (text2.StartsWith("GetCustomInt()"))
						{
							RCActionHelper item26 = new RCActionHelper(2, 8, null);
							list.Add(item26);
							sentType = 0;
						}
						else if (text2.StartsWith("GetCustomBool()"))
						{
							RCActionHelper item27 = new RCActionHelper(2, 9, null);
							list.Add(item27);
							sentType = 1;
						}
						else if (text2.StartsWith("GetCustomString()"))
						{
							RCActionHelper item28 = new RCActionHelper(2, 10, null);
							list.Add(item28);
							sentType = 2;
						}
						else if (text2.StartsWith("GetCustomFloat()"))
						{
							RCActionHelper item29 = new RCActionHelper(2, 11, null);
							list.Add(item29);
							sentType = 3;
						}
						else if (text2.StartsWith("GetPositionX()"))
						{
							RCActionHelper item30 = new RCActionHelper(2, 14, null);
							list.Add(item30);
							sentType = 3;
						}
						else if (text2.StartsWith("GetPositionY()"))
						{
							RCActionHelper item31 = new RCActionHelper(2, 15, null);
							list.Add(item31);
							sentType = 3;
						}
						else if (text2.StartsWith("GetPositionZ()"))
						{
							RCActionHelper item32 = new RCActionHelper(2, 16, null);
							list.Add(item32);
							sentType = 3;
						}
						else if (text2.StartsWith("GetName()"))
						{
							RCActionHelper item33 = new RCActionHelper(2, 12, null);
							list.Add(item33);
							sentType = 2;
						}
						else if (text2.StartsWith("GetGuildName()"))
						{
							RCActionHelper item34 = new RCActionHelper(2, 13, null);
							list.Add(item34);
							sentType = 2;
						}
						else if (text2.StartsWith("GetSpeed()"))
						{
							RCActionHelper item35 = new RCActionHelper(2, 17, null);
							list.Add(item35);
							sentType = 3;
						}
						break;
					case 5:
						if (text2.StartsWith("GetType()"))
						{
							RCActionHelper item36 = new RCActionHelper(3, 0, null);
							list.Add(item36);
							sentType = 0;
						}
						else if (text2.StartsWith("GetSize()"))
						{
							RCActionHelper item37 = new RCActionHelper(3, 1, null);
							list.Add(item37);
							sentType = 3;
						}
						else if (text2.StartsWith("GetHealth()"))
						{
							RCActionHelper item38 = new RCActionHelper(3, 2, null);
							list.Add(item38);
							sentType = 0;
						}
						else if (text2.StartsWith("GetPositionX()"))
						{
							RCActionHelper item39 = new RCActionHelper(3, 3, null);
							list.Add(item39);
							sentType = 3;
						}
						else if (text2.StartsWith("GetPositionY()"))
						{
							RCActionHelper item40 = new RCActionHelper(3, 4, null);
							list.Add(item40);
							sentType = 3;
						}
						else if (text2.StartsWith("GetPositionZ()"))
						{
							RCActionHelper item41 = new RCActionHelper(3, 5, null);
							list.Add(item41);
							sentType = 3;
						}
						break;
					default:
						if (text2.StartsWith("ConvertToInt()"))
						{
							RCActionHelper item14 = new RCActionHelper(5, sentType, null);
							list.Add(item14);
							sentType = 0;
						}
						else if (text2.StartsWith("ConvertToBool()"))
						{
							RCActionHelper item15 = new RCActionHelper(5, sentType, null);
							list.Add(item15);
							sentType = 1;
						}
						else if (text2.StartsWith("ConvertToString()"))
						{
							RCActionHelper item16 = new RCActionHelper(5, sentType, null);
							list.Add(item16);
							sentType = 2;
						}
						else if (text2.StartsWith("ConvertToFloat()"))
						{
							RCActionHelper item17 = new RCActionHelper(5, sentType, null);
							list.Add(item17);
							sentType = 3;
						}
						break;
					}
				}
				else if (text2.StartsWith("ConvertToInt()"))
				{
					RCActionHelper item42 = new RCActionHelper(5, sentType, null);
					list.Add(item42);
					sentType = 0;
				}
				else if (text2.StartsWith("ConvertToBool()"))
				{
					RCActionHelper item43 = new RCActionHelper(5, sentType, null);
					list.Add(item43);
					sentType = 1;
				}
				else if (text2.StartsWith("ConvertToString()"))
				{
					RCActionHelper item44 = new RCActionHelper(5, sentType, null);
					list.Add(item44);
					sentType = 2;
				}
				else if (text2.StartsWith("ConvertToFloat()"))
				{
					RCActionHelper item45 = new RCActionHelper(5, sentType, null);
					list.Add(item45);
					sentType = 3;
				}
			}
		}
		for (int num5 = list.Count - 1; num5 > 0; num5--)
		{
			list[num5 - 1].SetNextHelper(list[num5]);
		}
		return list[0];
	}

	public int OperandType(string str, int condition)
	{
		switch (condition)
		{
		case 0:
		case 3:
			if (str.StartsWith("Equals"))
			{
				return 2;
			}
			if (str.StartsWith("NotEquals"))
			{
				return 5;
			}
			if (str.StartsWith("LessThan"))
			{
				return 0;
			}
			if (str.StartsWith("LessThanOrEquals"))
			{
				return 1;
			}
			if (str.StartsWith("GreaterThanOrEquals"))
			{
				return 3;
			}
			if (str.StartsWith("GreaterThan"))
			{
				return 4;
			}
			return 0;
		case 1:
		case 4:
		case 5:
			if (str.StartsWith("Equals"))
			{
				return 2;
			}
			if (str.StartsWith("NotEquals"))
			{
				return 5;
			}
			return 0;
		case 2:
			if (str.StartsWith("Equals"))
			{
				return 0;
			}
			if (str.StartsWith("NotEquals"))
			{
				return 1;
			}
			if (str.StartsWith("Contains"))
			{
				return 2;
			}
			if (str.StartsWith("NotContains"))
			{
				return 3;
			}
			if (str.StartsWith("StartsWith"))
			{
				return 4;
			}
			if (str.StartsWith("NotStartsWith"))
			{
				return 5;
			}
			if (str.StartsWith("EndsWith"))
			{
				return 6;
			}
			if (str.StartsWith("NotEndsWith"))
			{
				return 7;
			}
			return 0;
		default:
			return 0;
		}
	}

	public int ConditionType(string str)
	{
		if (str.StartsWith("Int"))
		{
			return 0;
		}
		if (str.StartsWith("Bool"))
		{
			return 1;
		}
		if (str.StartsWith("String"))
		{
			return 2;
		}
		if (str.StartsWith("Float"))
		{
			return 3;
		}
		if (str.StartsWith("Titan"))
		{
			return 5;
		}
		if (str.StartsWith("Player"))
		{
			return 4;
		}
		return 0;
	}

	public void OnUpdate()
	{
		if (RCEvents.ContainsKey("OnUpdate"))
		{
			if (updateTime > 0f)
			{
				updateTime -= Time.deltaTime;
				return;
			}
			((RCEvent)RCEvents["OnUpdate"]).CheckEvent();
			updateTime = 1f;
		}
	}

	[RPC]
	public void spawnPlayerAtRPC(float posX, float posY, float posZ, PhotonMessageInfo info)
	{
		if (!info.sender.isMasterClient || !LogicLoaded || !CustomLevelLoaded || needChooseSide || !mainCamera.gameOver)
		{
			return;
		}
		Vector3 position = new Vector3(posX, posY, posZ);
		mainCamera.SetMainObject(PhotonNetwork.Instantiate("AOTTG_HERO 1", position, new Quaternion(0f, 0f, 0f, 1f), 0));
		HERO component = mainCamera.main_object.GetComponent<HERO>();
		HERO_SETUP component2 = component.GetComponent<HERO_SETUP>();
		string text = myLastHero;
		text = text.ToUpper();
		switch (text)
		{
		case "SET 1":
		case "SET 2":
		case "SET 3":
		{
			HeroCostume heroCostume = CostumeConverter.FromLocalData(text);
			CostumeConverter.ToLocalData(heroCostume, text);
			component2.Init();
			if (heroCostume != null)
			{
				component2.myCostume = heroCostume;
				component2.myCostume.stat = heroCostume.stat;
			}
			else
			{
				heroCostume = (component2.myCostume = HeroCostume.CostumeOptions[3]);
				component2.myCostume.stat = HeroStat.GetInfo(heroCostume.name.ToUpper());
			}
			component2.CreateCharacterComponent();
			component.SetStat2();
			component.SetSkillHUDPosition2();
			break;
		}
		default:
		{
			for (int i = 0; i < HeroCostume.Costumes.Length; i++)
			{
				if (HeroCostume.Costumes[i].name.ToUpper() == text.ToUpper())
				{
					int num = HeroCostume.Costumes[i].id;
					if (text.ToUpper() != "AHSS")
					{
						num += CheckBoxCostume.CostumeSet - 1;
					}
					if (HeroCostume.Costumes[num].name != HeroCostume.Costumes[i].name)
					{
						num = HeroCostume.Costumes[i].id + 1;
					}
					component2.Init();
					component2.myCostume = HeroCostume.Costumes[num];
					component2.myCostume.stat = HeroStat.GetInfo(HeroCostume.Costumes[num].name.ToUpper());
					component2.CreateCharacterComponent();
					component.SetStat2();
					component.SetSkillHUDPosition2();
					break;
				}
			}
			break;
		}
		}
		CostumeConverter.ToPhotonData(component2.myCostume, PhotonNetwork.player);
		if (IN_GAME_MAIN_CAMERA.Gamemode == GameMode.PvPCapture)
		{
			mainCamera.main_object.transform.position += new Vector3(UnityEngine.Random.Range(-20, 20), 2f, UnityEngine.Random.Range(-20, 20));
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add("dead", false);
		hashtable.Add(PhotonPlayerProperty.IsTitan, 1);
		PhotonNetwork.player.SetCustomProperties(hashtable);
		mainCamera.enabled = true;
		GameObject obj = GameObject.Find("MainCamera");
		mainCamera.SetHUDPosition();
		obj.GetComponent<SpectatorMovement>().disable = true;
		obj.GetComponent<MouseLook>().disable = true;
		mainCamera.gameOver = false;
		Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode == CameraType.TPS;
		Screen.showCursor = false;
		isLosing = false;
		SetTextCenter(string.Empty);
	}

	private void spawnPlayerCustomMap()
	{
		if (!needChooseSide && mainCamera.gameOver)
		{
			mainCamera.gameOver = false;
			if (GExtensions.AsInt(PhotonNetwork.player.customProperties[PhotonPlayerProperty.IsTitan]) == 2)
			{
				SpawnNonAITitan2(myLastHero);
			}
			else
			{
				SpawnPlayer(myLastHero, myLastRespawnTag);
			}
			SetTextCenter(string.Empty);
		}
	}

	public void NOTSpawnPlayerRC(string id)
	{
		myLastHero = id.ToUpper();
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add("dead", true);
		hashtable.Add(PhotonPlayerProperty.IsTitan, 1);
		PhotonNetwork.player.SetCustomProperties(hashtable);
		Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode == CameraType.TPS;
		Screen.showCursor = false;
		mainCamera.enabled = true;
		mainCamera.SetMainObject(null);
		mainCamera.SetSpectorMode(val: true);
		mainCamera.gameOver = true;
	}

	public void NOTSpawnNonAITitanRC(string id)
	{
		myLastHero = id.ToUpper();
		ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable
		{
			{ "dead", true },
			{
				PhotonPlayerProperty.IsTitan,
				2
			}
		};
		PhotonNetwork.player.SetCustomProperties(propertiesToSet);
		Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode == CameraType.TPS;
		Screen.showCursor = true;
		SetTextCenter("Syncing spawn locations...");
		mainCamera.enabled = true;
		mainCamera.SetMainObject(null);
		mainCamera.SetSpectorMode(val: true);
		mainCamera.gameOver = true;
	}

	private IEnumerator CoRespawn(float seconds)
	{
		while (true)
		{
			yield return new WaitForSeconds(seconds);
			if (isLosing || isWinning)
			{
				continue;
			}
			PhotonPlayer[] array = PhotonNetwork.playerList;
			foreach (PhotonPlayer photonPlayer in array)
			{
				if (photonPlayer.customProperties[PhotonPlayerProperty.RCTeam] == null && GExtensions.AsBool(photonPlayer.customProperties[PhotonPlayerProperty.IsDead]) && GExtensions.AsInt(photonPlayer.customProperties[PhotonPlayerProperty.IsTitan]) != 2)
				{
					base.photonView.RPC("respawnHeroInNewRound", photonPlayer);
				}
			}
		}
	}

	private void EndGameRC()
	{
		if (RCSettings.PointMode > 0)
		{
			for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
			{
				PhotonNetwork.playerList[i].SetCustomProperties(new ExitGames.Client.Photon.Hashtable
				{
					{
						PhotonPlayerProperty.Kills,
						0
					},
					{
						PhotonPlayerProperty.Deaths,
						0
					},
					{
						PhotonPlayerProperty.MaxDamage,
						0
					},
					{
						PhotonPlayerProperty.TotalDamage,
						0
					}
				});
			}
		}
		gameEndCD = 0f;
		RestartGame();
	}

	private void EndGameInfection()
	{
		ImATitan.Clear();
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add(PhotonPlayerProperty.IsTitan, 1);
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			PhotonNetwork.playerList[i].SetCustomProperties(hashtable);
		}
		int num = PhotonNetwork.playerList.Length;
		int num2 = RCSettings.InfectionMode;
		ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable();
		hashtable2.Add(PhotonPlayerProperty.IsTitan, 2);
		for (int j = 0; j < PhotonNetwork.playerList.Length; j++)
		{
			PhotonPlayer photonPlayer = PhotonNetwork.playerList[j];
			if (num > 0 && UnityEngine.Random.Range(0f, 1f) <= (float)num2 / (float)num)
			{
				photonPlayer.SetCustomProperties(hashtable2);
				ImATitan.Add(photonPlayer.Id, 2);
				num2--;
			}
			num--;
		}
		gameEndCD = 0f;
		RestartGame();
	}

	public void KickPlayer(PhotonPlayer player, bool ban, string reason)
	{
		if (OnPrivateServer)
		{
			string inGameName = GExtensions.AsString(player.customProperties[PhotonPlayerProperty.Name]);
			ServerCloseConnection(player, ban, inGameName);
			return;
		}
		PhotonNetwork.DestroyPlayerObjects(player);
		PhotonNetwork.CloseConnection(player);
		base.photonView.RPC("ignorePlayer", PhotonTargets.Others, player.Id);
		if (!IgnoreList.Contains(player.Id))
		{
			IgnoreList.Add(player.Id);
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.TargetActors = new int[1] { player.Id };
			RaiseEventOptions options = raiseEventOptions;
			PhotonNetwork.RaiseEvent(254, null, sendReliable: true, options);
		}
		if (ban && !BanHash.ContainsKey(player.Id))
		{
			string value = GExtensions.AsString(player.customProperties[PhotonPlayerProperty.Name]);
			BanHash.Add(player.Id, value);
		}
		if (reason.Length > 0)
		{
			InRoomChat.Instance.AddLine(string.Format("Player #{0} was {1}. Reason: ", player.Id, ban ? "banned" : "kicked") + reason);
		}
		RecompilePlayerList(0.1f);
	}

	[RPC]
	private void ignorePlayer(int id, PhotonMessageInfo info)
	{
		if (info.sender.isMasterClient && PhotonPlayer.Find(id) != null && !IgnoreList.Contains(id))
		{
			IgnoreList.Add(id);
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.TargetActors = new int[1] { id };
			RaiseEventOptions options = raiseEventOptions;
			PhotonNetwork.RaiseEvent(254, null, sendReliable: true, options);
		}
		RecompilePlayerList(0.1f);
	}

	[RPC]
	private void ignorePlayerArray(int[] ids, PhotonMessageInfo info)
	{
		if (info.sender.isMasterClient)
		{
			foreach (int num in ids)
			{
				if (PhotonPlayer.Find(num) != null && !IgnoreList.Contains(num))
				{
					IgnoreList.Add(num);
					PhotonNetwork.RaiseEvent(254, null, sendReliable: true, new RaiseEventOptions
					{
						TargetActors = new int[1] { num }
					});
				}
			}
		}
		RecompilePlayerList(0.1f);
	}

	private void ResetSettings(bool isLeave)
	{
		MasterRC = false;
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add(PhotonPlayerProperty.RCTeam, 0);
		if (isLeave)
		{
			CurrentLevel = string.Empty;
			hashtable.Add(PhotonPlayerProperty.CurrentLevel, string.Empty);
			levelCache = new List<string[]>();
			titanSpawns.Clear();
			playerSpawnsC.Clear();
			playerSpawnsM.Clear();
			titanSpawners.Clear();
			IntVariables.Clear();
			BoolVariables.Clear();
			StringVariables.Clear();
			FloatVariables.Clear();
			GlobalVariables.Clear();
			RCRegions.Clear();
			RCEvents.Clear();
			RCVariableNames.Clear();
			PlayerVariables.Clear();
			TitanVariables.Clear();
			RCRegionTriggers.Clear();
			CurrentScriptLogic = string.Empty;
			hashtable.Add(PhotonPlayerProperty.StatAccel, 100);
			hashtable.Add(PhotonPlayerProperty.StatBlade, 100);
			hashtable.Add(PhotonPlayerProperty.StatGas, 100);
			hashtable.Add(PhotonPlayerProperty.StatSpeed, 100);
			restartingTitan = false;
			restartingMC = false;
			restartingHorse = false;
			restartingEren = false;
			restartingBomb = false;
		}
		PhotonNetwork.player.SetCustomProperties(hashtable);
		ResetGameSettings();
		BanHash = new ExitGames.Client.Photon.Hashtable();
		ImATitan = new ExitGames.Client.Photon.Hashtable();
		OldScript = string.Empty;
		IgnoreList = new List<int>();
		restartCount = new List<float>();
		HeroHash = new ExitGames.Client.Photon.Hashtable();
	}

	private void ResetGameSettings()
	{
		RCSettings.BombMode = 0;
		RCSettings.TeamMode = 0;
		RCSettings.PointMode = 0;
		RCSettings.DisableRock = 0;
		RCSettings.ExplodeMode = 0;
		RCSettings.HealthMode = 0;
		RCSettings.HealthLower = 0;
		RCSettings.HealthUpper = 0;
		RCSettings.InfectionMode = 0;
		RCSettings.BanEren = 0;
		RCSettings.MoreTitans = 0;
		RCSettings.MinimumDamage = 0;
		RCSettings.SizeMode = 0;
		RCSettings.SizeLower = 0f;
		RCSettings.SizeUpper = 0f;
		RCSettings.SpawnMode = 0;
		RCSettings.NormalRate = 0f;
		RCSettings.AberrantRate = 0f;
		RCSettings.JumperRate = 0f;
		RCSettings.CrawlerRate = 0f;
		RCSettings.PunkRate = 0f;
		RCSettings.HorseMode = 0;
		RCSettings.WaveModeOn = 0;
		RCSettings.WaveModeNum = 0;
		RCSettings.FriendlyMode = 0;
		RCSettings.PvPMode = 0;
		RCSettings.MaxWave = 0;
		RCSettings.EndlessMode = 0;
		RCSettings.AhssReload = 0;
		RCSettings.PunkWaves = 0;
		RCSettings.GlobalDisableMinimap = 0;
		RCSettings.Motd = string.Empty;
		RCSettings.DeadlyCannons = 0;
		RCSettings.AsoPreserveKDR = 0;
		RCSettings.RacingStatic = 0;
	}

	public void AddTime(float time)
	{
		timeTotalServer -= time;
	}

	[Obsolete("Use FengGameManagerMKII.Chat instead.")]
	[RPC]
	private void ChatPM(string sender, string content, PhotonMessageInfo info)
	{
		Chat(content, "=> You".AsColor("FFCC00"), info);
	}

	[RPC]
	private void pauseRPC(bool paused, PhotonMessageInfo info)
	{
		if (FGMChecker.IsPauseStateChangeValid(info))
		{
			if (paused)
			{
				pauseWaitTime = 4f;
				Time.timeScale = 1E-06f;
			}
			else
			{
				pauseWaitTime = 3f;
			}
		}
	}

	private void CoreAdd()
	{
		if (PhotonNetwork.isMasterClient)
		{
			OnUpdate();
			if (CustomLevelLoaded)
			{
				for (int i = 0; i < titanSpawners.Count; i++)
				{
					TitanSpawner titanSpawner = titanSpawners[i];
					titanSpawner.time -= Time.deltaTime;
					if (!(titanSpawner.time <= 0f) || titans.Count + fT.Count >= Math.Min(RCSettings.TitanCap, 80))
					{
						continue;
					}
					if (titanSpawner.name == "spawnAnnie")
					{
						PhotonNetwork.Instantiate("FEMALE_TITAN", titanSpawner.location, new Quaternion(0f, 0f, 0f, 1f), 0);
					}
					else
					{
						GameObject gameObject = PhotonNetwork.Instantiate("TITAN_VER3.1", titanSpawner.location, new Quaternion(0f, 0f, 0f, 1f), 0);
						switch (titanSpawner.name)
						{
						case "spawnAbnormal":
							gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Aberrant, forceCrawler: false);
							break;
						case "spawnJumper":
							gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Jumper, forceCrawler: false);
							break;
						case "spawnCrawler":
							gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Crawler, forceCrawler: true);
							break;
						case "spawnPunk":
							gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Punk, forceCrawler: false);
							break;
						}
					}
					if (titanSpawner.endless)
					{
						titanSpawner.time = titanSpawner.delay;
					}
					else
					{
						titanSpawners.Remove(titanSpawner);
					}
				}
			}
		}
		if (!(Time.timeScale <= 0.1f))
		{
			return;
		}
		if (pauseWaitTime <= 3f)
		{
			pauseWaitTime -= Time.deltaTime * 1000000f;
			if (pauseWaitTime <= 1f)
			{
				Camera.main.farClipPlane = 1500f;
			}
			if (pauseWaitTime <= 0f)
			{
				pauseWaitTime = 0f;
				Time.timeScale = 1f;
			}
		}
		CoWaitAndRecompilePlayerList(0.1f);
	}

	private void Cache()
	{
		ClothFactory.ClearClothCache();
		inputManager = GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>();
		otherUsers.Clear();
		titanSpawners.Clear();
		groundList.Clear();
		PreservedPlayerKDR = new Dictionary<string, int[]>();
		NoRestart = false;
		SkyMaterial = null;
		isSpawning = false;
		retryTime = 0f;
		LogicLoaded = false;
		CustomLevelLoaded = true;
		isUnloading = false;
		isRecompiling = false;
		Time.timeScale = 1f;
		Camera.main.farClipPlane = 1500f;
		pauseWaitTime = 0f;
		spectateSprites = new List<GameObject>();
		isRestarting = false;
		if ((int)Settings[64] >= 100)
		{
			return;
		}
		if (PhotonNetwork.isMasterClient)
		{
			StartCoroutine(CoWaitAndResetRestarts());
		}
		roundTime = 0f;
		if (Level.Name.StartsWith("Custom"))
		{
			CustomLevelLoaded = false;
		}
		if (PhotonNetwork.isMasterClient)
		{
			if (isFirstLoad)
			{
				SetGameSettings(CheckGameGUI());
			}
			if (RCSettings.EndlessMode > 0)
			{
				StartCoroutine(CoRespawn(RCSettings.EndlessMode));
			}
		}
		if ((int)Settings[244] == 1)
		{
			InRoomChat.Instance.AddLine(("(" + roundTime.ToString("F2") + ") ").AsColor("FFCC00") + "Round Start.");
		}
		isFirstLoad = false;
		RecompilePlayerList(0.5f);
	}

	private void CoreEditor()
	{
		if (Input.GetKey(KeyCode.Tab))
		{
			GUI.FocusControl(null);
		}
		Screen.showCursor = true;
		if (selectedObj != null)
		{
			float num = 0.2f;
			if (InputRC.isInputLevel(InputCodeRC.LevelSlow))
			{
				num = 0.04f;
			}
			else if (InputRC.isInputLevel(InputCodeRC.LevelFast))
			{
				num = 0.6f;
			}
			if (InputRC.isInputLevel(InputCodeRC.LevelForward))
			{
				selectedObj.transform.position += num * new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
			}
			else if (InputRC.isInputLevel(InputCodeRC.LevelBack))
			{
				selectedObj.transform.position -= num * new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
			}
			if (InputRC.isInputLevel(InputCodeRC.LevelLeft))
			{
				selectedObj.transform.position -= num * new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z);
			}
			else if (InputRC.isInputLevel(InputCodeRC.LevelRight))
			{
				selectedObj.transform.position += num * new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z);
			}
			if (InputRC.isInputLevel(InputCodeRC.LevelDown))
			{
				selectedObj.transform.position -= Vector3.up * num;
			}
			else if (InputRC.isInputLevel(InputCodeRC.LevelUp))
			{
				selectedObj.transform.position += Vector3.up * num;
			}
			if (!selectedObj.name.StartsWith("misc,region"))
			{
				if (InputRC.isInputLevel(InputCodeRC.LevelRRight))
				{
					selectedObj.transform.Rotate(Vector3.up * num);
				}
				else if (InputRC.isInputLevel(InputCodeRC.LevelRLeft))
				{
					selectedObj.transform.Rotate(Vector3.down * num);
				}
				if (InputRC.isInputLevel(InputCodeRC.LevelRCCW))
				{
					selectedObj.transform.Rotate(Vector3.forward * num);
				}
				else if (InputRC.isInputLevel(InputCodeRC.LevelRCW))
				{
					selectedObj.transform.Rotate(Vector3.back * num);
				}
				if (InputRC.isInputLevel(InputCodeRC.LevelRBack))
				{
					selectedObj.transform.Rotate(Vector3.left * num);
				}
				else if (InputRC.isInputLevel(InputCodeRC.LevelRForward))
				{
					selectedObj.transform.Rotate(Vector3.right * num);
				}
			}
			if (InputRC.isInputLevel(InputCodeRC.LevelPlace))
			{
				LinkHash[3].Add(selectedObj.GetInstanceID(), selectedObj.name + "," + Convert.ToString(selectedObj.transform.position.x) + "," + Convert.ToString(selectedObj.transform.position.y) + "," + Convert.ToString(selectedObj.transform.position.z) + "," + Convert.ToString(selectedObj.transform.rotation.x) + "," + Convert.ToString(selectedObj.transform.rotation.y) + "," + Convert.ToString(selectedObj.transform.rotation.z) + "," + Convert.ToString(selectedObj.transform.rotation.w));
				selectedObj = null;
				Camera.main.GetComponent<MouseLook>().enabled = true;
				Screen.lockCursor = true;
			}
			if (InputRC.isInputLevel(InputCodeRC.LevelDelete))
			{
				UnityEngine.Object.Destroy(selectedObj);
				selectedObj = null;
				Camera.main.GetComponent<MouseLook>().enabled = true;
				Screen.lockCursor = true;
				LinkHash[3].Remove(selectedObj.GetInstanceID());
			}
			return;
		}
		if (Screen.lockCursor)
		{
			float num2 = 100f;
			if (InputRC.isInputLevel(InputCodeRC.LevelSlow))
			{
				num2 = 20f;
			}
			else if (InputRC.isInputLevel(InputCodeRC.LevelFast))
			{
				num2 = 400f;
			}
			Transform transform = Camera.main.transform;
			if (InputRC.isInputLevel(InputCodeRC.LevelForward))
			{
				transform.position += transform.forward * num2 * Time.deltaTime;
			}
			else if (InputRC.isInputLevel(InputCodeRC.LevelBack))
			{
				transform.position -= transform.forward * num2 * Time.deltaTime;
			}
			if (InputRC.isInputLevel(InputCodeRC.LevelLeft))
			{
				transform.position -= transform.right * num2 * Time.deltaTime;
			}
			else if (InputRC.isInputLevel(InputCodeRC.LevelRight))
			{
				transform.position += transform.right * num2 * Time.deltaTime;
			}
			if (InputRC.isInputLevel(InputCodeRC.LevelUp))
			{
				transform.position += transform.up * num2 * Time.deltaTime;
			}
			else if (InputRC.isInputLevel(InputCodeRC.LevelDown))
			{
				transform.position -= transform.up * num2 * Time.deltaTime;
			}
		}
		if (InputRC.isInputLevelDown(InputCodeRC.LevelCursor))
		{
			if (Screen.lockCursor)
			{
				Camera.main.GetComponent<MouseLook>().enabled = false;
				Screen.lockCursor = false;
			}
			else
			{
				Camera.main.GetComponent<MouseLook>().enabled = true;
				Screen.lockCursor = true;
			}
		}
		if (Input.GetKeyDown(KeyCode.Mouse0) && !Screen.lockCursor && GUIUtility.hotControl == 0 && ((Input.mousePosition.x > 300f && Input.mousePosition.x < (float)Screen.width - 300f) || (float)Screen.height - Input.mousePosition.y > 600f) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo))
		{
			Transform transform2 = hitInfo.transform;
			if (transform2.gameObject.name.StartsWith("custom") || transform2.gameObject.name.StartsWith("base") || transform2.gameObject.name.StartsWith("racing") || transform2.gameObject.name.StartsWith("photon") || transform2.gameObject.name.StartsWith("spawnpoint") || transform2.gameObject.name.StartsWith("misc"))
			{
				selectedObj = transform2.gameObject;
				Camera.main.GetComponent<MouseLook>().enabled = false;
				Screen.lockCursor = true;
				LinkHash[3].Remove(selectedObj.GetInstanceID());
			}
			else if (transform2.parent.gameObject.name.StartsWith("custom") || transform2.parent.gameObject.name.StartsWith("base") || transform2.parent.gameObject.name.StartsWith("racing") || transform2.parent.gameObject.name.StartsWith("photon"))
			{
				selectedObj = transform2.parent.gameObject;
				Camera.main.GetComponent<MouseLook>().enabled = false;
				Screen.lockCursor = true;
				LinkHash[3].Remove(selectedObj.GetInstanceID());
			}
		}
	}

	private void LoadConfig()
	{
		object[] array = new object[270]
		{
			PlayerPrefs.GetInt("human", 1),
			PlayerPrefs.GetInt("titan", 1),
			PlayerPrefs.GetInt("level", 1),
			PlayerPrefs.GetString("horse", string.Empty),
			PlayerPrefs.GetString("hair", string.Empty),
			PlayerPrefs.GetString("eye", string.Empty),
			PlayerPrefs.GetString("glass", string.Empty),
			PlayerPrefs.GetString("face", string.Empty),
			PlayerPrefs.GetString("skin", string.Empty),
			PlayerPrefs.GetString("costume", string.Empty),
			PlayerPrefs.GetString("logo", string.Empty),
			PlayerPrefs.GetString("bladel", string.Empty),
			PlayerPrefs.GetString("blader", string.Empty),
			PlayerPrefs.GetString("gas", string.Empty),
			PlayerPrefs.GetString("hoodie", string.Empty),
			PlayerPrefs.GetInt("gasenable", 0),
			PlayerPrefs.GetInt("titantype1", -1),
			PlayerPrefs.GetInt("titantype2", -1),
			PlayerPrefs.GetInt("titantype3", -1),
			PlayerPrefs.GetInt("titantype4", -1),
			PlayerPrefs.GetInt("titantype5", -1),
			PlayerPrefs.GetString("titanhair1", string.Empty),
			PlayerPrefs.GetString("titanhair2", string.Empty),
			PlayerPrefs.GetString("titanhair3", string.Empty),
			PlayerPrefs.GetString("titanhair4", string.Empty),
			PlayerPrefs.GetString("titanhair5", string.Empty),
			PlayerPrefs.GetString("titaneye1", string.Empty),
			PlayerPrefs.GetString("titaneye2", string.Empty),
			PlayerPrefs.GetString("titaneye3", string.Empty),
			PlayerPrefs.GetString("titaneye4", string.Empty),
			PlayerPrefs.GetString("titaneye5", string.Empty),
			0,
			PlayerPrefs.GetInt("titanR", 0),
			PlayerPrefs.GetString("tree1", string.Empty),
			PlayerPrefs.GetString("tree2", string.Empty),
			PlayerPrefs.GetString("tree3", string.Empty),
			PlayerPrefs.GetString("tree4", string.Empty),
			PlayerPrefs.GetString("tree5", string.Empty),
			PlayerPrefs.GetString("tree6", string.Empty),
			PlayerPrefs.GetString("tree7", string.Empty),
			PlayerPrefs.GetString("tree8", string.Empty),
			PlayerPrefs.GetString("leaf1", string.Empty),
			PlayerPrefs.GetString("leaf2", string.Empty),
			PlayerPrefs.GetString("leaf3", string.Empty),
			PlayerPrefs.GetString("leaf4", string.Empty),
			PlayerPrefs.GetString("leaf5", string.Empty),
			PlayerPrefs.GetString("leaf6", string.Empty),
			PlayerPrefs.GetString("leaf7", string.Empty),
			PlayerPrefs.GetString("leaf8", string.Empty),
			PlayerPrefs.GetString("forestG", string.Empty),
			PlayerPrefs.GetInt("forestR", 0),
			PlayerPrefs.GetString("house1", string.Empty),
			PlayerPrefs.GetString("house2", string.Empty),
			PlayerPrefs.GetString("house3", string.Empty),
			PlayerPrefs.GetString("house4", string.Empty),
			PlayerPrefs.GetString("house5", string.Empty),
			PlayerPrefs.GetString("house6", string.Empty),
			PlayerPrefs.GetString("house7", string.Empty),
			PlayerPrefs.GetString("house8", string.Empty),
			PlayerPrefs.GetString("cityG", string.Empty),
			PlayerPrefs.GetString("cityW", string.Empty),
			PlayerPrefs.GetString("cityH", string.Empty),
			PlayerPrefs.GetInt("skinQ", 0),
			PlayerPrefs.GetInt("skinQL", 0),
			0,
			PlayerPrefs.GetString("eren", string.Empty),
			PlayerPrefs.GetString("annie", string.Empty),
			PlayerPrefs.GetString("colossal", string.Empty),
			100,
			"default",
			"1",
			"1",
			"1",
			1f,
			1f,
			1f,
			0,
			string.Empty,
			0,
			"1.0",
			"1.0",
			0,
			PlayerPrefs.GetString("cnumber", "1"),
			"30",
			0,
			PlayerPrefs.GetString("cmax", "20"),
			PlayerPrefs.GetString("titanbody1", string.Empty),
			PlayerPrefs.GetString("titanbody2", string.Empty),
			PlayerPrefs.GetString("titanbody3", string.Empty),
			PlayerPrefs.GetString("titanbody4", string.Empty),
			PlayerPrefs.GetString("titanbody5", string.Empty),
			0,
			PlayerPrefs.GetInt("traildisable", 0),
			PlayerPrefs.GetInt("wind", 0),
			PlayerPrefs.GetString("trailskin", string.Empty),
			PlayerPrefs.GetString("snapshot", "0"),
			PlayerPrefs.GetString("trailskin2", string.Empty),
			PlayerPrefs.GetInt("reel", 0),
			PlayerPrefs.GetString("reelin", "LeftControl"),
			PlayerPrefs.GetString("reelout", "LeftAlt"),
			0,
			PlayerPrefs.GetString("tforward", "W"),
			PlayerPrefs.GetString("tback", "S"),
			PlayerPrefs.GetString("tleft", "A"),
			PlayerPrefs.GetString("tright", "D"),
			PlayerPrefs.GetString("twalk", "LeftShift"),
			PlayerPrefs.GetString("tjump", "Space"),
			PlayerPrefs.GetString("tpunch", "Q"),
			PlayerPrefs.GetString("tslam", "E"),
			PlayerPrefs.GetString("tgrabfront", "Alpha1"),
			PlayerPrefs.GetString("tgrabback", "Alpha3"),
			PlayerPrefs.GetString("tgrabnape", "Mouse1"),
			PlayerPrefs.GetString("tantiae", "Mouse0"),
			PlayerPrefs.GetString("tbite", "Alpha2"),
			PlayerPrefs.GetString("tcover", "Z"),
			PlayerPrefs.GetString("tsit", "X"),
			PlayerPrefs.GetInt("reel2", 0),
			PlayerPrefs.GetString("lforward", "W"),
			PlayerPrefs.GetString("lback", "S"),
			PlayerPrefs.GetString("lleft", "A"),
			PlayerPrefs.GetString("lright", "D"),
			PlayerPrefs.GetString("lup", "Mouse1"),
			PlayerPrefs.GetString("ldown", "Mouse0"),
			PlayerPrefs.GetString("lcursor", "X"),
			PlayerPrefs.GetString("lplace", "Space"),
			PlayerPrefs.GetString("ldel", "Backspace"),
			PlayerPrefs.GetString("lslow", "LeftShift"),
			PlayerPrefs.GetString("lrforward", "R"),
			PlayerPrefs.GetString("lrback", "F"),
			PlayerPrefs.GetString("lrleft", "Q"),
			PlayerPrefs.GetString("lrright", "E"),
			PlayerPrefs.GetString("lrccw", "Z"),
			PlayerPrefs.GetString("lrcw", "C"),
			PlayerPrefs.GetInt("humangui", 0),
			PlayerPrefs.GetString("horse2", string.Empty),
			PlayerPrefs.GetString("hair2", string.Empty),
			PlayerPrefs.GetString("eye2", string.Empty),
			PlayerPrefs.GetString("glass2", string.Empty),
			PlayerPrefs.GetString("face2", string.Empty),
			PlayerPrefs.GetString("skin2", string.Empty),
			PlayerPrefs.GetString("costume2", string.Empty),
			PlayerPrefs.GetString("logo2", string.Empty),
			PlayerPrefs.GetString("bladel2", string.Empty),
			PlayerPrefs.GetString("blader2", string.Empty),
			PlayerPrefs.GetString("gas2", string.Empty),
			PlayerPrefs.GetString("hoodie2", string.Empty),
			PlayerPrefs.GetString("trail2", string.Empty),
			PlayerPrefs.GetString("horse3", string.Empty),
			PlayerPrefs.GetString("hair3", string.Empty),
			PlayerPrefs.GetString("eye3", string.Empty),
			PlayerPrefs.GetString("glass3", string.Empty),
			PlayerPrefs.GetString("face3", string.Empty),
			PlayerPrefs.GetString("skin3", string.Empty),
			PlayerPrefs.GetString("costume3", string.Empty),
			PlayerPrefs.GetString("logo3", string.Empty),
			PlayerPrefs.GetString("bladel3", string.Empty),
			PlayerPrefs.GetString("blader3", string.Empty),
			PlayerPrefs.GetString("gas3", string.Empty),
			PlayerPrefs.GetString("hoodie3", string.Empty),
			PlayerPrefs.GetString("trail3", string.Empty),
			null,
			PlayerPrefs.GetString("lfast", "LeftControl"),
			PlayerPrefs.GetString("customGround", string.Empty),
			PlayerPrefs.GetString("forestskyfront", string.Empty),
			PlayerPrefs.GetString("forestskyback", string.Empty),
			PlayerPrefs.GetString("forestskyleft", string.Empty),
			PlayerPrefs.GetString("forestskyright", string.Empty),
			PlayerPrefs.GetString("forestskyup", string.Empty),
			PlayerPrefs.GetString("forestskydown", string.Empty),
			PlayerPrefs.GetString("cityskyfront", string.Empty),
			PlayerPrefs.GetString("cityskyback", string.Empty),
			PlayerPrefs.GetString("cityskyleft", string.Empty),
			PlayerPrefs.GetString("cityskyright", string.Empty),
			PlayerPrefs.GetString("cityskyup", string.Empty),
			PlayerPrefs.GetString("cityskydown", string.Empty),
			PlayerPrefs.GetString("customskyfront", string.Empty),
			PlayerPrefs.GetString("customskyback", string.Empty),
			PlayerPrefs.GetString("customskyleft", string.Empty),
			PlayerPrefs.GetString("customskyright", string.Empty),
			PlayerPrefs.GetString("customskyup", string.Empty),
			PlayerPrefs.GetString("customskydown", string.Empty),
			PlayerPrefs.GetInt("dashenable", 0),
			PlayerPrefs.GetString("dashkey", "RightControl"),
			PlayerPrefs.GetInt("vsync", 0),
			PlayerPrefs.GetString("fpscap", "0"),
			0,
			0,
			0,
			0,
			PlayerPrefs.GetInt("speedometer", 0),
			0,
			string.Empty,
			PlayerPrefs.GetInt("bombMode", 0),
			PlayerPrefs.GetInt("teamMode", 0),
			PlayerPrefs.GetInt("rockThrow", 0),
			PlayerPrefs.GetInt("explodeModeOn", 0),
			PlayerPrefs.GetString("explodeModeNum", "30"),
			PlayerPrefs.GetInt("healthMode", 0),
			PlayerPrefs.GetString("healthLower", "100"),
			PlayerPrefs.GetString("healthUpper", "200"),
			PlayerPrefs.GetInt("infectionModeOn", 0),
			PlayerPrefs.GetString("infectionModeNum", "1"),
			PlayerPrefs.GetInt("banEren", 0),
			PlayerPrefs.GetInt("moreTitanOn", 0),
			PlayerPrefs.GetString("moreTitanNum", "1"),
			PlayerPrefs.GetInt("damageModeOn", 0),
			PlayerPrefs.GetString("damageModeNum", "1000"),
			PlayerPrefs.GetInt("sizeMode", 0),
			PlayerPrefs.GetString("sizeLower", "1.0"),
			PlayerPrefs.GetString("sizeUpper", "3.0"),
			PlayerPrefs.GetInt("spawnModeOn", 0),
			PlayerPrefs.GetString("nRate", "20.0"),
			PlayerPrefs.GetString("aRate", "20.0"),
			PlayerPrefs.GetString("jRate", "20.0"),
			PlayerPrefs.GetString("cRate", "20.0"),
			PlayerPrefs.GetString("pRate", "20.0"),
			PlayerPrefs.GetInt("horseMode", 0),
			PlayerPrefs.GetInt("waveModeOn", 0),
			PlayerPrefs.GetString("waveModeNum", "1"),
			PlayerPrefs.GetInt("friendlyMode", 0),
			PlayerPrefs.GetInt("pvpMode", 0),
			PlayerPrefs.GetInt("maxWaveOn", 0),
			PlayerPrefs.GetString("maxWaveNum", "20"),
			PlayerPrefs.GetInt("endlessModeOn", 0),
			PlayerPrefs.GetString("endlessModeNum", "10"),
			PlayerPrefs.GetString("motd", string.Empty),
			PlayerPrefs.GetInt("pointModeOn", 0),
			PlayerPrefs.GetString("pointModeNum", "50"),
			PlayerPrefs.GetInt("ahssReload", 0),
			PlayerPrefs.GetInt("punkWaves", 0),
			0,
			PlayerPrefs.GetInt("mapOn", 0),
			PlayerPrefs.GetString("mapMaximize", "Tab"),
			PlayerPrefs.GetString("mapToggle", "M"),
			PlayerPrefs.GetString("mapReset", "K"),
			PlayerPrefs.GetInt("globalDisableMinimap", 0),
			PlayerPrefs.GetString("chatRebind", "None"),
			PlayerPrefs.GetString("hforward", "W"),
			PlayerPrefs.GetString("hback", "S"),
			PlayerPrefs.GetString("hleft", "A"),
			PlayerPrefs.GetString("hright", "D"),
			PlayerPrefs.GetString("hwalk", "LeftShift"),
			PlayerPrefs.GetString("hjump", "Q"),
			PlayerPrefs.GetString("hmount", "LeftControl"),
			PlayerPrefs.GetInt("chatfeed", 0),
			0,
			PlayerPrefs.GetFloat("bombR", 1f),
			PlayerPrefs.GetFloat("bombG", 1f),
			PlayerPrefs.GetFloat("bombB", 1f),
			PlayerPrefs.GetFloat("bombA", 1f),
			PlayerPrefs.GetFloat("bombRadius", 5f),
			PlayerPrefs.GetFloat("bombRange", 3f),
			PlayerPrefs.GetFloat("bombSpeed", 5f),
			PlayerPrefs.GetFloat("bombCD", 7f),
			PlayerPrefs.GetString("cannonUp", "W"),
			PlayerPrefs.GetString("cannonDown", "S"),
			PlayerPrefs.GetString("cannonLeft", "A"),
			PlayerPrefs.GetString("cannonRight", "D"),
			PlayerPrefs.GetString("cannonFire", "Q"),
			PlayerPrefs.GetString("cannonMount", "G"),
			PlayerPrefs.GetString("cannonSlow", "LeftShift"),
			PlayerPrefs.GetInt("deadlyCannon", 0),
			PlayerPrefs.GetString("liveCam", "Y"),
			0,
			null,
			null,
			null,
			null,
			null,
			null
		};
		InputRC = new InputManagerRC();
		InputRC.setInputHuman(InputCodeRC.ReelIn, (string)array[98]);
		InputRC.setInputHuman(InputCodeRC.ReelOut, (string)array[99]);
		InputRC.setInputHuman(InputCodeRC.Dash, (string)array[182]);
		InputRC.setInputHuman(InputCodeRC.MapMaximize, (string)array[232]);
		InputRC.setInputHuman(InputCodeRC.MapToggle, (string)array[233]);
		InputRC.setInputHuman(InputCodeRC.MapReset, (string)array[234]);
		InputRC.setInputHuman(InputCodeRC.Chat, (string)array[236]);
		InputRC.setInputHuman(InputCodeRC.LiveCamera, (string)array[262]);
		if (!Enum.IsDefined(typeof(KeyCode), (string)array[232]))
		{
			array[232] = "None";
		}
		if (!Enum.IsDefined(typeof(KeyCode), (string)array[233]))
		{
			array[233] = "None";
		}
		if (!Enum.IsDefined(typeof(KeyCode), (string)array[234]))
		{
			array[234] = "None";
		}
		for (int i = 0; i < 15; i++)
		{
			InputRC.setInputTitan(i, (string)array[101 + i]);
		}
		for (int j = 0; j < 16; j++)
		{
			InputRC.setInputLevel(j, (string)array[117 + j]);
		}
		for (int k = 0; k < 7; k++)
		{
			InputRC.setInputHorse(k, (string)array[237 + k]);
		}
		for (int l = 0; l < 7; l++)
		{
			InputRC.setInputCannon(l, (string)array[254 + l]);
		}
		InputRC.setInputLevel(InputCodeRC.LevelFast, (string)array[161]);
		Application.targetFrameRate = -1;
		if (int.TryParse((string)array[184], out var result) && result > 0)
		{
			Application.targetFrameRate = result;
		}
		QualitySettings.vSyncCount = (((int)array[183] == 1) ? 1 : 0);
		AudioListener.volume = PlayerPrefs.GetFloat("vol", 1f);
		QualitySettings.masterTextureLimit = PlayerPrefs.GetInt("skinQ", 0);
		LinkHash = new ExitGames.Client.Photon.Hashtable[5]
		{
			new ExitGames.Client.Photon.Hashtable(),
			new ExitGames.Client.Photon.Hashtable(),
			new ExitGames.Client.Photon.Hashtable(),
			new ExitGames.Client.Photon.Hashtable(),
			new ExitGames.Client.Photon.Hashtable()
		};
		Settings = array;
		scroll = Vector2.zero;
		scroll2 = Vector2.zero;
		distanceSlider = PlayerPrefs.GetFloat("cameraDistance", 1f);
		mouseSlider = PlayerPrefs.GetFloat("MouseSensitivity", 0.5f);
		qualitySlider = PlayerPrefs.GetFloat("GameQuality", 0f);
		transparencySlider = 1f;
	}

	public void LoadSkin()
	{
		if ((int)Settings[64] >= 100)
		{
			string[] array = new string[5] { "Flare", "LabelInfoBottomRight", "LabelNetworkStatus", "skill_cd_bottom", "GasUI" };
			UnityEngine.Object[] array2 = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
			for (int i = 0; i < array2.Length; i++)
			{
				GameObject gameObject = (GameObject)array2[i];
				if (gameObject.name.Contains("TREE") || gameObject.name.Contains("aot_supply") || gameObject.name.Contains("gameobjectOutSide"))
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
			GameObject.Find("Cube_001").renderer.material.mainTexture = ((Material)RCAssets.Load("grass")).mainTexture;
			UnityEngine.Object.Instantiate(RCAssets.Load("spawnPlayer"), new Vector3(-10f, 1f, -10f), new Quaternion(0f, 0f, 0f, 1f));
			string[] array3 = array;
			for (int i = 0; i < array3.Length; i++)
			{
				GameObject gameObject2 = GameObject.Find(array3[i]);
				if (gameObject2 != null)
				{
					UnityEngine.Object.Destroy(gameObject2);
				}
			}
			Camera.main.GetComponent<SpectatorMovement>().disable = true;
			return;
		}
		InstantiateTracker.Instance.Dispose();
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
		{
			updateTime = 1f;
			if (OldScriptLogic != CurrentScriptLogic)
			{
				IntVariables.Clear();
				BoolVariables.Clear();
				StringVariables.Clear();
				FloatVariables.Clear();
				GlobalVariables.Clear();
				RCEvents.Clear();
				RCVariableNames.Clear();
				PlayerVariables.Clear();
				TitanVariables.Clear();
				RCRegionTriggers.Clear();
				OldScriptLogic = CurrentScriptLogic;
				CompileScript(CurrentScriptLogic);
				if (RCEvents.ContainsKey("OnFirstLoad"))
				{
					((RCEvent)RCEvents["OnFirstLoad"]).CheckEvent();
				}
			}
			if (RCEvents.ContainsKey("OnRoundStart"))
			{
				((RCEvent)RCEvents["OnRoundStart"]).CheckEvent();
			}
			base.photonView.RPC("setMasterRC", PhotonTargets.All);
		}
		LogicLoaded = true;
		racingSpawnPoint = new Vector3(0f, 0f, 0f);
		racingSpawnPointSet = false;
		racingDoors = new List<GameObject>();
		allowedToCannon = new Dictionary<int, CannonValues>();
		if (!Level.Name.StartsWith("Custom") && (int)Settings[2] == 1)
		{
			GameObject gameObject3 = GameObject.Find("aot_supply");
			if (gameObject3 != null && Minimap.Instance != null)
			{
				Minimap.Instance.TrackGameObjectOnMinimap(gameObject3, Color.white, trackOrientation: false, depthAboveAll: true, Minimap.IconStyle.SUPPLY);
			}
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			string[] array4 = new string[6]
			{
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty
			};
			if (Level.Map.Contains("City"))
			{
				for (int j = 51; j < 59; j++)
				{
					text = text + (string)Settings[j] + ",";
				}
				text.TrimEnd(',');
				for (int k = 0; k < 250; k++)
				{
					text3 += Convert.ToString((int)UnityEngine.Random.Range(0f, 8f));
				}
				text2 = (string)Settings[59] + "," + (string)Settings[60] + "," + (string)Settings[61];
				for (int l = 0; l < 6; l++)
				{
					array4[l] = (string)Settings[l + 169];
				}
			}
			else if (Level.Map.Contains("Forest"))
			{
				for (int m = 33; m < 41; m++)
				{
					text = text + (string)Settings[m] + ",";
				}
				text.TrimEnd(',');
				for (int n = 41; n < 49; n++)
				{
					text2 = text2 + (string)Settings[n] + ",";
				}
				text2 += (string)Settings[49];
				for (int num = 0; num < 150; num++)
				{
					string text4 = Convert.ToString((int)UnityEngine.Random.Range(0f, 8f));
					text3 += text4;
					text3 = (((int)Settings[50] != 0) ? (text3 + Convert.ToString((int)UnityEngine.Random.Range(0f, 8f))) : (text3 + text4));
				}
				for (int num2 = 0; num2 < 6; num2++)
				{
					array4[num2] = (string)Settings[num2 + 163];
				}
			}
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
			{
				StartCoroutine(CoLoadSkin(text3, text, text2, array4));
			}
			else if (PhotonNetwork.isMasterClient)
			{
				base.photonView.RPC("loadskinRPC", PhotonTargets.AllBuffered, text3, text, text2, array4);
			}
		}
		else
		{
			if (!Level.Name.StartsWith("Custom"))
			{
				return;
			}
			GameObject[] array5 = GameObject.FindGameObjectsWithTag("playerRespawn");
			for (int i = 0; i < array5.Length; i++)
			{
				array5[i].transform.position = new Vector3(UnityEngine.Random.Range(-5f, 5f), 0f, UnityEngine.Random.Range(-5f, 5f));
			}
			UnityEngine.Object[] array2 = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
			for (int i = 0; i < array2.Length; i++)
			{
				GameObject gameObject4 = (GameObject)array2[i];
				if (gameObject4.name.Contains("TREE") || gameObject4.name.Contains("aot_supply"))
				{
					UnityEngine.Object.Destroy(gameObject4);
				}
				else if (gameObject4.name == "Cube_001" && gameObject4.transform.parent.gameObject.tag != "player" && gameObject4.renderer != null)
				{
					groundList.Add(gameObject4);
					gameObject4.renderer.material.mainTexture = ((Material)RCAssets.Load("grass")).mainTexture;
				}
			}
			if (!PhotonNetwork.isMasterClient)
			{
				return;
			}
			string[] array6 = new string[7]
			{
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty
			};
			for (int num3 = 0; num3 < 6; num3++)
			{
				array6[num3] = (string)Settings[num3 + 175];
			}
			array6[6] = (string)Settings[162];
			if (int.TryParse((string)Settings[85], out var result))
			{
				RCSettings.TitanCap = result;
			}
			else
			{
				RCSettings.TitanCap = 0;
				Settings[85] = "0";
			}
			RCSettings.TitanCap = Math.Min(50, RCSettings.TitanCap);
			base.photonView.RPC("clearlevel", PhotonTargets.AllBuffered, array6, RCSettings.GameType);
			RCRegions.Clear();
			if (OldScript != CurrentScript)
			{
				levelCache.Clear();
				titanSpawns.Clear();
				playerSpawnsC.Clear();
				playerSpawnsM.Clear();
				titanSpawners.Clear();
				CurrentLevel = string.Empty;
				if (CurrentScript == string.Empty)
				{
					ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
					hashtable.Add(PhotonPlayerProperty.CurrentLevel, CurrentLevel);
					PhotonNetwork.player.SetCustomProperties(hashtable);
					OldScript = CurrentScript;
				}
				else
				{
					string[] array7 = Regex.Replace(CurrentScript, "\\s+", string.Empty).Replace("\r\n", string.Empty).Replace('\n', '\0')
						.Replace('\r', '\0')
						.Split(';');
					for (int num4 = 0; num4 < Mathf.FloorToInt((array7.Length - 1) / 100) + 1; num4++)
					{
						string[] array8;
						int num5;
						if (num4 < Mathf.FloorToInt(array7.Length / 100))
						{
							array8 = new string[101];
							num5 = 0;
							for (int num6 = 100 * num4; num6 < 100 * num4 + 100; num6++)
							{
								if (array7[num6].StartsWith("spawnpoint"))
								{
									string[] array9 = array7[num6].Split(',');
									switch (array9[1])
									{
									case "titan":
										titanSpawns.Add(new Vector3(Convert.ToSingle(array9[2]), Convert.ToSingle(array9[3]), Convert.ToSingle(array9[4])));
										break;
									case "playerC":
										playerSpawnsC.Add(new Vector3(Convert.ToSingle(array9[2]), Convert.ToSingle(array9[3]), Convert.ToSingle(array9[4])));
										break;
									case "playerM":
										playerSpawnsM.Add(new Vector3(Convert.ToSingle(array9[2]), Convert.ToSingle(array9[3]), Convert.ToSingle(array9[4])));
										break;
									}
								}
								array8[num5] = array7[num6];
								num5++;
							}
							CurrentLevel += (array8[100] = UnityEngine.Random.Range(10000, 99999).ToString());
							levelCache.Add(array8);
							continue;
						}
						array8 = new string[array7.Length % 100 + 1];
						num5 = 0;
						for (int num7 = 100 * num4; num7 < 100 * num4 + array7.Length % 100; num7++)
						{
							if (array7[num7].StartsWith("spawnpoint"))
							{
								string[] array10 = array7[num7].Split(',');
								switch (array10[1])
								{
								case "titan":
									titanSpawns.Add(new Vector3(Convert.ToSingle(array10[2]), Convert.ToSingle(array10[3]), Convert.ToSingle(array10[4])));
									break;
								case "playerC":
									playerSpawnsC.Add(new Vector3(Convert.ToSingle(array10[2]), Convert.ToSingle(array10[3]), Convert.ToSingle(array10[4])));
									break;
								case "playerM":
									playerSpawnsM.Add(new Vector3(Convert.ToSingle(array10[2]), Convert.ToSingle(array10[3]), Convert.ToSingle(array10[4])));
									break;
								}
							}
							array8[num5] = array7[num7];
							num5++;
						}
						string text5 = UnityEngine.Random.Range(10000, 99999).ToString();
						array8[array7.Length % 100] = text5;
						CurrentLevel += text5;
						levelCache.Add(array8);
					}
					List<string> list = new List<string>();
					foreach (Vector3 titanSpawn in titanSpawns)
					{
						string[] obj = new string[6] { "titan,", null, null, null, null, null };
						float x = titanSpawn.x;
						obj[1] = x.ToString();
						obj[2] = ",";
						x = titanSpawn.y;
						obj[3] = x.ToString();
						obj[4] = ",";
						x = titanSpawn.z;
						obj[5] = x.ToString();
						string[] array11 = obj;
						list.Add(string.Concat(array11));
					}
					foreach (Vector3 item in playerSpawnsC)
					{
						string[] obj2 = new string[6] { "playerC,", null, null, null, null, null };
						float x = item.x;
						obj2[1] = x.ToString();
						obj2[2] = ",";
						x = item.y;
						obj2[3] = x.ToString();
						obj2[4] = ",";
						x = item.z;
						obj2[5] = x.ToString();
						string[] array12 = obj2;
						list.Add(string.Concat(array12));
					}
					foreach (Vector3 item2 in playerSpawnsM)
					{
						string[] obj3 = new string[6] { "playerM,", null, null, null, null, null };
						float x = item2.x;
						obj3[1] = x.ToString();
						obj3[2] = ",";
						x = item2.y;
						obj3[3] = x.ToString();
						obj3[4] = ",";
						x = item2.z;
						obj3[5] = x.ToString();
						string[] array13 = obj3;
						list.Add(string.Concat(array13));
					}
					string text6 = "a" + UnityEngine.Random.Range(10000, 99999);
					list.Add(text6);
					CurrentLevel = text6 + CurrentLevel;
					levelCache.Insert(0, list.ToArray());
					string text7 = "z" + UnityEngine.Random.Range(10000, 99999);
					levelCache.Add(new string[1] { text7 });
					CurrentLevel += text7;
					ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable();
					hashtable2.Add(PhotonPlayerProperty.CurrentLevel, CurrentLevel);
					PhotonNetwork.player.SetCustomProperties(hashtable2);
					OldScript = CurrentScript;
				}
			}
			PhotonPlayer[] array14 = PhotonNetwork.playerList;
			foreach (PhotonPlayer photonPlayer in array14)
			{
				if (!photonPlayer.isMasterClient)
				{
					otherUsers.Add(photonPlayer);
				}
			}
			StartCoroutine(CoLoadCustomLevel(otherUsers));
			StartCoroutine(CoCacheCustomLevel());
		}
	}

	private void CustomLevelClientE(string[] content, bool renewHash)
	{
		bool flag = false;
		bool flag2 = false;
		if (content[content.Length - 1].StartsWith("a"))
		{
			flag = true;
		}
		else if (content[content.Length - 1].StartsWith("z"))
		{
			flag2 = true;
			CustomLevelLoaded = true;
			spawnPlayerCustomMap();
			Minimap.TryRecaptureInstance();
			UnloadAssets();
		}
		if (renewHash)
		{
			if (flag)
			{
				CurrentLevel = string.Empty;
				levelCache.Clear();
				titanSpawns.Clear();
				playerSpawnsC.Clear();
				playerSpawnsM.Clear();
				for (int i = 0; i < content.Length; i++)
				{
					string[] array = content[i].Split(',');
					if (array[0] == "titan")
					{
						titanSpawns.Add(new Vector3(Convert.ToSingle(array[1]), Convert.ToSingle(array[2]), Convert.ToSingle(array[3])));
					}
					else if (array[0] == "playerC")
					{
						playerSpawnsC.Add(new Vector3(Convert.ToSingle(array[1]), Convert.ToSingle(array[2]), Convert.ToSingle(array[3])));
					}
					else if (array[0] == "playerM")
					{
						playerSpawnsM.Add(new Vector3(Convert.ToSingle(array[1]), Convert.ToSingle(array[2]), Convert.ToSingle(array[3])));
					}
				}
				spawnPlayerCustomMap();
			}
			CurrentLevel += content[content.Length - 1];
			levelCache.Add(content);
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add(PhotonPlayerProperty.CurrentLevel, CurrentLevel);
			PhotonNetwork.player.SetCustomProperties(hashtable);
		}
		if (flag || flag2)
		{
			return;
		}
		for (int i = 0; i < content.Length; i++)
		{
			GameObject gameObject = null;
			string[] array = content[i].Split(',');
			float result;
			if (array[0].StartsWith("custom"))
			{
				float a = 1f;
				GameObject gameObject2 = null;
				gameObject2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load(array[1]), new Vector3(Convert.ToSingle(array[12]), Convert.ToSingle(array[13]), Convert.ToSingle(array[14])), new Quaternion(Convert.ToSingle(array[15]), Convert.ToSingle(array[16]), Convert.ToSingle(array[17]), Convert.ToSingle(array[18])));
				if (array[2] != "default")
				{
					if (array[2].StartsWith("transparent"))
					{
						if (float.TryParse(array[2].Substring(11), out result))
						{
							a = result;
						}
						Renderer[] componentsInChildren = gameObject2.GetComponentsInChildren<Renderer>();
						foreach (Renderer renderer in componentsInChildren)
						{
							renderer.material = (Material)RCAssets.Load("transparent");
							if (Convert.ToSingle(array[10]) != 1f || Convert.ToSingle(array[11]) != 1f)
							{
								renderer.material.mainTextureScale = new Vector2(renderer.material.mainTextureScale.x * Convert.ToSingle(array[10]), renderer.material.mainTextureScale.y * Convert.ToSingle(array[11]));
							}
						}
					}
					else
					{
						Renderer[] componentsInChildren = gameObject2.GetComponentsInChildren<Renderer>();
						foreach (Renderer renderer2 in componentsInChildren)
						{
							renderer2.material = (Material)RCAssets.Load(array[2]);
							if (Convert.ToSingle(array[10]) != 1f || Convert.ToSingle(array[11]) != 1f)
							{
								renderer2.material.mainTextureScale = new Vector2(renderer2.material.mainTextureScale.x * Convert.ToSingle(array[10]), renderer2.material.mainTextureScale.y * Convert.ToSingle(array[11]));
							}
						}
					}
				}
				float num = gameObject2.transform.localScale.x * Convert.ToSingle(array[3]);
				num -= 0.001f;
				float y = gameObject2.transform.localScale.y * Convert.ToSingle(array[4]);
				float z = gameObject2.transform.localScale.z * Convert.ToSingle(array[5]);
				gameObject2.transform.localScale = new Vector3(num, y, z);
				if (array[6] != "0")
				{
					Color color = new Color(Convert.ToSingle(array[7]), Convert.ToSingle(array[8]), Convert.ToSingle(array[9]), a);
					MeshFilter[] componentsInChildren2 = gameObject2.GetComponentsInChildren<MeshFilter>();
					for (int j = 0; j < componentsInChildren2.Length; j++)
					{
						Mesh mesh = componentsInChildren2[j].mesh;
						Color[] array2 = new Color[mesh.vertexCount];
						for (int k = 0; k < mesh.vertexCount; k++)
						{
							array2[k] = color;
						}
						mesh.colors = array2;
					}
				}
				gameObject = gameObject2;
			}
			else if (array[0].StartsWith("base"))
			{
				if (array.Length < 15)
				{
					gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load(array[1]), new Vector3(Convert.ToSingle(array[2]), Convert.ToSingle(array[3]), Convert.ToSingle(array[4])), new Quaternion(Convert.ToSingle(array[5]), Convert.ToSingle(array[6]), Convert.ToSingle(array[7]), Convert.ToSingle(array[8])));
				}
				else
				{
					float a = 1f;
					GameObject gameObject2 = null;
					gameObject2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)Resources.Load(array[1]), new Vector3(Convert.ToSingle(array[12]), Convert.ToSingle(array[13]), Convert.ToSingle(array[14])), new Quaternion(Convert.ToSingle(array[15]), Convert.ToSingle(array[16]), Convert.ToSingle(array[17]), Convert.ToSingle(array[18])));
					if (array[2] != "default")
					{
						if (array[2].StartsWith("transparent"))
						{
							if (float.TryParse(array[2].Substring(11), out result))
							{
								a = result;
							}
							Renderer[] componentsInChildren = gameObject2.GetComponentsInChildren<Renderer>();
							foreach (Renderer renderer3 in componentsInChildren)
							{
								renderer3.material = (Material)RCAssets.Load("transparent");
								if (Convert.ToSingle(array[10]) != 1f || Convert.ToSingle(array[11]) != 1f)
								{
									renderer3.material.mainTextureScale = new Vector2(renderer3.material.mainTextureScale.x * Convert.ToSingle(array[10]), renderer3.material.mainTextureScale.y * Convert.ToSingle(array[11]));
								}
							}
						}
						else
						{
							Renderer[] componentsInChildren = gameObject2.GetComponentsInChildren<Renderer>();
							foreach (Renderer renderer4 in componentsInChildren)
							{
								if (!renderer4.name.Contains("Particle System") || !gameObject2.name.Contains("aot_supply"))
								{
									renderer4.material = (Material)RCAssets.Load(array[2]);
									if (Convert.ToSingle(array[10]) != 1f || Convert.ToSingle(array[11]) != 1f)
									{
										renderer4.material.mainTextureScale = new Vector2(renderer4.material.mainTextureScale.x * Convert.ToSingle(array[10]), renderer4.material.mainTextureScale.y * Convert.ToSingle(array[11]));
									}
								}
							}
						}
					}
					float num = gameObject2.transform.localScale.x * Convert.ToSingle(array[3]);
					num -= 0.001f;
					float y = gameObject2.transform.localScale.y * Convert.ToSingle(array[4]);
					float z = gameObject2.transform.localScale.z * Convert.ToSingle(array[5]);
					gameObject2.transform.localScale = new Vector3(num, y, z);
					if (array[6] != "0")
					{
						Color color = new Color(Convert.ToSingle(array[7]), Convert.ToSingle(array[8]), Convert.ToSingle(array[9]), a);
						MeshFilter[] componentsInChildren2 = gameObject2.GetComponentsInChildren<MeshFilter>();
						for (int j = 0; j < componentsInChildren2.Length; j++)
						{
							Mesh mesh = componentsInChildren2[j].mesh;
							Color[] array2 = new Color[mesh.vertexCount];
							for (int k = 0; k < mesh.vertexCount; k++)
							{
								array2[k] = color;
							}
							mesh.colors = array2;
						}
					}
					gameObject = gameObject2;
				}
			}
			else if (array[0].StartsWith("misc"))
			{
				if (array[1].StartsWith("barrier"))
				{
					GameObject gameObject2 = null;
					gameObject2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load(array[1]), new Vector3(Convert.ToSingle(array[5]), Convert.ToSingle(array[6]), Convert.ToSingle(array[7])), new Quaternion(Convert.ToSingle(array[8]), Convert.ToSingle(array[9]), Convert.ToSingle(array[10]), Convert.ToSingle(array[11])));
					float num = gameObject2.transform.localScale.x * Convert.ToSingle(array[2]);
					num -= 0.001f;
					float y = gameObject2.transform.localScale.y * Convert.ToSingle(array[3]);
					float z = gameObject2.transform.localScale.z * Convert.ToSingle(array[4]);
					gameObject2.transform.localScale = new Vector3(num, y, z);
					gameObject = gameObject2;
				}
				else if (array[1].StartsWith("racingStart"))
				{
					GameObject gameObject2 = null;
					gameObject2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load(array[1]), new Vector3(Convert.ToSingle(array[5]), Convert.ToSingle(array[6]), Convert.ToSingle(array[7])), new Quaternion(Convert.ToSingle(array[8]), Convert.ToSingle(array[9]), Convert.ToSingle(array[10]), Convert.ToSingle(array[11])));
					float num = gameObject2.transform.localScale.x * Convert.ToSingle(array[2]);
					num -= 0.001f;
					float y = gameObject2.transform.localScale.y * Convert.ToSingle(array[3]);
					float z = gameObject2.transform.localScale.z * Convert.ToSingle(array[4]);
					gameObject2.transform.localScale = new Vector3(num, y, z);
					if (racingDoors != null)
					{
						racingDoors.Add(gameObject2);
					}
					gameObject = gameObject2;
				}
				else if (array[1].StartsWith("racingEnd"))
				{
					GameObject gameObject2 = null;
					gameObject2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load(array[1]), new Vector3(Convert.ToSingle(array[5]), Convert.ToSingle(array[6]), Convert.ToSingle(array[7])), new Quaternion(Convert.ToSingle(array[8]), Convert.ToSingle(array[9]), Convert.ToSingle(array[10]), Convert.ToSingle(array[11])));
					float num = gameObject2.transform.localScale.x * Convert.ToSingle(array[2]);
					num -= 0.001f;
					float y = gameObject2.transform.localScale.y * Convert.ToSingle(array[3]);
					float z = gameObject2.transform.localScale.z * Convert.ToSingle(array[4]);
					gameObject2.transform.localScale = new Vector3(num, y, z);
					gameObject2.AddComponent<LevelTriggerRacingEnd>();
					gameObject = gameObject2;
				}
				else if (array[1].StartsWith("region") && PhotonNetwork.isMasterClient)
				{
					Vector3 vector = new Vector3(Convert.ToSingle(array[6]), Convert.ToSingle(array[7]), Convert.ToSingle(array[8]));
					RCRegion rCRegion = new RCRegion(vector, Convert.ToSingle(array[3]), Convert.ToSingle(array[4]), Convert.ToSingle(array[5]));
					string key = array[2];
					if (RCRegionTriggers.ContainsKey(key))
					{
						GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load("region"));
						gameObject3.transform.position = vector;
						gameObject3.AddComponent<RegionTrigger>();
						gameObject3.GetComponent<RegionTrigger>().CopyTrigger((RegionTrigger)RCRegionTriggers[key]);
						float num = gameObject3.transform.localScale.x * Convert.ToSingle(array[3]);
						num -= 0.001f;
						float y = gameObject3.transform.localScale.y * Convert.ToSingle(array[4]);
						float z = gameObject3.transform.localScale.z * Convert.ToSingle(array[5]);
						gameObject3.transform.localScale = new Vector3(num, y, z);
						rCRegion.myBox = gameObject3;
					}
					RCRegions.Add(key, rCRegion);
				}
			}
			else if (array[0].StartsWith("racing"))
			{
				if (array[1].StartsWith("start"))
				{
					GameObject gameObject2 = null;
					gameObject2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load(array[1]), new Vector3(Convert.ToSingle(array[5]), Convert.ToSingle(array[6]), Convert.ToSingle(array[7])), new Quaternion(Convert.ToSingle(array[8]), Convert.ToSingle(array[9]), Convert.ToSingle(array[10]), Convert.ToSingle(array[11])));
					float num = gameObject2.transform.localScale.x * Convert.ToSingle(array[2]);
					num -= 0.001f;
					float y = gameObject2.transform.localScale.y * Convert.ToSingle(array[3]);
					float z = gameObject2.transform.localScale.z * Convert.ToSingle(array[4]);
					gameObject2.transform.localScale = new Vector3(num, y, z);
					if (racingDoors != null)
					{
						racingDoors.Add(gameObject2);
					}
					gameObject = gameObject2;
				}
				else if (array[1].StartsWith("end"))
				{
					GameObject gameObject2 = null;
					gameObject2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load(array[1]), new Vector3(Convert.ToSingle(array[5]), Convert.ToSingle(array[6]), Convert.ToSingle(array[7])), new Quaternion(Convert.ToSingle(array[8]), Convert.ToSingle(array[9]), Convert.ToSingle(array[10]), Convert.ToSingle(array[11])));
					float num = gameObject2.transform.localScale.x * Convert.ToSingle(array[2]);
					num -= 0.001f;
					float y = gameObject2.transform.localScale.y * Convert.ToSingle(array[3]);
					float z = gameObject2.transform.localScale.z * Convert.ToSingle(array[4]);
					gameObject2.transform.localScale = new Vector3(num, y, z);
					gameObject2.GetComponentInChildren<Collider>().gameObject.AddComponent<LevelTriggerRacingEnd>();
					gameObject = gameObject2;
				}
				else if (array[1].StartsWith("kill"))
				{
					GameObject gameObject2 = null;
					gameObject2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load(array[1]), new Vector3(Convert.ToSingle(array[5]), Convert.ToSingle(array[6]), Convert.ToSingle(array[7])), new Quaternion(Convert.ToSingle(array[8]), Convert.ToSingle(array[9]), Convert.ToSingle(array[10]), Convert.ToSingle(array[11])));
					float num = gameObject2.transform.localScale.x * Convert.ToSingle(array[2]);
					num -= 0.001f;
					float y = gameObject2.transform.localScale.y * Convert.ToSingle(array[3]);
					float z = gameObject2.transform.localScale.z * Convert.ToSingle(array[4]);
					gameObject2.transform.localScale = new Vector3(num, y, z);
					gameObject2.GetComponentInChildren<Collider>().gameObject.AddComponent<RacingKillTrigger>();
					gameObject = gameObject2;
				}
				else if (array[1].StartsWith("checkpoint"))
				{
					GameObject gameObject2 = null;
					gameObject2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load(array[1]), new Vector3(Convert.ToSingle(array[5]), Convert.ToSingle(array[6]), Convert.ToSingle(array[7])), new Quaternion(Convert.ToSingle(array[8]), Convert.ToSingle(array[9]), Convert.ToSingle(array[10]), Convert.ToSingle(array[11])));
					float num = gameObject2.transform.localScale.x * Convert.ToSingle(array[2]);
					num -= 0.001f;
					float y = gameObject2.transform.localScale.y * Convert.ToSingle(array[3]);
					float z = gameObject2.transform.localScale.z * Convert.ToSingle(array[4]);
					gameObject2.transform.localScale = new Vector3(num, y, z);
					gameObject2.GetComponentInChildren<Collider>().gameObject.AddComponent<RacingCheckpointTrigger>();
					gameObject = gameObject2;
				}
			}
			else if (array[0].StartsWith("map"))
			{
				if (array[1].StartsWith("disablebounds"))
				{
					UnityEngine.Object.Destroy(GameObject.Find("gameobjectOutSide"));
					UnityEngine.Object.Instantiate(RCAssets.Load("outside"));
				}
			}
			else if (PhotonNetwork.isMasterClient && array[0].StartsWith("photon"))
			{
				if (array[1].StartsWith("Cannon"))
				{
					if (array.Length > 15)
					{
						GameObject obj = PhotonNetwork.Instantiate("RCAsset/" + array[1] + "Prop", new Vector3(Convert.ToSingle(array[12]), Convert.ToSingle(array[13]), Convert.ToSingle(array[14])), new Quaternion(Convert.ToSingle(array[15]), Convert.ToSingle(array[16]), Convert.ToSingle(array[17]), Convert.ToSingle(array[18])), 0);
						obj.GetComponent<CannonPropRegion>().settings = content[i];
						obj.GetPhotonView().RPC("SetSize", PhotonTargets.AllBuffered, content[i]);
					}
					else
					{
						PhotonNetwork.Instantiate("RCAsset/" + array[1] + "Prop", new Vector3(Convert.ToSingle(array[2]), Convert.ToSingle(array[3]), Convert.ToSingle(array[4])), new Quaternion(Convert.ToSingle(array[5]), Convert.ToSingle(array[6]), Convert.ToSingle(array[7]), Convert.ToSingle(array[8])), 0).GetComponent<CannonPropRegion>().settings = content[i];
					}
				}
				else
				{
					TitanSpawner titanSpawner = new TitanSpawner();
					float num = 30f;
					if (float.TryParse(array[2], out result))
					{
						num = Mathf.Max(Convert.ToSingle(array[2]), 1f);
					}
					titanSpawner.time = num;
					titanSpawner.delay = num;
					titanSpawner.name = array[1];
					if (array[3] == "1")
					{
						titanSpawner.endless = true;
					}
					else
					{
						titanSpawner.endless = false;
					}
					titanSpawner.location = new Vector3(Convert.ToSingle(array[4]), Convert.ToSingle(array[5]), Convert.ToSingle(array[6]));
					titanSpawners.Add(titanSpawner);
				}
			}
			if (gameObject != null)
			{
				CustomAnarchyLevel.Instance.TryAddAnarchyScripts(gameObject, array);
			}
		}
	}

	private IEnumerator CoCacheCustomLevel()
	{
		for (int i = 0; i < levelCache.Count; i++)
		{
			CustomLevelClientE(levelCache[i], renewHash: false);
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator CoLoadCustomLevel(List<PhotonPlayer> players)
	{
		if (CurrentLevel == string.Empty)
		{
			string[] array = new string[1] { "loadempty" };
			foreach (PhotonPlayer player in players)
			{
				base.photonView.RPC("customlevelRPC", player, new object[1] { array });
			}
			CustomLevelLoaded = true;
			yield break;
		}
		for (int i = 0; i < levelCache.Count; i++)
		{
			foreach (PhotonPlayer player2 in players)
			{
				if (player2.customProperties[PhotonPlayerProperty.CurrentLevel] != null && CurrentLevel.Length > 0 && GExtensions.AsString(player2.customProperties[PhotonPlayerProperty.CurrentLevel]) == CurrentLevel)
				{
					if (i == 0)
					{
						string[] array2 = new string[1] { "loadcached" };
						base.photonView.RPC("customlevelRPC", player2, new object[1] { array2 });
					}
				}
				else
				{
					base.photonView.RPC("customlevelRPC", player2, new object[1] { levelCache[i] });
				}
			}
			if (i > 0)
			{
				yield return new WaitForSeconds(0.75f);
			}
			else
			{
				yield return new WaitForSeconds(0.25f);
			}
		}
	}

	private IEnumerator CoClearLevel(string[] skybox)
	{
		string linkGround = skybox[6];
		bool mipmapping = true;
		bool unload = false;
		if ((int)Settings[63] == 1)
		{
			mipmapping = false;
		}
		if (skybox[0] != string.Empty || skybox[1] != string.Empty || skybox[2] != string.Empty || skybox[3] != string.Empty || skybox[4] != string.Empty || skybox[5] != string.Empty)
		{
			string key = string.Join(",", skybox);
			if (!LinkHash[1].ContainsKey(key))
			{
				unload = true;
				Material newSky = new Material(Camera.main.GetComponent<Skybox>().material);
				string text = skybox[0];
				string skyBack = skybox[1];
				string skyLeft = skybox[2];
				string skyRight = skybox[3];
				string skyUp = skybox[4];
				string skyDown = skybox[5];
				if (text.EndsWith(".jpg") || text.EndsWith(".png") || text.EndsWith(".jpeg"))
				{
					WWW www7 = SkinChecker.CreateWWW(text);
					if (www7 != null)
					{
						using (www7)
						{
							yield return www7;
							Texture2D texture = RCextensions.LoadImage(www7, mipmapping, 2000000);
							newSky.SetTexture("_FrontTex", texture);
						}
					}
				}
				if (skyBack.EndsWith(".jpg") || skyBack.EndsWith(".png") || skyBack.EndsWith(".jpeg"))
				{
					WWW www7 = SkinChecker.CreateWWW(skyBack);
					if (www7 != null)
					{
						using (www7)
						{
							yield return www7;
							Texture2D texture2 = RCextensions.LoadImage(www7, mipmapping, 2000000);
							newSky.SetTexture("_BackTex", texture2);
						}
					}
				}
				if (skyLeft.EndsWith(".jpg") || skyLeft.EndsWith(".png") || skyLeft.EndsWith(".jpeg"))
				{
					WWW www7 = SkinChecker.CreateWWW(skyLeft);
					if (www7 != null)
					{
						using (www7)
						{
							yield return www7;
							Texture2D texture3 = RCextensions.LoadImage(www7, mipmapping, 2000000);
							newSky.SetTexture("_LeftTex", texture3);
						}
					}
				}
				if (skyRight.EndsWith(".jpg") || skyRight.EndsWith(".png") || skyRight.EndsWith(".jpeg"))
				{
					WWW www7 = SkinChecker.CreateWWW(skyRight);
					if (www7 != null)
					{
						using (www7)
						{
							yield return www7;
							Texture2D texture4 = RCextensions.LoadImage(www7, mipmapping, 2000000);
							newSky.SetTexture("_RightTex", texture4);
						}
					}
				}
				if (skyUp.EndsWith(".jpg") || skyUp.EndsWith(".png") || skyUp.EndsWith(".jpeg"))
				{
					WWW www7 = SkinChecker.CreateWWW(skyUp);
					if (www7 != null)
					{
						using (www7)
						{
							yield return www7;
							Texture2D texture5 = RCextensions.LoadImage(www7, mipmapping, 2000000);
							newSky.SetTexture("_UpTex", texture5);
						}
					}
				}
				if (skyDown.EndsWith(".jpg") || skyDown.EndsWith(".png") || skyDown.EndsWith(".jpeg"))
				{
					WWW www7 = SkinChecker.CreateWWW(skyDown);
					if (www7 != null)
					{
						using (www7)
						{
							yield return www7;
							Texture2D texture6 = RCextensions.LoadImage(www7, mipmapping, 2000000);
							newSky.SetTexture("_DownTex", texture6);
						}
					}
				}
				Camera.main.GetComponent<Skybox>().material = newSky;
				LinkHash[1].Add(key, newSky);
				SkyMaterial = newSky;
			}
			else
			{
				Camera.main.GetComponent<Skybox>().material = (Material)LinkHash[1][key];
				SkyMaterial = (Material)LinkHash[1][key];
			}
		}
		if (linkGround.EndsWith(".jpg") || linkGround.EndsWith(".png") || linkGround.EndsWith(".jpeg"))
		{
			foreach (GameObject ground in groundList)
			{
				if (!(ground != null) || !(ground.renderer != null))
				{
					continue;
				}
				Renderer[] componentsInChildren = ground.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in componentsInChildren)
				{
					if (!LinkHash[0].ContainsKey(linkGround))
					{
						WWW www7 = SkinChecker.CreateWWW(linkGround);
						if (www7 != null)
						{
							using (www7)
							{
								yield return www7;
								Texture2D mainTexture = RCextensions.LoadImage(www7, mipmapping, 200000);
								unload = true;
								renderer.material.mainTexture = mainTexture;
								LinkHash[0].Add(linkGround, renderer.material);
							}
							renderer.material = (Material)LinkHash[0][linkGround];
						}
					}
					else
					{
						renderer.material = (Material)LinkHash[0][linkGround];
					}
				}
			}
		}
		else if (linkGround.ToLower() == "transparent")
		{
			foreach (GameObject ground2 in groundList)
			{
				if (ground2 != null && ground2.renderer != null)
				{
					Renderer[] componentsInChildren2 = ground2.GetComponentsInChildren<Renderer>();
					for (int j = 0; j < componentsInChildren2.Length; j++)
					{
						componentsInChildren2[j].enabled = false;
					}
				}
			}
		}
		if (unload)
		{
			UnloadAssets();
		}
	}

	[RPC]
	private void customlevelRPC(string[] content, PhotonMessageInfo info)
	{
		if (info.sender.isMasterClient)
		{
			if (content.Length == 1 && content[0] == "loadcached")
			{
				StartCoroutine(CoCacheCustomLevel());
			}
			else if (content.Length == 1 && content[0] == "loadempty")
			{
				CurrentLevel = string.Empty;
				levelCache.Clear();
				titanSpawns.Clear();
				playerSpawnsC.Clear();
				playerSpawnsM.Clear();
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable.Add(PhotonPlayerProperty.CurrentLevel, CurrentLevel);
				PhotonNetwork.player.SetCustomProperties(hashtable);
				CustomLevelLoaded = true;
				spawnPlayerCustomMap();
			}
			else
			{
				CustomLevelClientE(content, renewHash: true);
			}
		}
	}

	[RPC]
	private void clearlevel(string[] link, int gametype, PhotonMessageInfo info)
	{
		if (info.sender.isMasterClient)
		{
			switch (gametype)
			{
			case 0:
				IN_GAME_MAIN_CAMERA.Gamemode = GameMode.KillTitans;
				break;
			case 1:
				IN_GAME_MAIN_CAMERA.Gamemode = GameMode.Survival;
				break;
			case 2:
				IN_GAME_MAIN_CAMERA.Gamemode = GameMode.TeamDeathmatch;
				break;
			case 3:
				IN_GAME_MAIN_CAMERA.Gamemode = GameMode.Racing;
				break;
			case 4:
				IN_GAME_MAIN_CAMERA.Gamemode = GameMode.None;
				break;
			}
			if (link.Length > 6 && (int)Settings[2] == 1)
			{
				StartCoroutine(CoClearLevel(link));
			}
		}
	}

	[RPC]
	private void loadskinRPC(string n, string url, string url2, string[] skybox, PhotonMessageInfo info)
	{
		if ((int)Settings[2] == 1 && info.sender.isMasterClient)
		{
			StartCoroutine(CoLoadSkin(n, url, url2, skybox));
		}
	}

	private IEnumerator CoLoadSkin(string n, string url, string url2, string[] skybox)
	{
		bool mipmapping = true;
		bool unload = false;
		if ((int)Settings[63] == 1)
		{
			mipmapping = false;
		}
		if (skybox.Length > 5 && (skybox[0] != string.Empty || skybox[1] != string.Empty || skybox[2] != string.Empty || skybox[3] != string.Empty || skybox[4] != string.Empty || skybox[5] != string.Empty))
		{
			string key = string.Join(",", skybox);
			if (!LinkHash[1].ContainsKey(key))
			{
				unload = true;
				Material newSky = new Material(Camera.main.GetComponent<Skybox>().material);
				string text = skybox[0];
				string skyBack = skybox[1];
				string skyLeft = skybox[2];
				string skyRight = skybox[3];
				string skyUp = skybox[4];
				string skyDown7 = skybox[5];
				if (text.EndsWith(".jpg") || text.EndsWith(".png") || text.EndsWith(".jpeg"))
				{
					WWW www13 = SkinChecker.CreateWWW(text);
					if (www13 != null)
					{
						using (www13)
						{
							yield return www13;
							Texture2D texture2D = RCextensions.LoadImage(www13, mipmapping, 2000000);
							texture2D.wrapMode = TextureWrapMode.Clamp;
							newSky.SetTexture("_FrontTex", texture2D);
						}
					}
				}
				if (skyBack.EndsWith(".jpg") || skyBack.EndsWith(".png") || skyBack.EndsWith(".jpeg"))
				{
					WWW www13 = SkinChecker.CreateWWW(skyBack);
					if (www13 != null)
					{
						using (www13)
						{
							yield return www13;
							Texture2D texture2D2 = RCextensions.LoadImage(www13, mipmapping, 2000000);
							texture2D2.wrapMode = TextureWrapMode.Clamp;
							newSky.SetTexture("_BackTex", texture2D2);
						}
					}
				}
				if (skyLeft.EndsWith(".jpg") || skyLeft.EndsWith(".png") || skyLeft.EndsWith(".jpeg"))
				{
					WWW www13 = SkinChecker.CreateWWW(skyLeft);
					if (www13 != null)
					{
						using (www13)
						{
							yield return www13;
							Texture2D texture2D3 = RCextensions.LoadImage(www13, mipmapping, 2000000);
							texture2D3.wrapMode = TextureWrapMode.Clamp;
							newSky.SetTexture("_LeftTex", texture2D3);
						}
					}
				}
				if (skyRight.EndsWith(".jpg") || skyRight.EndsWith(".png") || skyRight.EndsWith(".jpeg"))
				{
					WWW www13 = SkinChecker.CreateWWW(skyRight);
					if (www13 != null)
					{
						using (www13)
						{
							yield return www13;
							Texture2D texture2D4 = RCextensions.LoadImage(www13, mipmapping, 2000000);
							texture2D4.wrapMode = TextureWrapMode.Clamp;
							newSky.SetTexture("_RightTex", texture2D4);
						}
					}
				}
				if (skyUp.EndsWith(".jpg") || skyUp.EndsWith(".png") || skyUp.EndsWith(".jpeg"))
				{
					WWW www13 = SkinChecker.CreateWWW(skyUp);
					if (www13 != null)
					{
						using (www13)
						{
							yield return www13;
							Texture2D texture2D5 = RCextensions.LoadImage(www13, mipmapping, 2000000);
							texture2D5.wrapMode = TextureWrapMode.Clamp;
							newSky.SetTexture("_UpTex", texture2D5);
						}
					}
				}
				if (skyDown7.EndsWith(".jpg") || skyDown7.EndsWith(".png") || skyDown7.EndsWith(".jpeg"))
				{
					WWW www13 = SkinChecker.CreateWWW(skyDown7);
					if (www13 != null)
					{
						using (www13)
						{
							yield return www13;
							Texture2D texture2D6 = RCextensions.LoadImage(www13, mipmapping, 2000000);
							texture2D6.wrapMode = TextureWrapMode.Clamp;
							newSky.SetTexture("_DownTex", texture2D6);
						}
					}
				}
				Camera.main.GetComponent<Skybox>().material = newSky;
				SkyMaterial = newSky;
				LinkHash[1].Add(key, newSky);
			}
			else
			{
				Camera.main.GetComponent<Skybox>().material = (Material)LinkHash[1][key];
				SkyMaterial = (Material)LinkHash[1][key];
			}
		}
		if (Level.Map.Contains("Forest"))
		{
			string[] strArray2 = url.Split(',');
			string[] strArray4 = url2.Split(',');
			int startIndex2 = 0;
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = (GameObject)array[i];
				if (!(gameObject != null))
				{
					continue;
				}
				if (gameObject.name.Contains("TREE") && n.Length > startIndex2 + 1)
				{
					string s = n.Substring(startIndex2, 1);
					string s2 = n.Substring(startIndex2 + 1, 1);
					if (int.TryParse(s, out var result) && int.TryParse(s2, out var result2) && result >= 0 && result < 8 && result2 >= 0 && result2 < 8 && strArray2.Length >= 8 && strArray4.Length >= 8 && strArray2[result] != null && strArray4[result2] != null)
					{
						string key = strArray2[result];
						string skyDown7 = strArray4[result2];
						Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
						foreach (Renderer renderer11 in componentsInChildren)
						{
							if (renderer11.name.Contains(S[22]))
							{
								if (!key.EndsWith(".jpg") && !key.EndsWith(".png") && !key.EndsWith(".jpeg"))
								{
									continue;
								}
								if (!LinkHash[2].ContainsKey(key))
								{
									WWW www13 = SkinChecker.CreateWWW(key);
									if (www13 == null)
									{
										continue;
									}
									using (www13)
									{
										yield return www13;
										Texture2D mainTexture = RCextensions.LoadImage(www13, mipmapping, 2000000);
										if (!LinkHash[2].ContainsKey(key))
										{
											unload = true;
											renderer11.material.mainTexture = mainTexture;
											LinkHash[2].Add(key, renderer11.material);
										}
									}
									renderer11.material = (Material)LinkHash[2][key];
								}
								else
								{
									renderer11.material = (Material)LinkHash[2][key];
								}
							}
							else
							{
								if (!renderer11.name.Contains(S[23]))
								{
									continue;
								}
								if (skyDown7.EndsWith(".jpg") || skyDown7.EndsWith(".png") || skyDown7.EndsWith(".jpeg"))
								{
									if (!LinkHash[0].ContainsKey(skyDown7))
									{
										WWW www13 = SkinChecker.CreateWWW(skyDown7);
										if (www13 == null)
										{
											continue;
										}
										using (www13)
										{
											yield return www13;
											Texture2D mainTexture2 = RCextensions.LoadImage(www13, mipmapping, 500000);
											if (!LinkHash[0].ContainsKey(skyDown7))
											{
												unload = true;
												renderer11.material.mainTexture = mainTexture2;
												LinkHash[0].Add(skyDown7, renderer11.material);
											}
										}
										renderer11.material = (Material)LinkHash[0][skyDown7];
									}
									else
									{
										renderer11.material = (Material)LinkHash[0][skyDown7];
									}
								}
								else if (skyDown7.ToLower() == "transparent")
								{
									renderer11.enabled = false;
								}
							}
						}
					}
					startIndex2 += 2;
				}
				else
				{
					if (!gameObject.name.Contains("Cube_001") || !(gameObject.transform.parent.gameObject.tag != "Player") || strArray4.Length <= 8 || strArray4[8] == null)
					{
						continue;
					}
					string skyDown7 = strArray4[8];
					if (skyDown7.EndsWith(".jpg") || skyDown7.EndsWith(".png") || skyDown7.EndsWith(".jpeg"))
					{
						Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
						foreach (Renderer renderer11 in componentsInChildren)
						{
							if (!LinkHash[0].ContainsKey(skyDown7))
							{
								WWW www13 = SkinChecker.CreateWWW(skyDown7);
								if (www13 == null)
								{
									continue;
								}
								using (www13)
								{
									yield return www13;
									Texture2D mainTexture3 = RCextensions.LoadImage(www13, mipmapping, 500000);
									if (!LinkHash[0].ContainsKey(skyDown7))
									{
										unload = true;
										renderer11.material.mainTexture = mainTexture3;
										LinkHash[0].Add(skyDown7, renderer11.material);
									}
								}
								renderer11.material = (Material)LinkHash[0][skyDown7];
							}
							else
							{
								renderer11.material = (Material)LinkHash[0][skyDown7];
							}
						}
					}
					else if (skyDown7.ToLower() == "transparent")
					{
						Renderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<Renderer>();
						for (int k = 0; k < componentsInChildren2.Length; k++)
						{
							componentsInChildren2[k].enabled = false;
						}
					}
				}
			}
		}
		else if (Level.Map.Contains("City"))
		{
			string[] strArray4 = url.Split(',');
			string[] strArray2 = url2.Split(',');
			int startIndex2 = 0;
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject2 = (GameObject)array[i];
				if (!(gameObject2 != null) || !gameObject2.name.Contains("Cube_") || !(gameObject2.transform.parent.gameObject.tag != "Player"))
				{
					continue;
				}
				if (gameObject2.name.EndsWith("001"))
				{
					if (strArray2.Length == 0 || strArray2[0] == null)
					{
						continue;
					}
					string skyDown7 = strArray2[0];
					if (skyDown7.EndsWith(".jpg") || skyDown7.EndsWith(".png") || skyDown7.EndsWith(".jpeg"))
					{
						Renderer[] componentsInChildren = gameObject2.GetComponentsInChildren<Renderer>();
						foreach (Renderer renderer11 in componentsInChildren)
						{
							if (!LinkHash[0].ContainsKey(skyDown7))
							{
								WWW www13 = SkinChecker.CreateWWW(skyDown7);
								if (www13 == null)
								{
									continue;
								}
								using (www13)
								{
									yield return www13;
									Texture2D mainTexture4 = RCextensions.LoadImage(www13, mipmapping, 500000);
									if (!LinkHash[0].ContainsKey(skyDown7))
									{
										unload = true;
										renderer11.material.mainTexture = mainTexture4;
										LinkHash[0].Add(skyDown7, renderer11.material);
									}
								}
								renderer11.material = (Material)LinkHash[0][skyDown7];
							}
							else
							{
								renderer11.material = (Material)LinkHash[0][skyDown7];
							}
						}
					}
					else if (skyDown7.ToLower() == "transparent")
					{
						Renderer[] componentsInChildren2 = gameObject2.GetComponentsInChildren<Renderer>();
						for (int k = 0; k < componentsInChildren2.Length; k++)
						{
							componentsInChildren2[k].enabled = false;
						}
					}
				}
				else if (gameObject2.name.EndsWith("006") || gameObject2.name.EndsWith("007") || gameObject2.name.EndsWith("015") || gameObject2.name.EndsWith("000") || (gameObject2.name.EndsWith("002") && gameObject2.transform.position.x == 0f && gameObject2.transform.position.y == 0f && gameObject2.transform.position.z == 0f))
				{
					if (strArray2.Length == 0 || strArray2[1] == null)
					{
						continue;
					}
					string skyDown7 = strArray2[1];
					if (!skyDown7.EndsWith(".jpg") && !skyDown7.EndsWith(".png") && !skyDown7.EndsWith(".jpeg"))
					{
						continue;
					}
					Renderer[] componentsInChildren = gameObject2.GetComponentsInChildren<Renderer>();
					foreach (Renderer renderer11 in componentsInChildren)
					{
						if (!LinkHash[0].ContainsKey(skyDown7))
						{
							WWW www13 = SkinChecker.CreateWWW(skyDown7);
							if (www13 == null)
							{
								continue;
							}
							using (www13)
							{
								yield return www13;
								Texture2D mainTexture5 = RCextensions.LoadImage(www13, mipmapping, 500000);
								if (!LinkHash[0].ContainsKey(skyDown7))
								{
									unload = true;
									renderer11.material.mainTexture = mainTexture5;
									LinkHash[0].Add(skyDown7, renderer11.material);
								}
							}
							renderer11.material = (Material)LinkHash[0][skyDown7];
						}
						else
						{
							renderer11.material = (Material)LinkHash[0][skyDown7];
						}
					}
				}
				else if (gameObject2.name.EndsWith("005") || gameObject2.name.EndsWith("003") || (gameObject2.name.EndsWith("002") && (gameObject2.transform.position.x != 0f || gameObject2.transform.position.y != 0f || gameObject2.transform.position.z != 0f) && n.Length > startIndex2))
				{
					if (int.TryParse(n.Substring(startIndex2, 1), out var result3) && result3 >= 0 && result3 < 8 && strArray4.Length >= 8 && strArray4[result3] != null)
					{
						string skyDown7 = strArray4[result3];
						if (skyDown7.EndsWith(".jpg") || skyDown7.EndsWith(".png") || skyDown7.EndsWith(".jpeg"))
						{
							Renderer[] componentsInChildren = gameObject2.GetComponentsInChildren<Renderer>();
							foreach (Renderer renderer11 in componentsInChildren)
							{
								if (!LinkHash[2].ContainsKey(skyDown7))
								{
									WWW www13 = SkinChecker.CreateWWW(skyDown7);
									if (www13 == null)
									{
										continue;
									}
									using (www13)
									{
										yield return www13;
										Texture2D mainTexture6 = RCextensions.LoadImage(www13, mipmapping, 2000000);
										if (!LinkHash[2].ContainsKey(skyDown7))
										{
											unload = true;
											renderer11.material.mainTexture = mainTexture6;
											LinkHash[2].Add(skyDown7, renderer11.material);
										}
									}
									renderer11.material = (Material)LinkHash[2][skyDown7];
								}
								else
								{
									renderer11.material = (Material)LinkHash[2][skyDown7];
								}
							}
						}
					}
					startIndex2++;
				}
				else
				{
					if ((!gameObject2.name.EndsWith("019") && !gameObject2.name.EndsWith("020")) || strArray2.Length <= 2 || strArray2[2] == null)
					{
						continue;
					}
					string skyDown7 = strArray2[2];
					if (!skyDown7.EndsWith(".jpg") && !skyDown7.EndsWith(".png") && !skyDown7.EndsWith(".jpeg"))
					{
						continue;
					}
					Renderer[] componentsInChildren = gameObject2.GetComponentsInChildren<Renderer>();
					foreach (Renderer renderer11 in componentsInChildren)
					{
						if (!LinkHash[2].ContainsKey(skyDown7))
						{
							WWW www13 = SkinChecker.CreateWWW(skyDown7);
							if (www13 == null)
							{
								continue;
							}
							using (www13)
							{
								yield return www13;
								Texture2D mainTexture7 = RCextensions.LoadImage(www13, mipmapping, 2000000);
								if (!LinkHash[2].ContainsKey(skyDown7))
								{
									unload = true;
									renderer11.material.mainTexture = mainTexture7;
									LinkHash[2].Add(skyDown7, renderer11.material);
								}
							}
							renderer11.material = (Material)LinkHash[2][skyDown7];
						}
						else
						{
							renderer11.material = (Material)LinkHash[2][skyDown7];
						}
					}
				}
			}
		}
		Minimap.TryRecaptureInstance();
		if (unload)
		{
			UnloadAssets();
		}
	}

	public void OnGUI()
	{
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Stop && Application.loadedLevelName != "characterCreation" && Application.loadedLevelName != "SnapShot")
		{
			if (IsAssetLoaded)
			{
				if (GameObject.Find("VERSION").GetComponent<UILabel>().text == null || !(GameObject.Find("ButtonCREDITS") != null) || !(GameObject.Find("ButtonCREDITS").transform.parent.gameObject != null) || !NGUITools.GetActive(GameObject.Find("ButtonCREDITS").transform.parent.gameObject))
				{
					return;
				}
				if (ResourceLoader.TryGetAsset<Texture2D>("Textures/ko-fi.png", out var value) && GUI.Button(new Rect(10f, 185f, 220f, 75f), value))
				{
					Application.OpenURL("https://www.ko-fi.com/winnpixie");
				}
				if (ResourceLoader.TryGetAsset<Texture2D>("Textures/patreon.png", out var value2) && GUI.Button(new Rect(10f, 265f, 220f, 150f), value2))
				{
					Application.OpenURL("https://www.patreon.com/aottg2");
				}
				float num = (float)Screen.width / 2f - 85f;
				_ = (float)Screen.height / 2f;
				GUI.Box(new Rect(num, 5f, 150f, 105f), string.Empty);
				if (GUI.Button(new Rect(num + 11f, 15f, 128f, 25f), "Level Editor"))
				{
					Settings[64] = 101;
					Application.LoadLevel(2);
				}
				else if (GUI.Button(new Rect(num + 11f, 45f, 128f, 25f), "Server: " + NetworkHelper.App.Name))
				{
					if (NetworkHelper.App == PhotonApplication.AoTTG2)
					{
						NetworkHelper.App = PhotonApplication.Custom;
					}
					else
					{
						NetworkHelper.App = PhotonApplication.AoTTG2;
					}
				}
				else if (GUI.Button(new Rect(num + 11f, 75f, 128f, 25f), "Protocol: " + NetworkHelper.Connection.Name))
				{
					switch (NetworkHelper.Connection.Protocol)
					{
					case ConnectionProtocol.Tcp:
						NetworkHelper.Connection = PhotonConnection.UDP;
						break;
					case ConnectionProtocol.Udp:
						NetworkHelper.Connection = PhotonConnection.TCP;
						break;
					}
					PhotonNetwork.SwitchToProtocol(NetworkHelper.Connection.Protocol);
				}
				GUI.Box(new Rect(10f, 30f, 220f, 150f), string.Empty);
				if (GUI.Button(new Rect(17.5f, 40f, 40f, 25f), "Login"))
				{
					Settings[187] = 0;
				}
				else if (GUI.Button(new Rect(65f, 40f, 95f, 25f), "Custom Name"))
				{
					Settings[187] = 1;
				}
				else if (GUI.Button(new Rect(167.5f, 40f, 55f, 25f), "Servers"))
				{
					Settings[187] = 2;
				}
				switch ((int)Settings[187])
				{
				case 0:
					if (LoginFengKAI.LoginState == LoginState.LoggedIn)
					{
						GUI.Label(new Rect(20f, 80f, 70f, 20f), "Username:", "Label");
						GUI.Label(new Rect(90f, 80f, 90f, 20f), LoginFengKAI.Player.Name, "Label");
						GUI.Label(new Rect(20f, 105f, 45f, 20f), "Guild:", "Label");
						LoginFengKAI.Player.Guild = GUI.TextField(new Rect(65f, 105f, 145f, 20f), LoginFengKAI.Player.Guild, 40);
						if (GUI.Button(new Rect(35f, 140f, 70f, 25f), "Set Guild"))
						{
							StartCoroutine(CoSetGuild());
						}
						else if (GUI.Button(new Rect(130f, 140f, 65f, 25f), "Logout"))
						{
							LoginFengKAI.LoginState = LoginState.LoggedOut;
						}
						break;
					}
					GUI.Label(new Rect(20f, 80f, 70f, 20f), "Username:", "Label");
					UsernameField = GUI.TextField(new Rect(90f, 80f, 130f, 20f), UsernameField, 40);
					GUI.Label(new Rect(20f, 105f, 70f, 20f), "Password:", "Label");
					PasswordField = GUI.PasswordField(new Rect(90f, 105f, 130f, 20f), PasswordField, '*', 40);
					if (GUI.Button(new Rect(30f, 140f, 50f, 25f), "Login") && LoginFengKAI.LoginState != LoginState.LoggingIn)
					{
						StartCoroutine(CoLogin());
						LoginFengKAI.LoginState = LoginState.LoggingIn;
					}
					if (LoginFengKAI.LoginState == LoginState.LoggingIn)
					{
						GUI.Label(new Rect(100f, 140f, 120f, 25f), "Logging in...", "Label");
					}
					else if (LoginFengKAI.LoginState == LoginState.Failed)
					{
						GUI.Label(new Rect(100f, 140f, 120f, 25f), "Login Failed.", "Label");
					}
					break;
				case 1:
					if (LoginFengKAI.LoginState == LoginState.LoggedIn)
					{
						GUI.Label(new Rect(30f, 80f, 180f, 60f), "You're already logged in!", "Label");
						break;
					}
					GUI.Label(new Rect(20f, 80f, 45f, 20f), "Name:", "Label");
					NameField = GUI.TextField(new Rect(65f, 80f, 145f, 20f), NameField, 255);
					GUI.Label(new Rect(20f, 105f, 45f, 20f), "Guild:", "Label");
					LoginFengKAI.Player.Guild = GUI.TextField(new Rect(65f, 105f, 145f, 20f), LoginFengKAI.Player.Guild, 255);
					if (GUI.Button(new Rect(42f, 140f, 50f, 25f), "Save"))
					{
						PlayerPrefs.SetString("name", NameField);
						PlayerPrefs.SetString("guildname", LoginFengKAI.Player.Guild);
					}
					else if (GUI.Button(new Rect(128f, 140f, 50f, 25f), "Load"))
					{
						NameField = PlayerPrefs.GetString("name", string.Empty);
						LoginFengKAI.Player.Guild = PlayerPrefs.GetString("guildname", string.Empty);
					}
					break;
				case 2:
					if (UIMainReferences.Version == UIMainReferences.FengVersion)
					{
						GUI.Label(new Rect(37f, 75f, 190f, 25f), "Connected to public server.", "Label");
					}
					else if (UIMainReferences.Version == S[0])
					{
						GUI.Label(new Rect(28f, 75f, 190f, 25f), "Connected to RC private server.", "Label");
					}
					else
					{
						GUI.Label(new Rect(37f, 75f, 190f, 25f), "Connected to custom server.", "Label");
					}
					GUI.Label(new Rect(20f, 100f, 90f, 25f), "Public Server:", "Label");
					GUI.Label(new Rect(20f, 125f, 80f, 25f), "RC Private:", "Label");
					GUI.Label(new Rect(20f, 150f, 60f, 25f), "Custom:", "Label");
					if (GUI.Button(new Rect(160f, 100f, 60f, 20f), "Connect"))
					{
						UIMainReferences.Version = UIMainReferences.FengVersion;
					}
					else if (GUI.Button(new Rect(160f, 125f, 60f, 20f), "Connect"))
					{
						UIMainReferences.Version = S[0];
					}
					else if (GUI.Button(new Rect(160f, 150f, 60f, 20f), "Connect"))
					{
						UIMainReferences.Version = PrivateServerField;
					}
					PrivateServerField = GUI.TextField(new Rect(78f, 153f, 70f, 18f), PrivateServerField, 50);
					break;
				}
			}
			else
			{
				float num2 = (float)(Screen.width / 2) - 115f;
				float num3 = (float)(Screen.height / 2) - 45f;
				GUI.Box(new Rect(num2, num3, 230f, 90f), string.Empty);
				GUI.Label(new Rect(num2 + 13f, num3 + 20f, 172f, 70f), "Downloading assets. Clear your cache or try another RC-based mod if this takes longer than 10 seconds.");
			}
		}
		else
		{
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Stop)
			{
				return;
			}
			if ((int)Settings[64] >= 100)
			{
				float num4 = (float)Screen.width - 300f;
				bool flag = false;
				bool flag2 = false;
				GUI.Box(new Rect(5f, 5f, 295f, 590f), string.Empty);
				GUI.Box(new Rect(num4, 5f, 295f, 590f), string.Empty);
				if (GUI.Button(new Rect(10f, 10f, 60f, 25f), "Script"))
				{
					Settings[68] = 100;
				}
				if (GUI.Button(new Rect(75f, 10f, 65f, 25f), "Controls"))
				{
					Settings[68] = 101;
				}
				if (GUI.Button(new Rect(210f, 10f, 80f, 25f), "Full Screen"))
				{
					Screen.fullScreen = !Screen.fullScreen;
					if (Screen.fullScreen)
					{
						Screen.SetResolution(960, 600, fullscreen: false);
					}
					else
					{
						Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullscreen: true);
					}
				}
				if ((int)Settings[68] == 100 || (int)Settings[68] == 102)
				{
					GUI.Label(new Rect(115f, 40f, 100f, 20f), "Level Script:", "Label");
					GUI.Label(new Rect(115f, 115f, 100f, 20f), "Import Data", "Label");
					GUI.Label(new Rect(12f, 535f, 280f, 60f), "Warning: your current level will be lost if you quit or import data. Make sure to save the level to a text document.", "Label");
					Settings[77] = GUI.TextField(new Rect(10f, 140f, 285f, 350f), (string)Settings[77]);
					if (GUI.Button(new Rect(35f, 500f, 60f, 30f), "Apply"))
					{
						UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
						for (int i = 0; i < array.Length; i++)
						{
							GameObject gameObject = (GameObject)array[i];
							if (gameObject.name.StartsWith("custom") || gameObject.name.StartsWith("base") || gameObject.name.StartsWith("photon") || gameObject.name.StartsWith("spawnpoint") || gameObject.name.StartsWith("misc") || gameObject.name.StartsWith("racing"))
							{
								UnityEngine.Object.Destroy(gameObject);
							}
						}
						LinkHash[3].Clear();
						Settings[186] = 0;
						string[] array2 = Regex.Replace((string)Settings[77], "\\s+", string.Empty).Replace("\r\n", string.Empty).Replace("\n", string.Empty)
							.Replace("\r", string.Empty)
							.Split(';');
						for (int j = 0; j < array2.Length; j++)
						{
							string[] array3 = array2[j].Split(',');
							if (array3[0].StartsWith("custom") || array3[0].StartsWith("base") || array3[0].StartsWith("photon") || array3[0].StartsWith("spawnpoint") || array3[0].StartsWith("misc") || array3[0].StartsWith("racing"))
							{
								GameObject gameObject2 = null;
								if (array3[0].StartsWith("custom"))
								{
									gameObject2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load(array3[1]), new Vector3(Convert.ToSingle(array3[12]), Convert.ToSingle(array3[13]), Convert.ToSingle(array3[14])), new Quaternion(Convert.ToSingle(array3[15]), Convert.ToSingle(array3[16]), Convert.ToSingle(array3[17]), Convert.ToSingle(array3[18])));
								}
								else if (array3[0].StartsWith("photon"))
								{
									gameObject2 = ((!array3[1].StartsWith("Cannon")) ? ((GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load(array3[1]), new Vector3(Convert.ToSingle(array3[4]), Convert.ToSingle(array3[5]), Convert.ToSingle(array3[6])), new Quaternion(Convert.ToSingle(array3[7]), Convert.ToSingle(array3[8]), Convert.ToSingle(array3[9]), Convert.ToSingle(array3[10])))) : ((array3.Length >= 15) ? ((GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load(array3[1] + "Prop"), new Vector3(Convert.ToSingle(array3[12]), Convert.ToSingle(array3[13]), Convert.ToSingle(array3[14])), new Quaternion(Convert.ToSingle(array3[15]), Convert.ToSingle(array3[16]), Convert.ToSingle(array3[17]), Convert.ToSingle(array3[18])))) : ((GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load(array3[1] + "Prop"), new Vector3(Convert.ToSingle(array3[2]), Convert.ToSingle(array3[3]), Convert.ToSingle(array3[4])), new Quaternion(Convert.ToSingle(array3[5]), Convert.ToSingle(array3[6]), Convert.ToSingle(array3[7]), Convert.ToSingle(array3[8]))))));
								}
								else if (array3[0].StartsWith("spawnpoint"))
								{
									gameObject2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load(array3[1]), new Vector3(Convert.ToSingle(array3[2]), Convert.ToSingle(array3[3]), Convert.ToSingle(array3[4])), new Quaternion(Convert.ToSingle(array3[5]), Convert.ToSingle(array3[6]), Convert.ToSingle(array3[7]), Convert.ToSingle(array3[8])));
								}
								else if (array3[0].StartsWith("base"))
								{
									gameObject2 = ((array3.Length >= 15) ? ((GameObject)UnityEngine.Object.Instantiate((GameObject)Resources.Load(array3[1]), new Vector3(Convert.ToSingle(array3[12]), Convert.ToSingle(array3[13]), Convert.ToSingle(array3[14])), new Quaternion(Convert.ToSingle(array3[15]), Convert.ToSingle(array3[16]), Convert.ToSingle(array3[17]), Convert.ToSingle(array3[18])))) : ((GameObject)UnityEngine.Object.Instantiate((GameObject)Resources.Load(array3[1]), new Vector3(Convert.ToSingle(array3[2]), Convert.ToSingle(array3[3]), Convert.ToSingle(array3[4])), new Quaternion(Convert.ToSingle(array3[5]), Convert.ToSingle(array3[6]), Convert.ToSingle(array3[7]), Convert.ToSingle(array3[8])))));
								}
								else if (array3[0].StartsWith("misc"))
								{
									if (array3[1].StartsWith("barrier"))
									{
										gameObject2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load("barrierEditor"), new Vector3(Convert.ToSingle(array3[5]), Convert.ToSingle(array3[6]), Convert.ToSingle(array3[7])), new Quaternion(Convert.ToSingle(array3[8]), Convert.ToSingle(array3[9]), Convert.ToSingle(array3[10]), Convert.ToSingle(array3[11])));
									}
									else if (array3[1].StartsWith("region"))
									{
										gameObject2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load("regionEditor"));
										gameObject2.transform.position = new Vector3(Convert.ToSingle(array3[6]), Convert.ToSingle(array3[7]), Convert.ToSingle(array3[8]));
										GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("UI/LabelNameOverHead"));
										gameObject3.name = "RegionLabel";
										gameObject3.transform.parent = gameObject2.transform;
										float y = 1f;
										if (Convert.ToSingle(array3[4]) > 100f)
										{
											y = 0.8f;
										}
										else if (Convert.ToSingle(array3[4]) > 1000f)
										{
											y = 0.5f;
										}
										gameObject3.transform.localPosition = new Vector3(0f, y, 0f);
										gameObject3.transform.localScale = new Vector3(5f / Convert.ToSingle(array3[3]), 5f / Convert.ToSingle(array3[4]), 5f / Convert.ToSingle(array3[5]));
										gameObject3.GetComponent<UILabel>().text = array3[2];
										gameObject2.AddComponent<RCRegionLabel>();
										gameObject2.GetComponent<RCRegionLabel>().myLabel = gameObject3;
									}
									else if (array3[1].StartsWith("racingStart"))
									{
										gameObject2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load("racingStart"), new Vector3(Convert.ToSingle(array3[5]), Convert.ToSingle(array3[6]), Convert.ToSingle(array3[7])), new Quaternion(Convert.ToSingle(array3[8]), Convert.ToSingle(array3[9]), Convert.ToSingle(array3[10]), Convert.ToSingle(array3[11])));
									}
									else if (array3[1].StartsWith("racingEnd"))
									{
										gameObject2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load("racingEnd"), new Vector3(Convert.ToSingle(array3[5]), Convert.ToSingle(array3[6]), Convert.ToSingle(array3[7])), new Quaternion(Convert.ToSingle(array3[8]), Convert.ToSingle(array3[9]), Convert.ToSingle(array3[10]), Convert.ToSingle(array3[11])));
									}
								}
								else if (array3[0].StartsWith("racing"))
								{
									gameObject2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCAssets.Load(array3[1]), new Vector3(Convert.ToSingle(array3[5]), Convert.ToSingle(array3[6]), Convert.ToSingle(array3[7])), new Quaternion(Convert.ToSingle(array3[8]), Convert.ToSingle(array3[9]), Convert.ToSingle(array3[10]), Convert.ToSingle(array3[11])));
								}
								if (array3[2] != "default" && (array3[0].StartsWith("custom") || (array3[0].StartsWith("base") && array3.Length > 15) || (array3[0].StartsWith("photon") && array3.Length > 15)))
								{
									Renderer[] componentsInChildren = gameObject2.GetComponentsInChildren<Renderer>();
									foreach (Renderer renderer in componentsInChildren)
									{
										if (!renderer.name.Contains("Particle System") || !gameObject2.name.Contains("aot_supply"))
										{
											renderer.material = (Material)RCAssets.Load(array3[2]);
											renderer.material.mainTextureScale = new Vector2(renderer.material.mainTextureScale.x * Convert.ToSingle(array3[10]), renderer.material.mainTextureScale.y * Convert.ToSingle(array3[11]));
										}
									}
								}
								if (array3[0].StartsWith("custom") || (array3[0].StartsWith("base") && array3.Length > 15) || (array3[0].StartsWith("photon") && array3.Length > 15))
								{
									float num5 = gameObject2.transform.localScale.x * Convert.ToSingle(array3[3]);
									num5 -= 0.001f;
									float y2 = gameObject2.transform.localScale.y * Convert.ToSingle(array3[4]);
									float z = gameObject2.transform.localScale.z * Convert.ToSingle(array3[5]);
									gameObject2.transform.localScale = new Vector3(num5, y2, z);
									if (array3[6] != "0")
									{
										Color color = new Color(Convert.ToSingle(array3[7]), Convert.ToSingle(array3[8]), Convert.ToSingle(array3[9]), 1f);
										MeshFilter[] componentsInChildren2 = gameObject2.GetComponentsInChildren<MeshFilter>();
										for (int i = 0; i < componentsInChildren2.Length; i++)
										{
											Mesh mesh = componentsInChildren2[i].mesh;
											Color[] array4 = new Color[mesh.vertexCount];
											for (int k = 0; k < mesh.vertexCount; k++)
											{
												array4[k] = color;
											}
											mesh.colors = array4;
										}
									}
									gameObject2.name = array3[0] + "," + array3[1] + "," + array3[2] + "," + array3[3] + "," + array3[4] + "," + array3[5] + "," + array3[6] + "," + array3[7] + "," + array3[8] + "," + array3[9] + "," + array3[10] + "," + array3[11];
								}
								else if (array3[0].StartsWith("misc"))
								{
									if (array3[1].StartsWith("barrier") || array3[1].StartsWith("racing"))
									{
										float num6 = gameObject2.transform.localScale.x * Convert.ToSingle(array3[2]);
										num6 -= 0.001f;
										float y3 = gameObject2.transform.localScale.y * Convert.ToSingle(array3[3]);
										float z2 = gameObject2.transform.localScale.z * Convert.ToSingle(array3[4]);
										gameObject2.transform.localScale = new Vector3(num6, y3, z2);
										gameObject2.name = array3[0] + "," + array3[1] + "," + array3[2] + "," + array3[3] + "," + array3[4];
									}
									else if (array3[1].StartsWith("region"))
									{
										float num7 = gameObject2.transform.localScale.x * Convert.ToSingle(array3[3]);
										num7 -= 0.001f;
										float y4 = gameObject2.transform.localScale.y * Convert.ToSingle(array3[4]);
										float z3 = gameObject2.transform.localScale.z * Convert.ToSingle(array3[5]);
										gameObject2.transform.localScale = new Vector3(num7, y4, z3);
										gameObject2.name = array3[0] + "," + array3[1] + "," + array3[2] + "," + array3[3] + "," + array3[4] + "," + array3[5];
									}
								}
								else if (array3[0].StartsWith("racing"))
								{
									float num8 = gameObject2.transform.localScale.x * Convert.ToSingle(array3[2]);
									num8 -= 0.001f;
									float y5 = gameObject2.transform.localScale.y * Convert.ToSingle(array3[3]);
									float z4 = gameObject2.transform.localScale.z * Convert.ToSingle(array3[4]);
									gameObject2.transform.localScale = new Vector3(num8, y5, z4);
									gameObject2.name = array3[0] + "," + array3[1] + "," + array3[2] + "," + array3[3] + "," + array3[4];
								}
								else if (array3[0].StartsWith("photon") && !array3[1].StartsWith("Cannon"))
								{
									gameObject2.name = array3[0] + "," + array3[1] + "," + array3[2] + "," + array3[3];
								}
								else
								{
									gameObject2.name = array3[0] + "," + array3[1];
								}
								LinkHash[3].Add(gameObject2.GetInstanceID(), array2[j]);
							}
							else if (array3[0].StartsWith("map") && array3[1].StartsWith("disablebounds"))
							{
								Settings[186] = 1;
								if (!LinkHash[3].ContainsKey("mapbounds"))
								{
									LinkHash[3].Add("mapbounds", "map,disablebounds");
								}
							}
						}
						UnloadAssets();
						Settings[77] = string.Empty;
					}
					else if (GUI.Button(new Rect(205f, 500f, 60f, 30f), "Exit"))
					{
						Screen.lockCursor = false;
						Screen.showCursor = true;
						IN_GAME_MAIN_CAMERA.Gametype = GameType.Stop;
						inputManager.menuOn = false;
						UnityEngine.Object.Destroy(GameObject.Find("MultiplayerManager"));
						Application.LoadLevel("menu");
					}
					else if (GUI.Button(new Rect(15f, 70f, 115f, 30f), "Copy to Clipboard"))
					{
						string text = string.Empty;
						int num9 = 0;
						foreach (string value3 in LinkHash[3].Values)
						{
							num9++;
							text = text + value3 + ";\n";
						}
						TextEditor textEditor = new TextEditor();
						textEditor.content = new GUIContent(text);
						textEditor.SelectAll();
						textEditor.Copy();
					}
					else if (GUI.Button(new Rect(175f, 70f, 115f, 30f), "View Script"))
					{
						Settings[68] = 102;
					}
					if ((int)Settings[68] == 102)
					{
						string text3 = string.Empty;
						int num10 = 0;
						foreach (string value4 in LinkHash[3].Values)
						{
							num10++;
							text3 = text3 + value4 + ";\n";
						}
						float num11 = (float)(Screen.width / 2) - 110.5f;
						float num12 = (float)(Screen.height / 2) - 250f;
						GUI.Box(new Rect(num11, num12, 221f, 500f), string.Empty);
						if (GUI.Button(new Rect(num11 + 10f, num12 + 460f, 60f, 30f), "Copy"))
						{
							TextEditor textEditor2 = new TextEditor();
							textEditor2.content = new GUIContent(text3);
							textEditor2.SelectAll();
							textEditor2.Copy();
						}
						else if (GUI.Button(new Rect(num11 + 151f, num12 + 460f, 60f, 30f), "Done"))
						{
							Settings[68] = 100;
						}
						GUI.TextArea(new Rect(num11 + 5f, num12 + 5f, 211f, 415f), text3);
						GUI.Label(new Rect(num11 + 10f, num12 + 430f, 150f, 20f), "Object Count: " + Convert.ToString(num10), "Label");
					}
				}
				else if ((int)Settings[68] == 101)
				{
					GUI.Label(new Rect(92f, 50f, 180f, 20f), "Level Editor Rebinds:", "Label");
					GUI.Label(new Rect(12f, 80f, 145f, 20f), "Forward:", "Label");
					GUI.Label(new Rect(12f, 105f, 145f, 20f), "Back:", "Label");
					GUI.Label(new Rect(12f, 130f, 145f, 20f), "Left:", "Label");
					GUI.Label(new Rect(12f, 155f, 145f, 20f), "Right:", "Label");
					GUI.Label(new Rect(12f, 180f, 145f, 20f), "Up:", "Label");
					GUI.Label(new Rect(12f, 205f, 145f, 20f), "Down:", "Label");
					GUI.Label(new Rect(12f, 230f, 145f, 20f), "Toggle Cursor:", "Label");
					GUI.Label(new Rect(12f, 255f, 145f, 20f), "Place Object:", "Label");
					GUI.Label(new Rect(12f, 280f, 145f, 20f), "Delete Object:", "Label");
					GUI.Label(new Rect(12f, 305f, 145f, 20f), "Movement-Slow:", "Label");
					GUI.Label(new Rect(12f, 330f, 145f, 20f), "Rotate Forward:", "Label");
					GUI.Label(new Rect(12f, 355f, 145f, 20f), "Rotate Backward:", "Label");
					GUI.Label(new Rect(12f, 380f, 145f, 20f), "Rotate Left:", "Label");
					GUI.Label(new Rect(12f, 405f, 145f, 20f), "Rotate Right:", "Label");
					GUI.Label(new Rect(12f, 430f, 145f, 20f), "Rotate CCW:", "Label");
					GUI.Label(new Rect(12f, 455f, 145f, 20f), "Rotate CW:", "Label");
					GUI.Label(new Rect(12f, 480f, 145f, 20f), "Movement-Speedup:", "Label");
					for (int l = 0; l < 17; l++)
					{
						float top = 80f + 25f * (float)l;
						int num13 = 117 + l;
						if (l == 16)
						{
							num13 = 161;
						}
						if (GUI.Button(new Rect(135f, top, 60f, 20f), (string)Settings[num13]))
						{
							Settings[num13] = "waiting...";
							Settings[100] = num13;
						}
					}
					if ((int)Settings[100] != 0)
					{
						Event current = Event.current;
						bool flag3 = false;
						string text5 = "waiting...";
						if (current.type == EventType.KeyDown && current.keyCode != 0)
						{
							flag3 = true;
							text5 = current.keyCode.ToString();
						}
						else if (Input.GetKey(KeyCode.LeftShift))
						{
							flag3 = true;
							text5 = KeyCode.LeftShift.ToString();
						}
						else if (Input.GetKey(KeyCode.RightShift))
						{
							flag3 = true;
							text5 = KeyCode.RightShift.ToString();
						}
						else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
						{
							if (Input.GetAxis("Mouse ScrollWheel") > 0f)
							{
								flag3 = true;
								text5 = "Scroll Up";
							}
							else
							{
								flag3 = true;
								text5 = "Scroll Down";
							}
						}
						else
						{
							for (int m = 0; m < 7; m++)
							{
								if (Input.GetKeyDown((KeyCode)(323 + m)))
								{
									flag3 = true;
									text5 = "Mouse" + Convert.ToString(m);
								}
							}
						}
						if (flag3)
						{
							for (int n = 0; n < 17; n++)
							{
								int num14 = 117 + n;
								if (n == 16)
								{
									num14 = 161;
								}
								if ((int)Settings[100] == num14)
								{
									Settings[num14] = text5;
									Settings[100] = 0;
									InputRC.setInputLevel(n, text5);
								}
							}
						}
					}
					if (GUI.Button(new Rect(100f, 515f, 110f, 30f), "Save Controls"))
					{
						PlayerPrefs.SetString("lforward", (string)Settings[117]);
						PlayerPrefs.SetString("lback", (string)Settings[118]);
						PlayerPrefs.SetString("lleft", (string)Settings[119]);
						PlayerPrefs.SetString("lright", (string)Settings[120]);
						PlayerPrefs.SetString("lup", (string)Settings[121]);
						PlayerPrefs.SetString("ldown", (string)Settings[122]);
						PlayerPrefs.SetString("lcursor", (string)Settings[123]);
						PlayerPrefs.SetString("lplace", (string)Settings[124]);
						PlayerPrefs.SetString("ldel", (string)Settings[125]);
						PlayerPrefs.SetString("lslow", (string)Settings[126]);
						PlayerPrefs.SetString("lrforward", (string)Settings[127]);
						PlayerPrefs.SetString("lrback", (string)Settings[128]);
						PlayerPrefs.SetString("lrleft", (string)Settings[129]);
						PlayerPrefs.SetString("lrright", (string)Settings[130]);
						PlayerPrefs.SetString("lrccw", (string)Settings[131]);
						PlayerPrefs.SetString("lrcw", (string)Settings[132]);
						PlayerPrefs.SetString("lfast", (string)Settings[161]);
					}
				}
				if ((int)Settings[64] != 105 && (int)Settings[64] != 106)
				{
					GUI.Label(new Rect(num4 + 13f, 445f, 125f, 20f), "Scale Multipliers:", "Label");
					GUI.Label(new Rect(num4 + 13f, 470f, 50f, 22f), "Length:", "Label");
					Settings[72] = GUI.TextField(new Rect(num4 + 58f, 470f, 40f, 20f), (string)Settings[72]);
					GUI.Label(new Rect(num4 + 13f, 495f, 50f, 20f), "Width:", "Label");
					Settings[70] = GUI.TextField(new Rect(num4 + 58f, 495f, 40f, 20f), (string)Settings[70]);
					GUI.Label(new Rect(num4 + 13f, 520f, 50f, 22f), "Height:", "Label");
					Settings[71] = GUI.TextField(new Rect(num4 + 58f, 520f, 40f, 20f), (string)Settings[71]);
					if ((int)Settings[64] <= 106)
					{
						GUI.Label(new Rect(num4 + 155f, 554f, 50f, 22f), "Tiling:", "Label");
						Settings[79] = GUI.TextField(new Rect(num4 + 200f, 554f, 40f, 20f), (string)Settings[79]);
						Settings[80] = GUI.TextField(new Rect(num4 + 245f, 554f, 40f, 20f), (string)Settings[80]);
						GUI.Label(new Rect(num4 + 219f, 570f, 10f, 22f), "x:", "Label");
						GUI.Label(new Rect(num4 + 264f, 570f, 10f, 22f), "y:", "Label");
						GUI.Label(new Rect(num4 + 155f, 445f, 50f, 20f), "Color:", "Label");
						GUI.Label(new Rect(num4 + 155f, 470f, 10f, 20f), "R:", "Label");
						GUI.Label(new Rect(num4 + 155f, 495f, 10f, 20f), "G:", "Label");
						GUI.Label(new Rect(num4 + 155f, 520f, 10f, 20f), "B:", "Label");
						Settings[73] = GUI.HorizontalSlider(new Rect(num4 + 170f, 475f, 100f, 20f), (float)Settings[73], 0f, 1f);
						Settings[74] = GUI.HorizontalSlider(new Rect(num4 + 170f, 500f, 100f, 20f), (float)Settings[74], 0f, 1f);
						Settings[75] = GUI.HorizontalSlider(new Rect(num4 + 170f, 525f, 100f, 20f), (float)Settings[75], 0f, 1f);
						GUI.Label(new Rect(num4 + 13f, 554f, 57f, 22f), "Material:", "Label");
						if (GUI.Button(new Rect(num4 + 66f, 554f, 60f, 20f), (string)Settings[69]))
						{
							Settings[78] = 1;
						}
						if ((int)Settings[78] == 1)
						{
							string[] item = new string[4] { "bark", "bark2", "bark3", "bark4" };
							string[] item2 = new string[4] { "wood1", "wood2", "wood3", "wood4" };
							string[] item3 = new string[4] { "grass", "grass2", "grass3", "grass4" };
							string[] item4 = new string[4] { "brick1", "brick2", "brick3", "brick4" };
							string[] item5 = new string[4] { "metal1", "metal2", "metal3", "metal4" };
							string[] item6 = new string[3] { "rock1", "rock2", "rock3" };
							string[] item7 = new string[10] { "stone1", "stone2", "stone3", "stone4", "stone5", "stone6", "stone7", "stone8", "stone9", "stone10" };
							string[] item8 = new string[7] { "earth1", "earth2", "ice1", "lava1", "crystal1", "crystal2", "empty" };
							_ = new string[0];
							List<string[]> list = new List<string[]> { item, item2, item3, item4, item5, item6, item7, item8 };
							string[] array5 = new string[9] { "bark", "wood", "grass", "brick", "metal", "rock", "stone", "misc", "transparent" };
							int num15 = 78;
							int num16 = 69;
							float num17 = (float)(Screen.width / 2) - 110.5f;
							float num18 = (float)(Screen.height / 2) - 220f;
							int num19 = (int)Settings[185];
							float val = 10f + 104f * (float)(list[num19].Length / 3 + 1);
							val = Math.Max(val, 280f);
							GUI.Box(new Rect(num17, num18, 212f, 450f), string.Empty);
							for (int num20 = 0; num20 < list.Count; num20++)
							{
								int num21 = num20 / 3;
								int num22 = num20 % 3;
								if (GUI.Button(new Rect(num17 + 5f + 69f * (float)num22, num18 + 5f + (float)(30 * num21), 64f, 25f), array5[num20]))
								{
									Settings[185] = num20;
								}
							}
							scroll2 = GUI.BeginScrollView(new Rect(num17, num18 + 110f, 225f, 290f), scroll2, new Rect(num17, num18 + 110f, 212f, val));
							if (num19 != 8)
							{
								for (int num23 = 0; num23 < list[num19].Length; num23++)
								{
									int num24 = num23 / 3;
									int num25 = num23 % 3;
									GUI.DrawTexture(new Rect(num17 + 5f + 69f * (float)num25, num18 + 115f + 104f * (float)num24, 64f, 64f), LoadTextureRC("p" + list[num19][num23]));
									if (GUI.Button(new Rect(num17 + 5f + 69f * (float)num25, num18 + 184f + 104f * (float)num24, 64f, 30f), list[num19][num23]))
									{
										Settings[num16] = list[num19][num23];
										Settings[num15] = 0;
									}
								}
							}
							GUI.EndScrollView();
							if (GUI.Button(new Rect(num17 + 24f, num18 + 410f, 70f, 30f), "Default"))
							{
								Settings[num16] = "default";
								Settings[num15] = 0;
							}
							else if (GUI.Button(new Rect(num17 + 118f, num18 + 410f, 70f, 30f), "Done"))
							{
								Settings[num15] = 0;
							}
						}
						bool flag4 = false;
						if ((int)Settings[76] == 1)
						{
							flag4 = true;
							Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, mipmap: false);
							texture2D.SetPixel(0, 0, new Color((float)Settings[73], (float)Settings[74], (float)Settings[75], 1f));
							texture2D.Apply();
							GUI.DrawTexture(new Rect(num4 + 235f, 445f, 30f, 20f), texture2D, ScaleMode.StretchToFill);
							UnityEngine.Object.Destroy(texture2D);
						}
						bool flag5 = GUI.Toggle(new Rect(num4 + 193f, 445f, 40f, 20f), flag4, "On");
						if (flag4 != flag5)
						{
							if (flag5)
							{
								Settings[76] = 1;
							}
							else
							{
								Settings[76] = 0;
							}
						}
					}
				}
				if (GUI.Button(new Rect(num4 + 5f, 10f, 60f, 25f), "General"))
				{
					Settings[64] = 101;
				}
				else if (GUI.Button(new Rect(num4 + 70f, 10f, 70f, 25f), "Geometry"))
				{
					Settings[64] = 102;
				}
				else if (GUI.Button(new Rect(num4 + 145f, 10f, 65f, 25f), "Buildings"))
				{
					Settings[64] = 103;
				}
				else if (GUI.Button(new Rect(num4 + 215f, 10f, 50f, 25f), "Nature"))
				{
					Settings[64] = 104;
				}
				else if (GUI.Button(new Rect(num4 + 5f, 45f, 70f, 25f), "Spawners"))
				{
					Settings[64] = 105;
				}
				else if (GUI.Button(new Rect(num4 + 80f, 45f, 70f, 25f), "Racing"))
				{
					Settings[64] = 108;
				}
				else if (GUI.Button(new Rect(num4 + 155f, 45f, 40f, 25f), "Misc"))
				{
					Settings[64] = 107;
				}
				else if (GUI.Button(new Rect(num4 + 200f, 45f, 70f, 25f), "Credits"))
				{
					Settings[64] = 106;
				}
				float result2;
				switch ((int)Settings[64])
				{
				case 101:
					scroll = GUI.BeginScrollView(new Rect(num4, 80f, 305f, 350f), scroll, new Rect(num4, 80f, 300f, 470f));
					GUI.Label(new Rect(num4 + 100f, 80f, 120f, 20f), "General Objects:", "Label");
					GUI.Label(new Rect(num4 + 108f, 245f, 120f, 20f), "Spawn Points:", "Label");
					GUI.Label(new Rect(num4 + 7f, 415f, 290f, 60f), "* The above titan spawn points apply only to randomly spawned titans specified by the Random Titan #.", "Label");
					GUI.Label(new Rect(num4 + 7f, 470f, 290f, 60f), "* If team mode is disabled both cyan and magenta spawn points will be randomly chosen for players.", "Label");
					GUI.DrawTexture(new Rect(num4 + 27f, 110f, 64f, 64f), LoadTextureRC("psupply"));
					GUI.DrawTexture(new Rect(num4 + 118f, 110f, 64f, 64f), LoadTextureRC("pcannonwall"));
					GUI.DrawTexture(new Rect(num4 + 209f, 110f, 64f, 64f), LoadTextureRC("pcannonground"));
					GUI.DrawTexture(new Rect(num4 + 27f, 275f, 64f, 64f), LoadTextureRC("pspawnt"));
					GUI.DrawTexture(new Rect(num4 + 118f, 275f, 64f, 64f), LoadTextureRC("pspawnplayerC"));
					GUI.DrawTexture(new Rect(num4 + 209f, 275f, 64f, 64f), LoadTextureRC("pspawnplayerM"));
					if (GUI.Button(new Rect(num4 + 27f, 179f, 64f, 60f), "Supply"))
					{
						flag = true;
						GameObject original14 = (GameObject)Resources.Load("aot_supply");
						selectedObj = (GameObject)UnityEngine.Object.Instantiate(original14);
						selectedObj.name = "base,aot_supply";
					}
					else if (GUI.Button(new Rect(num4 + 118f, 179f, 64f, 60f), "Cannon \nWall"))
					{
						flag = true;
						GameObject original15 = (GameObject)RCAssets.Load("CannonWallProp");
						selectedObj = (GameObject)UnityEngine.Object.Instantiate(original15);
						selectedObj.name = "photon,CannonWall";
					}
					else if (GUI.Button(new Rect(num4 + 209f, 179f, 64f, 60f), "Cannon\n Ground"))
					{
						flag = true;
						GameObject original16 = (GameObject)RCAssets.Load("CannonGroundProp");
						selectedObj = (GameObject)UnityEngine.Object.Instantiate(original16);
						selectedObj.name = "photon,CannonGround";
					}
					else if (GUI.Button(new Rect(num4 + 27f, 344f, 64f, 60f), "Titan"))
					{
						flag = true;
						flag2 = true;
						GameObject original17 = (GameObject)RCAssets.Load("titan");
						selectedObj = (GameObject)UnityEngine.Object.Instantiate(original17);
						selectedObj.name = "spawnpoint,titan";
					}
					else if (GUI.Button(new Rect(num4 + 118f, 344f, 64f, 60f), "Player \nCyan"))
					{
						flag = true;
						flag2 = true;
						GameObject original18 = (GameObject)RCAssets.Load("playerC");
						selectedObj = (GameObject)UnityEngine.Object.Instantiate(original18);
						selectedObj.name = "spawnpoint,playerC";
					}
					else if (GUI.Button(new Rect(num4 + 209f, 344f, 64f, 60f), "Player \nMagenta"))
					{
						flag = true;
						flag2 = true;
						GameObject original19 = (GameObject)RCAssets.Load("playerM");
						selectedObj = (GameObject)UnityEngine.Object.Instantiate(original19);
						selectedObj.name = "spawnpoint,playerM";
					}
					GUI.EndScrollView();
					break;
				case 102:
				{
					string[] array8 = new string[12]
					{
						"cuboid", "plane", "sphere", "cylinder", "capsule", "pyramid", "cone", "prism", "arc90", "arc180",
						"torus", "tube"
					};
					for (int num44 = 0; num44 < array8.Length; num44++)
					{
						int num45 = num44 % 4;
						int num46 = num44 / 4;
						GUI.DrawTexture(new Rect(num4 + 7.8f + 71.8f * (float)num45, 90f + 114f * (float)num46, 64f, 64f), LoadTextureRC("p" + array8[num44]));
						if (GUI.Button(new Rect(num4 + 7.8f + 71.8f * (float)num45, 159f + 114f * (float)num46, 64f, 30f), array8[num44]))
						{
							flag = true;
							GameObject original5 = (GameObject)RCAssets.Load(array8[num44]);
							selectedObj = (GameObject)UnityEngine.Object.Instantiate(original5);
							selectedObj.name = "custom," + array8[num44];
						}
					}
					break;
				}
				case 103:
				{
					List<string> list2 = new List<string> { "arch1", "house1" };
					string[] array9 = new string[44]
					{
						"tower1", "tower2", "tower3", "tower4", "tower5", "house1", "house2", "house3", "house4", "house5",
						"house6", "house7", "house8", "house9", "house10", "house11", "house12", "house13", "house14", "pillar1",
						"pillar2", "village1", "village2", "windmill1", "arch1", "canal1", "castle1", "church1", "cannon1", "statue1",
						"statue2", "wagon1", "elevator1", "bridge1", "dummy1", "spike1", "wall1", "wall2", "wall3", "wall4",
						"arena1", "arena2", "arena3", "arena4"
					};
					float height = 110f + 114f * (float)((array9.Length - 1) / 4);
					scroll = GUI.BeginScrollView(new Rect(num4, 90f, 303f, 350f), scroll, new Rect(num4, 90f, 300f, height));
					for (int num47 = 0; num47 < array9.Length; num47++)
					{
						int num48 = num47 % 4;
						int num49 = num47 / 4;
						GUI.DrawTexture(new Rect(num4 + 7.8f + 71.8f * (float)num48, 90f + 114f * (float)num49, 64f, 64f), LoadTextureRC("p" + array9[num47]));
						if (GUI.Button(new Rect(num4 + 7.8f + 71.8f * (float)num48, 159f + 114f * (float)num49, 64f, 30f), array9[num47]))
						{
							flag = true;
							GameObject original20 = (GameObject)RCAssets.Load(array9[num47]);
							selectedObj = (GameObject)UnityEngine.Object.Instantiate(original20);
							if (list2.Contains(array9[num47]))
							{
								selectedObj.name = "customb," + array9[num47];
							}
							else
							{
								selectedObj.name = "custom," + array9[num47];
							}
						}
					}
					GUI.EndScrollView();
					break;
				}
				case 104:
				{
					List<string> list3 = new List<string> { "tree0" };
					string[] array10 = new string[23]
					{
						"leaf0", "leaf1", "leaf2", "field1", "field2", "tree0", "tree1", "tree2", "tree3", "tree4",
						"tree5", "tree6", "tree7", "log1", "log2", "trunk1", "boulder1", "boulder2", "boulder3", "boulder4",
						"boulder5", "cave1", "cave2"
					};
					float height2 = 110f + 114f * (float)((array10.Length - 1) / 4);
					scroll = GUI.BeginScrollView(new Rect(num4, 90f, 303f, 350f), scroll, new Rect(num4, 90f, 300f, height2));
					for (int num50 = 0; num50 < array10.Length; num50++)
					{
						int num51 = num50 % 4;
						int num52 = num50 / 4;
						GUI.DrawTexture(new Rect(num4 + 7.8f + 71.8f * (float)num51, 90f + 114f * (float)num52, 64f, 64f), LoadTextureRC("p" + array10[num50]));
						if (GUI.Button(new Rect(num4 + 7.8f + 71.8f * (float)num51, 159f + 114f * (float)num52, 64f, 30f), array10[num50]))
						{
							flag = true;
							GameObject original21 = (GameObject)RCAssets.Load(array10[num50]);
							selectedObj = (GameObject)UnityEngine.Object.Instantiate(original21);
							if (list3.Contains(array10[num50]))
							{
								selectedObj.name = "customb," + array10[num50];
							}
							else
							{
								selectedObj.name = "custom," + array10[num50];
							}
						}
					}
					GUI.EndScrollView();
					break;
				}
				case 105:
				{
					GUI.Label(new Rect(num4 + 95f, 85f, 130f, 20f), "Custom Spawners:", "Label");
					GUI.DrawTexture(new Rect(num4 + 7.8f, 110f, 64f, 64f), LoadTextureRC("ptitan"));
					GUI.DrawTexture(new Rect(num4 + 79.6f, 110f, 64f, 64f), LoadTextureRC("pabnormal"));
					GUI.DrawTexture(new Rect(num4 + 151.4f, 110f, 64f, 64f), LoadTextureRC("pjumper"));
					GUI.DrawTexture(new Rect(num4 + 223.2f, 110f, 64f, 64f), LoadTextureRC("pcrawler"));
					GUI.DrawTexture(new Rect(num4 + 7.8f, 224f, 64f, 64f), LoadTextureRC("ppunk"));
					GUI.DrawTexture(new Rect(num4 + 79.6f, 224f, 64f, 64f), LoadTextureRC("pannie"));
					float result;
					if (GUI.Button(new Rect(num4 + 7.8f, 179f, 64f, 30f), "Titan"))
					{
						if (!float.TryParse((string)Settings[83], out result))
						{
							Settings[83] = "30";
						}
						flag = true;
						flag2 = true;
						GameObject original6 = (GameObject)RCAssets.Load("spawnTitan");
						selectedObj = (GameObject)UnityEngine.Object.Instantiate(original6);
						selectedObj.name = "photon,spawnTitan," + (string)Settings[83] + "," + (int)Settings[84];
					}
					else if (GUI.Button(new Rect(num4 + 79.6f, 179f, 64f, 30f), "Aberrant"))
					{
						if (!float.TryParse((string)Settings[83], out result))
						{
							Settings[83] = "30";
						}
						flag = true;
						flag2 = true;
						GameObject original7 = (GameObject)RCAssets.Load("spawnAbnormal");
						selectedObj = (GameObject)UnityEngine.Object.Instantiate(original7);
						selectedObj.name = "photon,spawnAbnormal," + (string)Settings[83] + "," + (int)Settings[84];
					}
					else if (GUI.Button(new Rect(num4 + 151.4f, 179f, 64f, 30f), "Jumper"))
					{
						if (!float.TryParse((string)Settings[83], out result))
						{
							Settings[83] = "30";
						}
						flag = true;
						flag2 = true;
						GameObject original8 = (GameObject)RCAssets.Load("spawnJumper");
						selectedObj = (GameObject)UnityEngine.Object.Instantiate(original8);
						selectedObj.name = "photon,spawnJumper," + (string)Settings[83] + "," + (int)Settings[84];
					}
					else if (GUI.Button(new Rect(num4 + 223.2f, 179f, 64f, 30f), "Crawler"))
					{
						if (!float.TryParse((string)Settings[83], out result))
						{
							Settings[83] = "30";
						}
						flag = true;
						flag2 = true;
						GameObject original9 = (GameObject)RCAssets.Load("spawnCrawler");
						selectedObj = (GameObject)UnityEngine.Object.Instantiate(original9);
						selectedObj.name = "photon,spawnCrawler," + (string)Settings[83] + "," + (int)Settings[84];
					}
					else if (GUI.Button(new Rect(num4 + 7.8f, 293f, 64f, 30f), "Punk"))
					{
						if (!float.TryParse((string)Settings[83], out result))
						{
							Settings[83] = "30";
						}
						flag = true;
						flag2 = true;
						GameObject original10 = (GameObject)RCAssets.Load("spawnPunk");
						selectedObj = (GameObject)UnityEngine.Object.Instantiate(original10);
						selectedObj.name = "photon,spawnPunk," + (string)Settings[83] + "," + (int)Settings[84];
					}
					else if (GUI.Button(new Rect(num4 + 79.6f, 293f, 64f, 30f), "Annie"))
					{
						if (!float.TryParse((string)Settings[83], out result))
						{
							Settings[83] = "30";
						}
						flag = true;
						flag2 = true;
						GameObject original11 = (GameObject)RCAssets.Load("spawnAnnie");
						selectedObj = (GameObject)UnityEngine.Object.Instantiate(original11);
						selectedObj.name = "photon,spawnAnnie," + (string)Settings[83] + "," + (int)Settings[84];
					}
					GUI.Label(new Rect(num4 + 7f, 379f, 140f, 22f), "Spawn Timer:", "Label");
					Settings[83] = GUI.TextField(new Rect(num4 + 100f, 379f, 50f, 20f), (string)Settings[83]);
					GUI.Label(new Rect(num4 + 7f, 356f, 140f, 22f), "Endless spawn:", "Label");
					GUI.Label(new Rect(num4 + 7f, 405f, 290f, 80f), "* The above settings apply only to the next placed spawner. You can have unique spawn times and settings for each individual titan spawner.", "Label");
					bool flag6 = false;
					if ((int)Settings[84] == 1)
					{
						flag6 = true;
					}
					bool flag7 = GUI.Toggle(new Rect(num4 + 100f, 356f, 40f, 20f), flag6, "On");
					if (flag6 != flag7)
					{
						if (flag7)
						{
							Settings[84] = 1;
						}
						else
						{
							Settings[84] = 0;
						}
					}
					break;
				}
				case 106:
					GUI.Label(new Rect(num4 + 10f, 80f, 200f, 22f), "- Tree 2 designed by Ken P.", "Label");
					GUI.Label(new Rect(num4 + 10f, 105f, 250f, 22f), "- Tower 2, House 5 designed by Matthew Santos", "Label");
					GUI.Label(new Rect(num4 + 10f, 130f, 200f, 22f), "- Cannon retextured by Mika", "Label");
					GUI.Label(new Rect(num4 + 10f, 155f, 200f, 22f), "- Arena 1,2,3 & 4 created by Gun", "Label");
					GUI.Label(new Rect(num4 + 10f, 180f, 250f, 22f), "- Cannon Wall/Ground textured by Bellfox", "Label");
					GUI.Label(new Rect(num4 + 10f, 205f, 250f, 120f), "- House 7 - 14, Statue1, Statue2, Wagon1, Wall 1, Wall 2, Wall 3, Wall 4, CannonWall, CannonGround, Tower5, Bridge1, Dummy1, Spike1 created by meecube", "Label");
					break;
				case 107:
				{
					GUI.DrawTexture(new Rect(num4 + 30f, 90f, 64f, 64f), LoadTextureRC("pbarrier"));
					GUI.DrawTexture(new Rect(num4 + 30f, 199f, 64f, 64f), LoadTextureRC("pregion"));
					GUI.Label(new Rect(num4 + 110f, 243f, 200f, 22f), "Region Name:", "Label");
					GUI.Label(new Rect(num4 + 110f, 179f, 200f, 22f), "Disable Map Bounds:", "Label");
					bool flag8 = false;
					if ((int)Settings[186] == 1)
					{
						flag8 = true;
						if (!LinkHash[3].ContainsKey("mapbounds"))
						{
							LinkHash[3].Add("mapbounds", "map,disablebounds");
						}
					}
					else if (LinkHash[3].ContainsKey("mapbounds"))
					{
						LinkHash[3].Remove("mapbounds");
					}
					if (GUI.Button(new Rect(num4 + 30f, 159f, 64f, 30f), "Barrier"))
					{
						flag = true;
						flag2 = true;
						GameObject original12 = (GameObject)RCAssets.Load("barrierEditor");
						selectedObj = (GameObject)UnityEngine.Object.Instantiate(original12);
						selectedObj.name = "misc,barrier";
					}
					else if (GUI.Button(new Rect(num4 + 30f, 268f, 64f, 30f), "Region"))
					{
						if ((string)Settings[191] == string.Empty)
						{
							Settings[191] = "Region" + UnityEngine.Random.Range(10000, 99999);
						}
						flag = true;
						flag2 = true;
						GameObject original13 = (GameObject)RCAssets.Load("regionEditor");
						selectedObj = (GameObject)UnityEngine.Object.Instantiate(original13);
						GameObject gameObject4 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("UI/LabelNameOverHead"));
						gameObject4.name = "RegionLabel";
						if (!float.TryParse((string)Settings[71], out result2))
						{
							Settings[71] = "1";
						}
						if (!float.TryParse((string)Settings[70], out result2))
						{
							Settings[70] = "1";
						}
						if (!float.TryParse((string)Settings[72], out result2))
						{
							Settings[72] = "1";
						}
						gameObject4.transform.parent = selectedObj.transform;
						float y6 = 1f;
						if (Convert.ToSingle((string)Settings[71]) > 100f)
						{
							y6 = 0.8f;
						}
						else if (Convert.ToSingle((string)Settings[71]) > 1000f)
						{
							y6 = 0.5f;
						}
						gameObject4.transform.localPosition = new Vector3(0f, y6, 0f);
						gameObject4.transform.localScale = new Vector3(5f / Convert.ToSingle((string)Settings[70]), 5f / Convert.ToSingle((string)Settings[71]), 5f / Convert.ToSingle((string)Settings[72]));
						gameObject4.GetComponent<UILabel>().text = (string)Settings[191];
						selectedObj.AddComponent<RCRegionLabel>();
						selectedObj.GetComponent<RCRegionLabel>().myLabel = gameObject4;
						selectedObj.name = "misc,region," + (string)Settings[191];
					}
					Settings[191] = GUI.TextField(new Rect(num4 + 200f, 243f, 75f, 20f), (string)Settings[191]);
					bool flag9 = GUI.Toggle(new Rect(num4 + 240f, 179f, 40f, 20f), flag8, "On");
					if (flag9 != flag8)
					{
						if (flag9)
						{
							Settings[186] = 1;
						}
						else
						{
							Settings[186] = 0;
						}
					}
					break;
				}
				case 108:
				{
					string[] array6 = new string[12]
					{
						"Cuboid", "Plane", "Sphere", "Cylinder", "Capsule", "Pyramid", "Cone", "Prism", "Arc90", "Arc180",
						"Torus", "Tube"
					};
					string[] array7 = new string[12];
					for (int num26 = 0; num26 < array7.Length; num26++)
					{
						array7[num26] = "start" + array6[num26];
					}
					float num27 = 110f + 114f * (float)((array7.Length - 1) / 4);
					num27 *= 4f;
					num27 += 200f;
					scroll = GUI.BeginScrollView(new Rect(num4, 90f, 303f, 350f), scroll, new Rect(num4, 90f, 300f, num27));
					GUI.Label(new Rect(num4 + 90f, 90f, 200f, 22f), "Racing Start Barrier");
					int num28 = 125;
					for (int num29 = 0; num29 < array7.Length; num29++)
					{
						int num30 = num29 % 4;
						int num31 = num29 / 4;
						GUI.DrawTexture(new Rect(num4 + 7.8f + 71.8f * (float)num30, (float)num28 + 114f * (float)num31, 64f, 64f), LoadTextureRC("p" + array7[num29]));
						if (GUI.Button(new Rect(num4 + 7.8f + 71.8f * (float)num30, (float)num28 + 69f + 114f * (float)num31, 64f, 30f), array6[num29]))
						{
							flag = true;
							flag2 = true;
							GameObject original = (GameObject)RCAssets.Load(array7[num29]);
							selectedObj = (GameObject)UnityEngine.Object.Instantiate(original);
							selectedObj.name = "racing," + array7[num29];
						}
					}
					num28 += 114 * (array7.Length / 4) + 10;
					GUI.Label(new Rect(num4 + 93f, num28, 200f, 22f), "Racing End Trigger");
					num28 += 35;
					for (int num32 = 0; num32 < array7.Length; num32++)
					{
						array7[num32] = "end" + array6[num32];
					}
					for (int num33 = 0; num33 < array7.Length; num33++)
					{
						int num34 = num33 % 4;
						int num35 = num33 / 4;
						GUI.DrawTexture(new Rect(num4 + 7.8f + 71.8f * (float)num34, (float)num28 + 114f * (float)num35, 64f, 64f), LoadTextureRC("p" + array7[num33]));
						if (GUI.Button(new Rect(num4 + 7.8f + 71.8f * (float)num34, (float)num28 + 69f + 114f * (float)num35, 64f, 30f), array6[num33]))
						{
							flag = true;
							flag2 = true;
							GameObject original2 = (GameObject)RCAssets.Load(array7[num33]);
							selectedObj = (GameObject)UnityEngine.Object.Instantiate(original2);
							selectedObj.name = "racing," + array7[num33];
						}
					}
					num28 += 114 * (array7.Length / 4) + 10;
					GUI.Label(new Rect(num4 + 113f, num28, 200f, 22f), "Kill Trigger");
					num28 += 35;
					for (int num36 = 0; num36 < array7.Length; num36++)
					{
						array7[num36] = "kill" + array6[num36];
					}
					for (int num37 = 0; num37 < array7.Length; num37++)
					{
						int num38 = num37 % 4;
						int num39 = num37 / 4;
						GUI.DrawTexture(new Rect(num4 + 7.8f + 71.8f * (float)num38, (float)num28 + 114f * (float)num39, 64f, 64f), LoadTextureRC("p" + array7[num37]));
						if (GUI.Button(new Rect(num4 + 7.8f + 71.8f * (float)num38, (float)num28 + 69f + 114f * (float)num39, 64f, 30f), array6[num37]))
						{
							flag = true;
							flag2 = true;
							GameObject original3 = (GameObject)RCAssets.Load(array7[num37]);
							selectedObj = (GameObject)UnityEngine.Object.Instantiate(original3);
							selectedObj.name = "racing," + array7[num37];
						}
					}
					num28 += 114 * (array7.Length / 4) + 10;
					GUI.Label(new Rect(num4 + 95f, num28, 200f, 22f), "Checkpoint Trigger");
					num28 += 35;
					for (int num40 = 0; num40 < array7.Length; num40++)
					{
						array7[num40] = "checkpoint" + array6[num40];
					}
					for (int num41 = 0; num41 < array7.Length; num41++)
					{
						int num42 = num41 % 4;
						int num43 = num41 / 4;
						GUI.DrawTexture(new Rect(num4 + 7.8f + 71.8f * (float)num42, (float)num28 + 114f * (float)num43, 64f, 64f), LoadTextureRC("p" + array7[num41]));
						if (GUI.Button(new Rect(num4 + 7.8f + 71.8f * (float)num42, (float)num28 + 69f + 114f * (float)num43, 64f, 30f), array6[num41]))
						{
							flag = true;
							flag2 = true;
							GameObject original4 = (GameObject)RCAssets.Load(array7[num41]);
							selectedObj = (GameObject)UnityEngine.Object.Instantiate(original4);
							selectedObj.name = "racing," + array7[num41];
						}
					}
					GUI.EndScrollView();
					break;
				}
				}
				if (!flag || !(selectedObj != null))
				{
					return;
				}
				if (!float.TryParse((string)Settings[70], out result2))
				{
					Settings[70] = "1";
				}
				if (!float.TryParse((string)Settings[71], out result2))
				{
					Settings[71] = "1";
				}
				if (!float.TryParse((string)Settings[72], out result2))
				{
					Settings[72] = "1";
				}
				if (!float.TryParse((string)Settings[79], out result2))
				{
					Settings[79] = "1";
				}
				if (!float.TryParse((string)Settings[80], out result2))
				{
					Settings[80] = "1";
				}
				if (!flag2)
				{
					float a = 1f;
					if ((string)Settings[69] != "default")
					{
						if (((string)Settings[69]).StartsWith("transparent"))
						{
							if (float.TryParse(((string)Settings[69]).Substring(11), out var result3))
							{
								a = result3;
							}
							Renderer[] componentsInChildren = selectedObj.GetComponentsInChildren<Renderer>();
							foreach (Renderer renderer2 in componentsInChildren)
							{
								renderer2.material = (Material)RCAssets.Load("transparent");
								renderer2.material.mainTextureScale = new Vector2(renderer2.material.mainTextureScale.x * Convert.ToSingle((string)Settings[79]), renderer2.material.mainTextureScale.y * Convert.ToSingle((string)Settings[80]));
							}
						}
						else
						{
							Renderer[] componentsInChildren = selectedObj.GetComponentsInChildren<Renderer>();
							foreach (Renderer renderer3 in componentsInChildren)
							{
								if (!renderer3.name.Contains("Particle System") || !selectedObj.name.Contains("aot_supply"))
								{
									renderer3.material = (Material)RCAssets.Load((string)Settings[69]);
									renderer3.material.mainTextureScale = new Vector2(renderer3.material.mainTextureScale.x * Convert.ToSingle((string)Settings[79]), renderer3.material.mainTextureScale.y * Convert.ToSingle((string)Settings[80]));
								}
							}
						}
					}
					float num53 = 1f;
					MeshFilter[] componentsInChildren2 = selectedObj.GetComponentsInChildren<MeshFilter>();
					foreach (MeshFilter meshFilter in componentsInChildren2)
					{
						if (selectedObj.name.StartsWith("customb"))
						{
							if (num53 < meshFilter.mesh.bounds.size.y)
							{
								num53 = meshFilter.mesh.bounds.size.y;
							}
						}
						else if (num53 < meshFilter.mesh.bounds.size.z)
						{
							num53 = meshFilter.mesh.bounds.size.z;
						}
					}
					float num54 = selectedObj.transform.localScale.x * Convert.ToSingle((string)Settings[70]);
					num54 -= 0.001f;
					float y7 = selectedObj.transform.localScale.y * Convert.ToSingle((string)Settings[71]);
					float z5 = selectedObj.transform.localScale.z * Convert.ToSingle((string)Settings[72]);
					selectedObj.transform.localScale = new Vector3(num54, y7, z5);
					if ((int)Settings[76] == 1)
					{
						Color color2 = new Color((float)Settings[73], (float)Settings[74], (float)Settings[75], a);
						componentsInChildren2 = selectedObj.GetComponentsInChildren<MeshFilter>();
						for (int i = 0; i < componentsInChildren2.Length; i++)
						{
							Mesh mesh2 = componentsInChildren2[i].mesh;
							Color[] array11 = new Color[mesh2.vertexCount];
							for (int num55 = 0; num55 < mesh2.vertexCount; num55++)
							{
								array11[num55] = color2;
							}
							mesh2.colors = array11;
						}
					}
					float num56 = selectedObj.transform.localScale.z;
					if (selectedObj.name.Contains("boulder2") || selectedObj.name.Contains("boulder3") || selectedObj.name.Contains("field2"))
					{
						num56 *= 0.01f;
					}
					float num57 = 10f + num56 * num53 * 1.2f / 2f;
					selectedObj.transform.position = new Vector3(Camera.main.transform.position.x + Camera.main.transform.forward.x * num57, Camera.main.transform.position.y + Camera.main.transform.forward.y * 10f, Camera.main.transform.position.z + Camera.main.transform.forward.z * num57);
					selectedObj.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
					string text6 = selectedObj.name;
					selectedObj.name = text6 + "," + (string)Settings[69] + "," + (string)Settings[70] + "," + (string)Settings[71] + "," + (string)Settings[72] + "," + Settings[76].ToString() + "," + (float)Settings[73] + "," + (float)Settings[74] + "," + (float)Settings[75] + "," + (string)Settings[79] + "," + (string)Settings[80];
					UnloadAssetsEditor();
				}
				else if (selectedObj.name.StartsWith("misc"))
				{
					if (selectedObj.name.Contains("barrier") || selectedObj.name.Contains("region") || selectedObj.name.Contains("racing"))
					{
						float num58 = 1f;
						float num59 = selectedObj.transform.localScale.x * Convert.ToSingle((string)Settings[70]);
						num59 -= 0.001f;
						float y8 = selectedObj.transform.localScale.y * Convert.ToSingle((string)Settings[71]);
						float z6 = selectedObj.transform.localScale.z * Convert.ToSingle((string)Settings[72]);
						selectedObj.transform.localScale = new Vector3(num59, y8, z6);
						float z7 = selectedObj.transform.localScale.z;
						float num60 = 10f + z7 * num58 * 1.2f / 2f;
						selectedObj.transform.position = new Vector3(Camera.main.transform.position.x + Camera.main.transform.forward.x * num60, Camera.main.transform.position.y + Camera.main.transform.forward.y * 10f, Camera.main.transform.position.z + Camera.main.transform.forward.z * num60);
						if (!selectedObj.name.Contains("region"))
						{
							selectedObj.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
						}
						string text7 = selectedObj.name;
						selectedObj.name = text7 + "," + (string)Settings[70] + "," + (string)Settings[71] + "," + (string)Settings[72];
					}
				}
				else if (selectedObj.name.StartsWith("racing"))
				{
					float num61 = 1f;
					float num62 = selectedObj.transform.localScale.x * Convert.ToSingle((string)Settings[70]);
					num62 -= 0.001f;
					float y9 = selectedObj.transform.localScale.y * Convert.ToSingle((string)Settings[71]);
					float z8 = selectedObj.transform.localScale.z * Convert.ToSingle((string)Settings[72]);
					selectedObj.transform.localScale = new Vector3(num62, y9, z8);
					float z9 = selectedObj.transform.localScale.z;
					float num63 = 10f + z9 * num61 * 1.2f / 2f;
					selectedObj.transform.position = new Vector3(Camera.main.transform.position.x + Camera.main.transform.forward.x * num63, Camera.main.transform.position.y + Camera.main.transform.forward.y * 10f, Camera.main.transform.position.z + Camera.main.transform.forward.z * num63);
					selectedObj.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
					string text8 = selectedObj.name;
					selectedObj.name = text8 + "," + (string)Settings[70] + "," + (string)Settings[71] + "," + (string)Settings[72];
				}
				else
				{
					selectedObj.transform.position = new Vector3(Camera.main.transform.position.x + Camera.main.transform.forward.x * 10f, Camera.main.transform.position.y + Camera.main.transform.forward.y * 10f, Camera.main.transform.position.z + Camera.main.transform.forward.z * 10f);
					selectedObj.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
				}
				Screen.lockCursor = true;
				GUI.FocusControl(null);
			}
			else if (inputManager != null && inputManager.menuOn)
			{
				Screen.showCursor = true;
				Screen.lockCursor = false;
				if ((int)Settings[64] == 6)
				{
					return;
				}
				float num64 = (float)Screen.width / 2f - 350f;
				float num65 = (float)Screen.height / 2f - 250f;
				GUI.Box(new Rect(num64, num65, 700f, 500f), string.Empty);
				if (GUI.Button(new Rect(num64 + 7f, num65 + 7f, 59f, 25f), "General"))
				{
					Settings[64] = 0;
				}
				else if (GUI.Button(new Rect(num64 + 71f, num65 + 7f, 60f, 25f), "Rebinds"))
				{
					Settings[64] = 1;
				}
				else if (GUI.Button(new Rect(num64 + 136f, num65 + 7f, 85f, 25f), "Human Skins"))
				{
					Settings[64] = 2;
				}
				else if (GUI.Button(new Rect(num64 + 226f, num65 + 7f, 85f, 25f), "Titan Skins"))
				{
					Settings[64] = 3;
				}
				else if (GUI.Button(new Rect(num64 + 316f, num65 + 7f, 85f, 25f), "Level Skins"))
				{
					Settings[64] = 7;
				}
				else if (GUI.Button(new Rect(num64 + 406f, num65 + 7f, 85f, 25f), "Custom Map"))
				{
					Settings[64] = 8;
				}
				else if (GUI.Button(new Rect(num64 + 496f, num65 + 7f, 93f, 25f), "Custom Logic"))
				{
					Settings[64] = 9;
				}
				else if (GUI.Button(new Rect(num64 + 594f, num65 + 7f, 99f, 25f), "Game Settings"))
				{
					Settings[64] = 10;
				}
				else if (GUI.Button(new Rect(num64 + 7f, num65 + 37f, 70f, 25f), "Abilities"))
				{
					Settings[64] = 11;
				}
				switch ((int)Settings[64])
				{
				case 0:
				{
					GUI.Label(new Rect(num64 + 150f, num65 + 51f, 185f, 22f), "Graphics", "Label");
					GUI.Label(new Rect(num64 + 72f, num65 + 81f, 185f, 22f), "Disable custom gas textures:", "Label");
					GUI.Label(new Rect(num64 + 72f, num65 + 106f, 185f, 22f), "Disable weapon trail:", "Label");
					GUI.Label(new Rect(num64 + 72f, num65 + 131f, 185f, 22f), "Disable wind effect:", "Label");
					GUI.Label(new Rect(num64 + 72f, num65 + 156f, 185f, 22f), "Enable vSync:", "Label");
					GUI.Label(new Rect(num64 + 72f, num65 + 184f, 227f, 20f), "FPS Cap (0 for disabled):", "Label");
					GUI.Label(new Rect(num64 + 72f, num65 + 212f, 150f, 22f), "Texture Quality:", "Label");
					GUI.Label(new Rect(num64 + 72f, num65 + 242f, 150f, 22f), "Overall Quality:", "Label");
					GUI.Label(new Rect(num64 + 72f, num65 + 272f, 185f, 22f), "Disable Mipmapping:", "Label");
					GUI.Label(new Rect(num64 + 72f, num65 + 297f, 185f, 65f), "*Disabling mipmapping will increase custom texture quality at the cost of performance.", "Label");
					qualitySlider = GUI.HorizontalSlider(new Rect(num64 + 199f, num65 + 247f, 115f, 20f), qualitySlider, 0f, 1f);
					PlayerPrefs.SetFloat("GameQuality", qualitySlider);
					if (qualitySlider < 0.167f)
					{
						QualitySettings.SetQualityLevel(0, applyExpensiveChanges: true);
					}
					else if (qualitySlider < 0.33f)
					{
						QualitySettings.SetQualityLevel(1, applyExpensiveChanges: true);
					}
					else if (qualitySlider < 0.5f)
					{
						QualitySettings.SetQualityLevel(2, applyExpensiveChanges: true);
					}
					else if (qualitySlider < 0.67f)
					{
						QualitySettings.SetQualityLevel(3, applyExpensiveChanges: true);
					}
					else if (qualitySlider < 0.83f)
					{
						QualitySettings.SetQualityLevel(4, applyExpensiveChanges: true);
					}
					else if (qualitySlider <= 1f)
					{
						QualitySettings.SetQualityLevel(5, applyExpensiveChanges: true);
					}
					bool flag22 = false;
					bool flag23 = false;
					bool flag24 = false;
					bool flag25 = false;
					bool flag26 = false;
					if ((int)Settings[15] == 1)
					{
						flag22 = true;
					}
					if ((int)Settings[92] == 1)
					{
						flag23 = true;
					}
					if ((int)Settings[93] == 1)
					{
						flag24 = true;
					}
					if ((int)Settings[63] == 1)
					{
						flag25 = true;
					}
					if ((int)Settings[183] == 1)
					{
						flag26 = true;
					}
					bool flag27 = GUI.Toggle(new Rect(num64 + 274f, num65 + 81f, 40f, 20f), flag22, "On");
					if (flag27 != flag22)
					{
						if (flag27)
						{
							Settings[15] = 1;
						}
						else
						{
							Settings[15] = 0;
						}
					}
					bool flag28 = GUI.Toggle(new Rect(num64 + 274f, num65 + 106f, 40f, 20f), flag23, "On");
					if (flag28 != flag23)
					{
						if (flag28)
						{
							Settings[92] = 1;
						}
						else
						{
							Settings[92] = 0;
						}
					}
					bool flag29 = GUI.Toggle(new Rect(num64 + 274f, num65 + 131f, 40f, 20f), flag24, "On");
					if (flag29 != flag24)
					{
						if (flag29)
						{
							Settings[93] = 1;
						}
						else
						{
							Settings[93] = 0;
						}
					}
					bool flag30 = GUI.Toggle(new Rect(num64 + 274f, num65 + 156f, 40f, 20f), flag26, "On");
					if (flag30 != flag26)
					{
						if (flag30)
						{
							Settings[183] = 1;
							QualitySettings.vSyncCount = 1;
						}
						else
						{
							Settings[183] = 0;
							QualitySettings.vSyncCount = 0;
						}
						Minimap.WaitAndTryRecaptureInstance(0.5f);
					}
					bool flag31 = GUI.Toggle(new Rect(num64 + 274f, num65 + 272f, 40f, 20f), flag25, "On");
					if (flag31 != flag25)
					{
						if (flag31)
						{
							Settings[63] = 1;
						}
						else
						{
							Settings[63] = 0;
						}
						LinkHash[0].Clear();
						LinkHash[1].Clear();
						LinkHash[2].Clear();
					}
					if (GUI.Button(new Rect(num64 + 199f, num65 + 212f, 115f, 20f), MasterTextureType(QualitySettings.masterTextureLimit)))
					{
						if (QualitySettings.masterTextureLimit <= 0)
						{
							QualitySettings.masterTextureLimit = 8;
						}
						else
						{
							QualitySettings.masterTextureLimit--;
						}
						LinkHash[0].Clear();
						LinkHash[1].Clear();
						LinkHash[2].Clear();
					}
					Settings[184] = GUI.TextField(new Rect(num64 + 234f, num65 + 184f, 80f, 20f), (string)Settings[184]);
					Application.targetFrameRate = -1;
					if (int.TryParse((string)Settings[184], out var result4) && result4 > 0)
					{
						Application.targetFrameRate = result4;
					}
					GUI.Label(new Rect(num64 + 470f, num65 + 51f, 185f, 22f), "Snapshots", "Label");
					GUI.Label(new Rect(num64 + 386f, num65 + 81f, 185f, 22f), "Enable Snapshots:", "Label");
					GUI.Label(new Rect(num64 + 386f, num65 + 106f, 185f, 22f), "Show In Game:", "Label");
					GUI.Label(new Rect(num64 + 386f, num65 + 131f, 227f, 22f), "Snapshot Minimum Damage:", "Label");
					Settings[95] = GUI.TextField(new Rect(num64 + 563f, num65 + 131f, 65f, 20f), (string)Settings[95]);
					bool flag32 = false;
					bool flag33 = false;
					if (PlayerPrefs.GetInt("EnableSS", 0) == 1)
					{
						flag32 = true;
					}
					if (PlayerPrefs.GetInt("showSSInGame", 0) == 1)
					{
						flag33 = true;
					}
					bool flag34 = GUI.Toggle(new Rect(num64 + 588f, num65 + 81f, 40f, 20f), flag32, "On");
					if (flag34 != flag32)
					{
						if (flag34)
						{
							PlayerPrefs.SetInt("EnableSS", 1);
						}
						else
						{
							PlayerPrefs.SetInt("EnableSS", 0);
						}
					}
					bool flag35 = GUI.Toggle(new Rect(num64 + 588f, num65 + 106f, 40f, 20f), flag33, "On");
					if (flag33 != flag35)
					{
						if (flag35)
						{
							PlayerPrefs.SetInt("showSSInGame", 1);
						}
						else
						{
							PlayerPrefs.SetInt("showSSInGame", 0);
						}
					}
					GUI.Label(new Rect(num64 + 485f, num65 + 161f, 185f, 22f), "Other", "Label");
					GUI.Label(new Rect(num64 + 386f, num65 + 186f, 80f, 20f), "Volume:", "Label");
					GUI.Label(new Rect(num64 + 386f, num65 + 211f, 95f, 20f), "Mouse Speed:", "Label");
					GUI.Label(new Rect(num64 + 386f, num65 + 236f, 95f, 20f), "Camera Dist:", "Label");
					GUI.Label(new Rect(num64 + 386f, num65 + 261f, 80f, 20f), "Camera Tilt:", "Label");
					GUI.Label(new Rect(num64 + 386f, num65 + 283f, 80f, 20f), "Invert Mouse:", "Label");
					GUI.Label(new Rect(num64 + 386f, num65 + 305f, 80f, 20f), "Speedometer:", "Label");
					GUI.Label(new Rect(num64 + 386f, num65 + 375f, 80f, 20f), "Minimap:", "Label");
					GUI.Label(new Rect(num64 + 386f, num65 + 397f, 100f, 20f), "Game Feed:", "Label");
					string[] texts2 = new string[3] { "Off", "Speed", "Damage" };
					Settings[189] = GUI.SelectionGrid(new Rect(num64 + 480f, num65 + 305f, 140f, 60f), (int)Settings[189], texts2, 1, GUI.skin.toggle);
					AudioListener.volume = GUI.HorizontalSlider(new Rect(num64 + 478f, num65 + 191f, 150f, 20f), AudioListener.volume, 0f, 1f);
					mouseSlider = GUI.HorizontalSlider(new Rect(num64 + 478f, num65 + 216f, 150f, 20f), mouseSlider, 0.1f, 1f);
					PlayerPrefs.SetFloat("MouseSensitivity", mouseSlider);
					IN_GAME_MAIN_CAMERA.SensitivityMulti = PlayerPrefs.GetFloat("MouseSensitivity");
					distanceSlider = GUI.HorizontalSlider(new Rect(num64 + 478f, num65 + 241f, 150f, 20f), distanceSlider, 0f, 1f);
					PlayerPrefs.SetFloat("cameraDistance", distanceSlider);
					IN_GAME_MAIN_CAMERA.CameraDistance = 0.3f + distanceSlider;
					bool flag36 = false;
					bool flag37 = false;
					bool flag38 = false;
					bool flag39 = false;
					if ((int)Settings[231] == 1)
					{
						flag38 = true;
					}
					if ((int)Settings[244] == 1)
					{
						flag39 = true;
					}
					if (PlayerPrefs.HasKey("cameraTilt"))
					{
						if (PlayerPrefs.GetInt("cameraTilt") == 1)
						{
							flag36 = true;
						}
					}
					else
					{
						PlayerPrefs.SetInt("cameraTilt", 1);
					}
					if (PlayerPrefs.HasKey("invertMouseY"))
					{
						if (PlayerPrefs.GetInt("invertMouseY") == -1)
						{
							flag37 = true;
						}
					}
					else
					{
						PlayerPrefs.SetInt("invertMouseY", 1);
					}
					bool flag40 = GUI.Toggle(new Rect(num64 + 480f, num65 + 261f, 40f, 20f), flag36, "On");
					if (flag36 != flag40)
					{
						if (flag40)
						{
							PlayerPrefs.SetInt("cameraTilt", 1);
						}
						else
						{
							PlayerPrefs.SetInt("cameraTilt", 0);
						}
					}
					bool flag41 = GUI.Toggle(new Rect(num64 + 480f, num65 + 283f, 40f, 20f), flag37, "On");
					if (flag41 != flag37)
					{
						if (flag41)
						{
							PlayerPrefs.SetInt("invertMouseY", -1);
						}
						else
						{
							PlayerPrefs.SetInt("invertMouseY", 1);
						}
					}
					bool flag42 = GUI.Toggle(new Rect(num64 + 480f, num65 + 375f, 40f, 20f), flag38, "On");
					if (flag38 != flag42)
					{
						if (flag42)
						{
							Settings[231] = 1;
						}
						else
						{
							Settings[231] = 0;
						}
					}
					bool flag43 = GUI.Toggle(new Rect(num64 + 480f, num65 + 397f, 40f, 20f), flag39, "On");
					if (flag39 != flag43)
					{
						if (flag43)
						{
							Settings[244] = 1;
						}
						else
						{
							Settings[244] = 0;
						}
					}
					IN_GAME_MAIN_CAMERA.CameraTilt = PlayerPrefs.GetInt("cameraTilt");
					IN_GAME_MAIN_CAMERA.InvertY = PlayerPrefs.GetInt("invertMouseY");
					break;
				}
				case 1:
					if (GUI.Button(new Rect(num64 + 233f, num65 + 51f, 55f, 25f), "Human"))
					{
						Settings[190] = 0;
					}
					else if (GUI.Button(new Rect(num64 + 293f, num65 + 51f, 52f, 25f), "Titan"))
					{
						Settings[190] = 1;
					}
					else if (GUI.Button(new Rect(num64 + 350f, num65 + 51f, 53f, 25f), "Horse"))
					{
						Settings[190] = 2;
					}
					else if (GUI.Button(new Rect(num64 + 408f, num65 + 51f, 59f, 25f), "Cannon"))
					{
						Settings[190] = 3;
					}
					if ((int)Settings[190] == 0)
					{
						List<string> list4 = new List<string>();
						list4.Add("Forward:");
						list4.Add("Backward:");
						list4.Add("Left:");
						list4.Add("Right:");
						list4.Add("Jump:");
						list4.Add("Dodge:");
						list4.Add("Left Hook:");
						list4.Add("Right Hook:");
						list4.Add("Both Hooks:");
						list4.Add("Lock:");
						list4.Add("Attack:");
						list4.Add("Special:");
						list4.Add("Salute:");
						list4.Add("Change Camera:");
						list4.Add("Reset:");
						list4.Add("Pause:");
						list4.Add("Show/Hide Cursor:");
						list4.Add("Fullscreen:");
						list4.Add("Change Blade:");
						list4.Add("Flare Green:");
						list4.Add("Flare Red:");
						list4.Add("Flare Black:");
						list4.Add("Reel in:");
						list4.Add("Reel out:");
						list4.Add("Gas Burst:");
						list4.Add("Minimap Max:");
						list4.Add("Minimap Toggle:");
						list4.Add("Minimap Reset:");
						list4.Add("Open Chat:");
						list4.Add("Live Spectate");
						for (int num68 = 0; num68 < list4.Count; num68++)
						{
							int num69 = num68;
							float num70 = 80f;
							if (num69 > 14)
							{
								num70 = 390f;
								num69 -= 15;
							}
							GUI.Label(new Rect(num64 + num70, num65 + 86f + (float)num69 * 25f, 145f, 22f), list4[num68], "Label");
						}
						bool flag12 = false;
						if ((int)Settings[97] == 1)
						{
							flag12 = true;
						}
						bool flag13 = false;
						if ((int)Settings[116] == 1)
						{
							flag13 = true;
						}
						bool flag14 = false;
						if ((int)Settings[181] == 1)
						{
							flag14 = true;
						}
						bool flag15 = GUI.Toggle(new Rect(num64 + 457f, num65 + 261f, 40f, 20f), flag12, "On");
						if (flag12 != flag15)
						{
							if (flag15)
							{
								Settings[97] = 1;
							}
							else
							{
								Settings[97] = 0;
							}
						}
						bool flag16 = GUI.Toggle(new Rect(num64 + 457f, num65 + 286f, 40f, 20f), flag13, "On");
						if (flag13 != flag16)
						{
							if (flag16)
							{
								Settings[116] = 1;
							}
							else
							{
								Settings[116] = 0;
							}
						}
						bool flag17 = GUI.Toggle(new Rect(num64 + 457f, num65 + 311f, 40f, 20f), flag14, "On");
						if (flag14 != flag17)
						{
							if (flag17)
							{
								Settings[181] = 1;
							}
							else
							{
								Settings[181] = 0;
							}
						}
						for (int num71 = 0; num71 < 22; num71++)
						{
							int num72 = num71;
							float num73 = 190f;
							if (num72 > 14)
							{
								num73 = 500f;
								num72 -= 15;
							}
							if (GUI.Button(new Rect(num64 + num73, num65 + 86f + (float)num72 * 25f, 120f, 20f), inputManager.getKeyRC(num71)))
							{
								Settings[100] = num71 + 1;
								inputManager.setNameRC(num71, "waiting...");
							}
						}
						if (GUI.Button(new Rect(num64 + 500f, num65 + 261f, 120f, 20f), (string)Settings[98]))
						{
							Settings[98] = "waiting...";
							Settings[100] = 98;
						}
						else if (GUI.Button(new Rect(num64 + 500f, num65 + 286f, 120f, 20f), (string)Settings[99]))
						{
							Settings[99] = "waiting...";
							Settings[100] = 99;
						}
						else if (GUI.Button(new Rect(num64 + 500f, num65 + 311f, 120f, 20f), (string)Settings[182]))
						{
							Settings[182] = "waiting...";
							Settings[100] = 182;
						}
						else if (GUI.Button(new Rect(num64 + 500f, num65 + 336f, 120f, 20f), (string)Settings[232]))
						{
							Settings[232] = "waiting...";
							Settings[100] = 232;
						}
						else if (GUI.Button(new Rect(num64 + 500f, num65 + 361f, 120f, 20f), (string)Settings[233]))
						{
							Settings[233] = "waiting...";
							Settings[100] = 233;
						}
						else if (GUI.Button(new Rect(num64 + 500f, num65 + 386f, 120f, 20f), (string)Settings[234]))
						{
							Settings[234] = "waiting...";
							Settings[100] = 234;
						}
						else if (GUI.Button(new Rect(num64 + 500f, num65 + 411f, 120f, 20f), (string)Settings[236]))
						{
							Settings[236] = "waiting...";
							Settings[100] = 236;
						}
						else if (GUI.Button(new Rect(num64 + 500f, num65 + 436f, 120f, 20f), (string)Settings[262]))
						{
							Settings[262] = "waiting...";
							Settings[100] = 262;
						}
						if ((int)Settings[100] == 0)
						{
							break;
						}
						Event current2 = Event.current;
						bool flag18 = false;
						string text9 = "waiting...";
						if (current2.type == EventType.KeyDown && current2.keyCode != 0)
						{
							flag18 = true;
							text9 = current2.keyCode.ToString();
						}
						else if (Input.GetKey(KeyCode.LeftShift))
						{
							flag18 = true;
							text9 = KeyCode.LeftShift.ToString();
						}
						else if (Input.GetKey(KeyCode.RightShift))
						{
							flag18 = true;
							text9 = KeyCode.RightShift.ToString();
						}
						else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
						{
							if (Input.GetAxis("Mouse ScrollWheel") > 0f)
							{
								flag18 = true;
								text9 = "Scroll Up";
							}
							else
							{
								flag18 = true;
								text9 = "Scroll Down";
							}
						}
						else
						{
							for (int num74 = 0; num74 < 7; num74++)
							{
								if (Input.GetKeyDown((KeyCode)(323 + num74)))
								{
									flag18 = true;
									text9 = "Mouse" + Convert.ToString(num74);
								}
							}
						}
						if (!flag18)
						{
							break;
						}
						if ((int)Settings[100] == 98)
						{
							Settings[98] = text9;
							Settings[100] = 0;
							InputRC.setInputHuman(InputCodeRC.ReelIn, text9);
							break;
						}
						if ((int)Settings[100] == 99)
						{
							Settings[99] = text9;
							Settings[100] = 0;
							InputRC.setInputHuman(InputCodeRC.ReelOut, text9);
							break;
						}
						if ((int)Settings[100] == 182)
						{
							Settings[182] = text9;
							Settings[100] = 0;
							InputRC.setInputHuman(InputCodeRC.Dash, text9);
							break;
						}
						if ((int)Settings[100] == 232)
						{
							Settings[232] = text9;
							Settings[100] = 0;
							InputRC.setInputHuman(InputCodeRC.MapMaximize, text9);
							break;
						}
						if ((int)Settings[100] == 233)
						{
							Settings[233] = text9;
							Settings[100] = 0;
							InputRC.setInputHuman(InputCodeRC.MapToggle, text9);
							break;
						}
						if ((int)Settings[100] == 234)
						{
							Settings[234] = text9;
							Settings[100] = 0;
							InputRC.setInputHuman(InputCodeRC.MapReset, text9);
							break;
						}
						if ((int)Settings[100] == 236)
						{
							Settings[236] = text9;
							Settings[100] = 0;
							InputRC.setInputHuman(InputCodeRC.Chat, text9);
							break;
						}
						if ((int)Settings[100] == 262)
						{
							Settings[262] = text9;
							Settings[100] = 0;
							InputRC.setInputHuman(InputCodeRC.LiveCamera, text9);
							break;
						}
						for (int num75 = 0; num75 < 22; num75++)
						{
							int num76 = num75 + 1;
							if ((int)Settings[100] == num76)
							{
								inputManager.setKeyRC(num75, text9);
								Settings[100] = 0;
							}
						}
					}
					else if ((int)Settings[190] == 1)
					{
						List<string> list5 = new List<string>
						{
							"Forward:", "Back:", "Left:", "Right:", "Walk:", "Jump:", "Punch:", "Slam:", "Grab (front):", "Grab (back):",
							"Grab (nape):", "Slap:", "Bite:", "Cover Nape:"
						};
						for (int num77 = 0; num77 < list5.Count; num77++)
						{
							int num78 = num77;
							float num79 = 80f;
							if (num78 > 6)
							{
								num79 = 390f;
								num78 -= 7;
							}
							GUI.Label(new Rect(num64 + num79, num65 + 86f + (float)num78 * 25f, 145f, 22f), list5[num77], "Label");
						}
						for (int num80 = 0; num80 < 14; num80++)
						{
							int num81 = 101 + num80;
							int num82 = num80;
							float num83 = 190f;
							if (num82 > 6)
							{
								num83 = 500f;
								num82 -= 7;
							}
							if (GUI.Button(new Rect(num64 + num83, num65 + 86f + (float)num82 * 25f, 120f, 20f), (string)Settings[num81]))
							{
								Settings[num81] = "waiting...";
								Settings[100] = num81;
							}
						}
						if ((int)Settings[100] == 0)
						{
							break;
						}
						Event current3 = Event.current;
						bool flag19 = false;
						string text10 = "waiting...";
						if (current3.type == EventType.KeyDown && current3.keyCode != 0)
						{
							flag19 = true;
							text10 = current3.keyCode.ToString();
						}
						else if (Input.GetKey(KeyCode.LeftShift))
						{
							flag19 = true;
							text10 = KeyCode.LeftShift.ToString();
						}
						else if (Input.GetKey(KeyCode.RightShift))
						{
							flag19 = true;
							text10 = KeyCode.RightShift.ToString();
						}
						else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
						{
							if (Input.GetAxis("Mouse ScrollWheel") > 0f)
							{
								flag19 = true;
								text10 = "Scroll Up";
							}
							else
							{
								flag19 = true;
								text10 = "Scroll Down";
							}
						}
						else
						{
							for (int num84 = 0; num84 < 7; num84++)
							{
								if (Input.GetKeyDown((KeyCode)(323 + num84)))
								{
									flag19 = true;
									text10 = "Mouse" + Convert.ToString(num84);
								}
							}
						}
						if (!flag19)
						{
							break;
						}
						for (int num85 = 0; num85 < 14; num85++)
						{
							int num86 = 101 + num85;
							if ((int)Settings[100] == num86)
							{
								Settings[num86] = text10;
								Settings[100] = 0;
								InputRC.setInputTitan(num85, text10);
							}
						}
					}
					else if ((int)Settings[190] == 2)
					{
						List<string> list6 = new List<string> { "Forward:", "Back:", "Left:", "Right:", "Walk:", "Jump:", "Mount:" };
						for (int num87 = 0; num87 < list6.Count; num87++)
						{
							int num88 = num87;
							float num89 = 80f;
							if (num88 > 3)
							{
								num89 = 390f;
								num88 -= 4;
							}
							GUI.Label(new Rect(num64 + num89, num65 + 86f + (float)num88 * 25f, 145f, 22f), list6[num87], "Label");
						}
						for (int num90 = 0; num90 < 7; num90++)
						{
							int num91 = 237 + num90;
							int num92 = num90;
							float num93 = 190f;
							if (num92 > 3)
							{
								num93 = 500f;
								num92 -= 4;
							}
							if (GUI.Button(new Rect(num64 + num93, num65 + 86f + (float)num92 * 25f, 120f, 20f), (string)Settings[num91]))
							{
								Settings[num91] = "waiting...";
								Settings[100] = num91;
							}
						}
						if ((int)Settings[100] == 0)
						{
							break;
						}
						Event current4 = Event.current;
						bool flag20 = false;
						string text11 = "waiting...";
						if (current4.type == EventType.KeyDown && current4.keyCode != 0)
						{
							flag20 = true;
							text11 = current4.keyCode.ToString();
						}
						else if (Input.GetKey(KeyCode.LeftShift))
						{
							flag20 = true;
							text11 = KeyCode.LeftShift.ToString();
						}
						else if (Input.GetKey(KeyCode.RightShift))
						{
							flag20 = true;
							text11 = KeyCode.RightShift.ToString();
						}
						else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
						{
							if (Input.GetAxis("Mouse ScrollWheel") > 0f)
							{
								flag20 = true;
								text11 = "Scroll Up";
							}
							else
							{
								flag20 = true;
								text11 = "Scroll Down";
							}
						}
						else
						{
							for (int num94 = 0; num94 < 7; num94++)
							{
								if (Input.GetKeyDown((KeyCode)(323 + num94)))
								{
									flag20 = true;
									text11 = "Mouse" + Convert.ToString(num94);
								}
							}
						}
						if (!flag20)
						{
							break;
						}
						for (int num95 = 0; num95 < 7; num95++)
						{
							int num96 = 237 + num95;
							if ((int)Settings[100] == num96)
							{
								Settings[num96] = text11;
								Settings[100] = 0;
								InputRC.setInputHorse(num95, text11);
							}
						}
					}
					else
					{
						if ((int)Settings[190] != 3)
						{
							break;
						}
						List<string> list7 = new List<string> { "Rotate Up:", "Rotate Down:", "Rotate Left:", "Rotate Right:", "Fire:", "Mount:", "Slow Rotate:" };
						for (int num97 = 0; num97 < list7.Count; num97++)
						{
							int num98 = num97;
							float num99 = 80f;
							if (num98 > 3)
							{
								num99 = 390f;
								num98 -= 4;
							}
							GUI.Label(new Rect(num64 + num99, num65 + 86f + (float)num98 * 25f, 145f, 22f), list7[num97], "Label");
						}
						for (int num100 = 0; num100 < 7; num100++)
						{
							int num101 = 254 + num100;
							int num102 = num100;
							float num103 = 190f;
							if (num102 > 3)
							{
								num103 = 500f;
								num102 -= 4;
							}
							if (GUI.Button(new Rect(num64 + num103, num65 + 86f + (float)num102 * 25f, 120f, 20f), (string)Settings[num101]))
							{
								Settings[num101] = "waiting...";
								Settings[100] = num101;
							}
						}
						if ((int)Settings[100] == 0)
						{
							break;
						}
						Event current5 = Event.current;
						bool flag21 = false;
						string text12 = "waiting...";
						if (current5.type == EventType.KeyDown && current5.keyCode != 0)
						{
							flag21 = true;
							text12 = current5.keyCode.ToString();
						}
						else if (Input.GetKey(KeyCode.LeftShift))
						{
							flag21 = true;
							text12 = KeyCode.LeftShift.ToString();
						}
						else if (Input.GetKey(KeyCode.RightShift))
						{
							flag21 = true;
							text12 = KeyCode.RightShift.ToString();
						}
						else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
						{
							if (Input.GetAxis("Mouse ScrollWheel") > 0f)
							{
								flag21 = true;
								text12 = "Scroll Up";
							}
							else
							{
								flag21 = true;
								text12 = "Scroll Down";
							}
						}
						else
						{
							for (int num104 = 0; num104 < 6; num104++)
							{
								if (Input.GetKeyDown((KeyCode)(323 + num104)))
								{
									flag21 = true;
									text12 = "Mouse" + Convert.ToString(num104);
								}
							}
						}
						if (!flag21)
						{
							break;
						}
						for (int num105 = 0; num105 < 7; num105++)
						{
							int num106 = 254 + num105;
							if ((int)Settings[100] == num106)
							{
								Settings[num106] = text12;
								Settings[100] = 0;
								InputRC.setInputCannon(num105, text12);
							}
						}
					}
					break;
				case 2:
				{
					GUI.Label(new Rect(num64 + 205f, num65 + 52f, 120f, 30f), "Human Skin Mode:", "Label");
					bool flag10 = false;
					if ((int)Settings[0] == 1)
					{
						flag10 = true;
					}
					bool flag11 = GUI.Toggle(new Rect(num64 + 325f, num65 + 52f, 40f, 20f), flag10, "On");
					if (flag10 != flag11)
					{
						if (flag11)
						{
							Settings[0] = 1;
						}
						else
						{
							Settings[0] = 0;
						}
					}
					float num67 = 44f;
					if ((int)Settings[133] == 0)
					{
						if (GUI.Button(new Rect(num64 + 375f, num65 + 51f, 120f, 22f), "Human Set 1"))
						{
							Settings[133] = 1;
						}
						Settings[3] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 0f, 230f, 20f), (string)Settings[3]);
						Settings[4] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 1f, 230f, 20f), (string)Settings[4]);
						Settings[5] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 2f, 230f, 20f), (string)Settings[5]);
						Settings[6] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 3f, 230f, 20f), (string)Settings[6]);
						Settings[7] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 4f, 230f, 20f), (string)Settings[7]);
						Settings[8] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 5f, 230f, 20f), (string)Settings[8]);
						Settings[14] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 6f, 230f, 20f), (string)Settings[14]);
						Settings[9] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 0f, 230f, 20f), (string)Settings[9]);
						Settings[10] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 1f, 230f, 20f), (string)Settings[10]);
						Settings[11] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 2f, 230f, 20f), (string)Settings[11]);
						Settings[12] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 3f, 230f, 20f), (string)Settings[12]);
						Settings[13] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 4f, 230f, 20f), (string)Settings[13]);
						Settings[94] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 5f, 230f, 20f), (string)Settings[94]);
					}
					else if ((int)Settings[133] == 1)
					{
						if (GUI.Button(new Rect(num64 + 375f, num65 + 51f, 120f, 22f), "Human Set 2"))
						{
							Settings[133] = 2;
						}
						Settings[134] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 0f, 230f, 20f), (string)Settings[134]);
						Settings[135] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 1f, 230f, 20f), (string)Settings[135]);
						Settings[136] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 2f, 230f, 20f), (string)Settings[136]);
						Settings[137] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 3f, 230f, 20f), (string)Settings[137]);
						Settings[138] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 4f, 230f, 20f), (string)Settings[138]);
						Settings[139] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 5f, 230f, 20f), (string)Settings[139]);
						Settings[145] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 6f, 230f, 20f), (string)Settings[145]);
						Settings[140] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 0f, 230f, 20f), (string)Settings[140]);
						Settings[141] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 1f, 230f, 20f), (string)Settings[141]);
						Settings[142] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 2f, 230f, 20f), (string)Settings[142]);
						Settings[143] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 3f, 230f, 20f), (string)Settings[143]);
						Settings[144] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 4f, 230f, 20f), (string)Settings[144]);
						Settings[146] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 5f, 230f, 20f), (string)Settings[146]);
					}
					else if ((int)Settings[133] == 2)
					{
						if (GUI.Button(new Rect(num64 + 375f, num65 + 51f, 120f, 22f), "Human Set 3"))
						{
							Settings[133] = 0;
						}
						Settings[147] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 0f, 230f, 20f), (string)Settings[147]);
						Settings[148] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 1f, 230f, 20f), (string)Settings[148]);
						Settings[149] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 2f, 230f, 20f), (string)Settings[149]);
						Settings[150] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 3f, 230f, 20f), (string)Settings[150]);
						Settings[151] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 4f, 230f, 20f), (string)Settings[151]);
						Settings[152] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 5f, 230f, 20f), (string)Settings[152]);
						Settings[158] = GUI.TextField(new Rect(num64 + 80f, num65 + 114f + num67 * 6f, 230f, 20f), (string)Settings[158]);
						Settings[153] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 0f, 230f, 20f), (string)Settings[153]);
						Settings[154] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 1f, 230f, 20f), (string)Settings[154]);
						Settings[155] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 2f, 230f, 20f), (string)Settings[155]);
						Settings[156] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 3f, 230f, 20f), (string)Settings[156]);
						Settings[157] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 4f, 230f, 20f), (string)Settings[157]);
						Settings[159] = GUI.TextField(new Rect(num64 + 390f, num65 + 114f + num67 * 5f, 230f, 20f), (string)Settings[159]);
					}
					GUI.Label(new Rect(num64 + 80f, num65 + 92f + num67 * 0f, 150f, 20f), "Horse:", "Label");
					GUI.Label(new Rect(num64 + 80f, num65 + 92f + num67 * 1f, 227f, 20f), "Hair (model dependent):", "Label");
					GUI.Label(new Rect(num64 + 80f, num65 + 92f + num67 * 2f, 150f, 20f), "Eyes:", "Label");
					GUI.Label(new Rect(num64 + 80f, num65 + 92f + num67 * 3f, 240f, 20f), "Glass (must have a glass enabled):", "Label");
					GUI.Label(new Rect(num64 + 80f, num65 + 92f + num67 * 4f, 150f, 20f), "Face:", "Label");
					GUI.Label(new Rect(num64 + 80f, num65 + 92f + num67 * 5f, 150f, 20f), "Skin:", "Label");
					GUI.Label(new Rect(num64 + 80f, num65 + 92f + num67 * 6f, 240f, 20f), "Hoodie (costume dependent):", "Label");
					GUI.Label(new Rect(num64 + 390f, num65 + 92f + num67 * 0f, 240f, 20f), "Costume (model dependent):", "Label");
					GUI.Label(new Rect(num64 + 390f, num65 + 92f + num67 * 1f, 150f, 20f), "Logo & Cape:", "Label");
					GUI.Label(new Rect(num64 + 390f, num65 + 92f + num67 * 2f, 240f, 20f), "3DMG Center & 3DMG/Blade/Gun(left):", "Label");
					GUI.Label(new Rect(num64 + 390f, num65 + 92f + num67 * 3f, 227f, 20f), "3DMG/Blade/Gun(right):", "Label");
					GUI.Label(new Rect(num64 + 390f, num65 + 92f + num67 * 4f, 150f, 20f), "Gas:", "Label");
					GUI.Label(new Rect(num64 + 390f, num65 + 92f + num67 * 5f, 150f, 20f), "Weapon Trail:", "Label");
					break;
				}
				case 3:
				{
					GUI.Label(new Rect(num64 + 270f, num65 + 52f, 120f, 30f), "Titan Skin Mode:", "Label");
					bool flag52 = false;
					if ((int)Settings[1] == 1)
					{
						flag52 = true;
					}
					bool flag53 = GUI.Toggle(new Rect(num64 + 390f, num65 + 52f, 40f, 20f), flag52, "On");
					if (flag52 != flag53)
					{
						if (flag53)
						{
							Settings[1] = 1;
						}
						else
						{
							Settings[1] = 0;
						}
					}
					GUI.Label(new Rect(num64 + 270f, num65 + 77f, 120f, 30f), "Randomized Pairs:", "Label");
					flag52 = false;
					if ((int)Settings[32] == 1)
					{
						flag52 = true;
					}
					flag53 = GUI.Toggle(new Rect(num64 + 390f, num65 + 77f, 40f, 20f), flag52, "On");
					if (flag52 != flag53)
					{
						if (flag53)
						{
							Settings[32] = 1;
						}
						else
						{
							Settings[32] = 0;
						}
					}
					GUI.Label(new Rect(num64 + 158f, num65 + 112f, 150f, 20f), "Titan Hair:", "Label");
					Settings[21] = GUI.TextField(new Rect(num64 + 80f, num65 + 134f, 165f, 20f), (string)Settings[21]);
					Settings[22] = GUI.TextField(new Rect(num64 + 80f, num65 + 156f, 165f, 20f), (string)Settings[22]);
					Settings[23] = GUI.TextField(new Rect(num64 + 80f, num65 + 178f, 165f, 20f), (string)Settings[23]);
					Settings[24] = GUI.TextField(new Rect(num64 + 80f, num65 + 200f, 165f, 20f), (string)Settings[24]);
					Settings[25] = GUI.TextField(new Rect(num64 + 80f, num65 + 222f, 165f, 20f), (string)Settings[25]);
					if (GUI.Button(new Rect(num64 + 250f, num65 + 134f, 60f, 20f), HairType((int)Settings[16])))
					{
						int num108 = 16;
						int num109 = (int)Settings[num108];
						num109 = ((num109 < 9) ? (num109 + 1) : (-1));
						Settings[num108] = num109;
					}
					else if (GUI.Button(new Rect(num64 + 250f, num65 + 156f, 60f, 20f), HairType((int)Settings[17])))
					{
						int num110 = 17;
						int num111 = (int)Settings[num110];
						num111 = ((num111 < 9) ? (num111 + 1) : (-1));
						Settings[num110] = num111;
					}
					else if (GUI.Button(new Rect(num64 + 250f, num65 + 178f, 60f, 20f), HairType((int)Settings[18])))
					{
						int num112 = 18;
						int num113 = (int)Settings[num112];
						num113 = ((num113 < 9) ? (num113 + 1) : (-1));
						Settings[num112] = num113;
					}
					else if (GUI.Button(new Rect(num64 + 250f, num65 + 200f, 60f, 20f), HairType((int)Settings[19])))
					{
						int num114 = 19;
						int num115 = (int)Settings[num114];
						num115 = ((num115 < 9) ? (num115 + 1) : (-1));
						Settings[num114] = num115;
					}
					else if (GUI.Button(new Rect(num64 + 250f, num65 + 222f, 60f, 20f), HairType((int)Settings[20])))
					{
						int num116 = 20;
						int num117 = (int)Settings[num116];
						num117 = ((num117 < 9) ? (num117 + 1) : (-1));
						Settings[num116] = num117;
					}
					GUI.Label(new Rect(num64 + 158f, num65 + 252f, 150f, 20f), "Titan Eye:", "Label");
					Settings[26] = GUI.TextField(new Rect(num64 + 80f, num65 + 274f, 230f, 20f), (string)Settings[26]);
					Settings[27] = GUI.TextField(new Rect(num64 + 80f, num65 + 296f, 230f, 20f), (string)Settings[27]);
					Settings[28] = GUI.TextField(new Rect(num64 + 80f, num65 + 318f, 230f, 20f), (string)Settings[28]);
					Settings[29] = GUI.TextField(new Rect(num64 + 80f, num65 + 340f, 230f, 20f), (string)Settings[29]);
					Settings[30] = GUI.TextField(new Rect(num64 + 80f, num65 + 362f, 230f, 20f), (string)Settings[30]);
					GUI.Label(new Rect(num64 + 455f, num65 + 112f, 150f, 20f), "Titan Body:", "Label");
					Settings[86] = GUI.TextField(new Rect(num64 + 390f, num65 + 134f, 230f, 20f), (string)Settings[86]);
					Settings[87] = GUI.TextField(new Rect(num64 + 390f, num65 + 156f, 230f, 20f), (string)Settings[87]);
					Settings[88] = GUI.TextField(new Rect(num64 + 390f, num65 + 178f, 230f, 20f), (string)Settings[88]);
					Settings[89] = GUI.TextField(new Rect(num64 + 390f, num65 + 200f, 230f, 20f), (string)Settings[89]);
					Settings[90] = GUI.TextField(new Rect(num64 + 390f, num65 + 222f, 230f, 20f), (string)Settings[90]);
					GUI.Label(new Rect(num64 + 472f, num65 + 252f, 150f, 20f), "Eren:", "Label");
					Settings[65] = GUI.TextField(new Rect(num64 + 390f, num65 + 274f, 230f, 20f), (string)Settings[65]);
					GUI.Label(new Rect(num64 + 470f, num65 + 296f, 150f, 20f), "Annie:", "Label");
					Settings[66] = GUI.TextField(new Rect(num64 + 390f, num65 + 318f, 230f, 20f), (string)Settings[66]);
					GUI.Label(new Rect(num64 + 465f, num65 + 340f, 150f, 20f), "Colossal:", "Label");
					Settings[67] = GUI.TextField(new Rect(num64 + 390f, num65 + 362f, 230f, 20f), (string)Settings[67]);
					break;
				}
				case 4:
					GUI.TextArea(new Rect(num64 + 80f, num65 + 57f, 270f, 30f), "Saved settings to PlayerPrefs!", 100, "Label");
					break;
				case 5:
					GUI.TextArea(new Rect(num64 + 80f, num65 + 57f, 270f, 30f), "Loaded settings from PlayerPrefs!", 100, "Label");
					break;
				case 7:
				{
					float num107 = 22f;
					GUI.Label(new Rect(num64 + 205f, num65 + 52f, 145f, 30f), "Level Skin Mode:", "Label");
					bool flag50 = false;
					if ((int)Settings[2] == 1)
					{
						flag50 = true;
					}
					bool flag51 = GUI.Toggle(new Rect(num64 + 325f, num65 + 52f, 40f, 20f), flag50, "On");
					if (flag50 != flag51)
					{
						if (flag51)
						{
							Settings[2] = 1;
						}
						else
						{
							Settings[2] = 0;
						}
					}
					if ((int)Settings[188] == 0)
					{
						if (GUI.Button(new Rect(num64 + 375f, num65 + 51f, 120f, 22f), "Forest Skins"))
						{
							Settings[188] = 1;
						}
						GUI.Label(new Rect(num64 + 205f, num65 + 77f, 145f, 30f), "Randomized Pairs:", "Label");
						flag50 = false;
						if ((int)Settings[50] == 1)
						{
							flag50 = true;
						}
						flag51 = GUI.Toggle(new Rect(num64 + 325f, num65 + 77f, 40f, 20f), flag50, "On");
						if (flag50 != flag51)
						{
							if (flag51)
							{
								Settings[50] = 1;
							}
							else
							{
								Settings[50] = 0;
							}
						}
						scroll = GUI.BeginScrollView(new Rect(num64, num65 + 115f, 700f, 340f), scroll, new Rect(num64, num65 + 115f, 700f, 475f));
						GUI.Label(new Rect(num64 + 79f, num65 + 117f + num107 * 0f, 150f, 20f), "Ground:", "Label");
						Settings[49] = GUI.TextField(new Rect(num64 + 79f, num65 + 117f + num107 * 1f, 227f, 20f), (string)Settings[49]);
						GUI.Label(new Rect(num64 + 79f, num65 + 117f + num107 * 2f, 150f, 20f), "Forest Trunks", "Label");
						Settings[33] = GUI.TextField(new Rect(num64 + 79f, num65 + 117f + num107 * 3f, 227f, 20f), (string)Settings[33]);
						Settings[34] = GUI.TextField(new Rect(num64 + 79f, num65 + 117f + num107 * 4f, 227f, 20f), (string)Settings[34]);
						Settings[35] = GUI.TextField(new Rect(num64 + 79f, num65 + 117f + num107 * 5f, 227f, 20f), (string)Settings[35]);
						Settings[36] = GUI.TextField(new Rect(num64 + 79f, num65 + 117f + num107 * 6f, 227f, 20f), (string)Settings[36]);
						Settings[37] = GUI.TextField(new Rect(num64 + 79f, num65 + 117f + num107 * 7f, 227f, 20f), (string)Settings[37]);
						Settings[38] = GUI.TextField(new Rect(num64 + 79f, num65 + 117f + num107 * 8f, 227f, 20f), (string)Settings[38]);
						Settings[39] = GUI.TextField(new Rect(num64 + 79f, num65 + 117f + num107 * 9f, 227f, 20f), (string)Settings[39]);
						Settings[40] = GUI.TextField(new Rect(num64 + 79f, num65 + 117f + num107 * 10f, 227f, 20f), (string)Settings[40]);
						GUI.Label(new Rect(num64 + 79f, num65 + 117f + num107 * 11f, 150f, 20f), "Forest Leaves:", "Label");
						Settings[41] = GUI.TextField(new Rect(num64 + 79f, num65 + 117f + num107 * 12f, 227f, 20f), (string)Settings[41]);
						Settings[42] = GUI.TextField(new Rect(num64 + 79f, num65 + 117f + num107 * 13f, 227f, 20f), (string)Settings[42]);
						Settings[43] = GUI.TextField(new Rect(num64 + 79f, num65 + 117f + num107 * 14f, 227f, 20f), (string)Settings[43]);
						Settings[44] = GUI.TextField(new Rect(num64 + 79f, num65 + 117f + num107 * 15f, 227f, 20f), (string)Settings[44]);
						Settings[45] = GUI.TextField(new Rect(num64 + 79f, num65 + 117f + num107 * 16f, 227f, 20f), (string)Settings[45]);
						Settings[46] = GUI.TextField(new Rect(num64 + 79f, num65 + 117f + num107 * 17f, 227f, 20f), (string)Settings[46]);
						Settings[47] = GUI.TextField(new Rect(num64 + 79f, num65 + 117f + num107 * 18f, 227f, 20f), (string)Settings[47]);
						Settings[48] = GUI.TextField(new Rect(num64 + 79f, num65 + 117f + num107 * 19f, 227f, 20f), (string)Settings[48]);
						GUI.Label(new Rect(num64 + 379f, num65 + 117f + num107 * 0f, 150f, 20f), "Skybox Front:", "Label");
						Settings[163] = GUI.TextField(new Rect(num64 + 379f, num65 + 117f + num107 * 1f, 227f, 20f), (string)Settings[163]);
						GUI.Label(new Rect(num64 + 379f, num65 + 117f + num107 * 2f, 150f, 20f), "Skybox Back:", "Label");
						Settings[164] = GUI.TextField(new Rect(num64 + 379f, num65 + 117f + num107 * 3f, 227f, 20f), (string)Settings[164]);
						GUI.Label(new Rect(num64 + 379f, num65 + 117f + num107 * 4f, 150f, 20f), "Skybox Left:", "Label");
						Settings[165] = GUI.TextField(new Rect(num64 + 379f, num65 + 117f + num107 * 5f, 227f, 20f), (string)Settings[165]);
						GUI.Label(new Rect(num64 + 379f, num65 + 117f + num107 * 6f, 150f, 20f), "Skybox Right:", "Label");
						Settings[166] = GUI.TextField(new Rect(num64 + 379f, num65 + 117f + num107 * 7f, 227f, 20f), (string)Settings[166]);
						GUI.Label(new Rect(num64 + 379f, num65 + 117f + num107 * 8f, 150f, 20f), "Skybox Up:", "Label");
						Settings[167] = GUI.TextField(new Rect(num64 + 379f, num65 + 117f + num107 * 9f, 227f, 20f), (string)Settings[167]);
						GUI.Label(new Rect(num64 + 379f, num65 + 117f + num107 * 10f, 150f, 20f), "Skybox Down:", "Label");
						Settings[168] = GUI.TextField(new Rect(num64 + 379f, num65 + 117f + num107 * 11f, 227f, 20f), (string)Settings[168]);
						GUI.EndScrollView();
					}
					else if ((int)Settings[188] == 1)
					{
						if (GUI.Button(new Rect(num64 + 375f, num65 + 51f, 120f, 22f), "City Skins"))
						{
							Settings[188] = 0;
						}
						GUI.Label(new Rect(num64 + 80f, num65 + 92f + num107 * 0f, 150f, 20f), "Ground:", "Label");
						Settings[59] = GUI.TextField(new Rect(num64 + 80f, num65 + 92f + num107 * 1f, 230f, 20f), (string)Settings[59]);
						GUI.Label(new Rect(num64 + 80f, num65 + 92f + num107 * 2f, 150f, 20f), "Wall:", "Label");
						Settings[60] = GUI.TextField(new Rect(num64 + 80f, num65 + 92f + num107 * 3f, 230f, 20f), (string)Settings[60]);
						GUI.Label(new Rect(num64 + 80f, num65 + 92f + num107 * 4f, 150f, 20f), "Gate:", "Label");
						Settings[61] = GUI.TextField(new Rect(num64 + 80f, num65 + 92f + num107 * 5f, 230f, 20f), (string)Settings[61]);
						GUI.Label(new Rect(num64 + 80f, num65 + 92f + num107 * 6f, 150f, 20f), "Houses:", "Label");
						Settings[51] = GUI.TextField(new Rect(num64 + 80f, num65 + 92f + num107 * 7f, 230f, 20f), (string)Settings[51]);
						Settings[52] = GUI.TextField(new Rect(num64 + 80f, num65 + 92f + num107 * 8f, 230f, 20f), (string)Settings[52]);
						Settings[53] = GUI.TextField(new Rect(num64 + 80f, num65 + 92f + num107 * 9f, 230f, 20f), (string)Settings[53]);
						Settings[54] = GUI.TextField(new Rect(num64 + 80f, num65 + 92f + num107 * 10f, 230f, 20f), (string)Settings[54]);
						Settings[55] = GUI.TextField(new Rect(num64 + 80f, num65 + 92f + num107 * 11f, 230f, 20f), (string)Settings[55]);
						Settings[56] = GUI.TextField(new Rect(num64 + 80f, num65 + 92f + num107 * 12f, 230f, 20f), (string)Settings[56]);
						Settings[57] = GUI.TextField(new Rect(num64 + 80f, num65 + 92f + num107 * 13f, 230f, 20f), (string)Settings[57]);
						Settings[58] = GUI.TextField(new Rect(num64 + 80f, num65 + 92f + num107 * 14f, 230f, 20f), (string)Settings[58]);
						GUI.Label(new Rect(num64 + 390f, num65 + 92f + num107 * 0f, 150f, 20f), "Skybox Front:", "Label");
						Settings[169] = GUI.TextField(new Rect(num64 + 390f, num65 + 92f + num107 * 1f, 230f, 20f), (string)Settings[169]);
						GUI.Label(new Rect(num64 + 390f, num65 + 92f + num107 * 2f, 150f, 20f), "Skybox Back:", "Label");
						Settings[170] = GUI.TextField(new Rect(num64 + 390f, num65 + 92f + num107 * 3f, 230f, 20f), (string)Settings[170]);
						GUI.Label(new Rect(num64 + 390f, num65 + 92f + num107 * 4f, 150f, 20f), "Skybox Left:", "Label");
						Settings[171] = GUI.TextField(new Rect(num64 + 390f, num65 + 92f + num107 * 5f, 230f, 20f), (string)Settings[171]);
						GUI.Label(new Rect(num64 + 390f, num65 + 92f + num107 * 6f, 150f, 20f), "Skybox Right:", "Label");
						Settings[172] = GUI.TextField(new Rect(num64 + 390f, num65 + 92f + num107 * 7f, 230f, 20f), (string)Settings[172]);
						GUI.Label(new Rect(num64 + 390f, num65 + 92f + num107 * 8f, 150f, 20f), "Skybox Up:", "Label");
						Settings[173] = GUI.TextField(new Rect(num64 + 390f, num65 + 92f + num107 * 9f, 230f, 20f), (string)Settings[173]);
						GUI.Label(new Rect(num64 + 390f, num65 + 92f + num107 * 10f, 150f, 20f), "Skybox Down:", "Label");
						Settings[174] = GUI.TextField(new Rect(num64 + 390f, num65 + 92f + num107 * 11f, 230f, 20f), (string)Settings[174]);
					}
					break;
				}
				case 8:
				{
					GUI.Label(new Rect(num64 + 150f, num65 + 51f, 120f, 22f), "Map Settings", "Label");
					GUI.Label(new Rect(num64 + 50f, num65 + 81f, 140f, 20f), "Titan Spawn Cap:", "Label");
					Settings[85] = GUI.TextField(new Rect(num64 + 155f, num65 + 81f, 30f, 20f), (string)Settings[85]);
					string[] texts = new string[5] { "1 Round", "Waves", "PVP", "Racing", "Custom" };
					RCSettings.GameType = GUI.SelectionGrid(new Rect(num64 + 190f, num65 + 80f, 140f, 60f), RCSettings.GameType, texts, 2, GUI.skin.toggle);
					GUI.Label(new Rect(num64 + 150f, num65 + 155f, 150f, 20f), "Level Script:", "Label");
					CurrentScript = GUI.TextField(new Rect(num64 + 50f, num65 + 180f, 275f, 220f), CurrentScript);
					if (GUI.Button(new Rect(num64 + 100f, num65 + 410f, 50f, 25f), "Copy"))
					{
						TextEditor textEditor3 = new TextEditor();
						textEditor3.content = new GUIContent(CurrentScript);
						textEditor3.SelectAll();
						textEditor3.Copy();
					}
					else if (GUI.Button(new Rect(num64 + 225f, num65 + 410f, 50f, 25f), "Clear"))
					{
						CurrentScript = string.Empty;
					}
					GUI.Label(new Rect(num64 + 455f, num65 + 51f, 180f, 20f), "Custom Textures", "Label");
					GUI.Label(new Rect(num64 + 375f, num65 + 81f, 180f, 20f), "Ground Skin:", "Label");
					Settings[162] = GUI.TextField(new Rect(num64 + 375f, num65 + 103f, 275f, 20f), (string)Settings[162]);
					GUI.Label(new Rect(num64 + 375f, num65 + 125f, 150f, 20f), "Skybox Front:", "Label");
					Settings[175] = GUI.TextField(new Rect(num64 + 375f, num65 + 147f, 275f, 20f), (string)Settings[175]);
					GUI.Label(new Rect(num64 + 375f, num65 + 169f, 150f, 20f), "Skybox Back:", "Label");
					Settings[176] = GUI.TextField(new Rect(num64 + 375f, num65 + 191f, 275f, 20f), (string)Settings[176]);
					GUI.Label(new Rect(num64 + 375f, num65 + 213f, 150f, 20f), "Skybox Left:", "Label");
					Settings[177] = GUI.TextField(new Rect(num64 + 375f, num65 + 235f, 275f, 20f), (string)Settings[177]);
					GUI.Label(new Rect(num64 + 375f, num65 + 257f, 150f, 20f), "Skybox Right:", "Label");
					Settings[178] = GUI.TextField(new Rect(num64 + 375f, num65 + 279f, 275f, 20f), (string)Settings[178]);
					GUI.Label(new Rect(num64 + 375f, num65 + 301f, 150f, 20f), "Skybox Up:", "Label");
					Settings[179] = GUI.TextField(new Rect(num64 + 375f, num65 + 323f, 275f, 20f), (string)Settings[179]);
					GUI.Label(new Rect(num64 + 375f, num65 + 345f, 150f, 20f), "Skybox Down:", "Label");
					Settings[180] = GUI.TextField(new Rect(num64 + 375f, num65 + 367f, 275f, 20f), (string)Settings[180]);
					break;
				}
				case 9:
					CurrentScriptLogic = GUI.TextField(new Rect(num64 + 50f, num65 + 82f, 600f, 270f), CurrentScriptLogic);
					if (GUI.Button(new Rect(num64 + 250f, num65 + 365f, 50f, 20f), "Copy"))
					{
						TextEditor textEditor4 = new TextEditor();
						textEditor4.content = new GUIContent(CurrentScriptLogic);
						textEditor4.SelectAll();
						textEditor4.Copy();
					}
					else if (GUI.Button(new Rect(num64 + 400f, num65 + 365f, 50f, 20f), "Clear"))
					{
						CurrentScriptLogic = string.Empty;
					}
					break;
				case 10:
					GUI.Label(new Rect(num64 + 200f, num65 + 382f, 400f, 22f), "Master Client only. Changes will take effect upon restart.");
					if (GUI.Button(new Rect(num64 + 267.5f, num65 + 50f, 60f, 25f), "Titans"))
					{
						Settings[230] = 0;
					}
					else if (GUI.Button(new Rect(num64 + 332.5f, num65 + 50f, 40f, 25f), "PVP"))
					{
						Settings[230] = 1;
					}
					else if (GUI.Button(new Rect(num64 + 377.5f, num65 + 50f, 50f, 25f), "Misc"))
					{
						Settings[230] = 2;
					}
					else if (GUI.Button(new Rect(num64 + 320f, num65 + 415f, 60f, 30f), "Reset"))
					{
						Settings[192] = 0;
						Settings[193] = 0;
						Settings[194] = 0;
						Settings[195] = 0;
						Settings[196] = "30";
						Settings[197] = 0;
						Settings[198] = "100";
						Settings[199] = "200";
						Settings[200] = 0;
						Settings[201] = "1";
						Settings[202] = 0;
						Settings[203] = 0;
						Settings[204] = "1";
						Settings[205] = 0;
						Settings[206] = "1000";
						Settings[207] = 0;
						Settings[208] = "1.0";
						Settings[209] = "3.0";
						Settings[210] = 0;
						Settings[211] = "20.0";
						Settings[212] = "20.0";
						Settings[213] = "20.0";
						Settings[214] = "20.0";
						Settings[215] = "20.0";
						Settings[216] = 0;
						Settings[217] = 0;
						Settings[218] = "1";
						Settings[219] = 0;
						Settings[220] = 0;
						Settings[221] = 0;
						Settings[222] = "20";
						Settings[223] = 0;
						Settings[224] = "10";
						Settings[225] = string.Empty;
						Settings[226] = 0;
						Settings[227] = "50";
						Settings[228] = 0;
						Settings[229] = 0;
						Settings[235] = 0;
					}
					if ((int)Settings[230] == 0)
					{
						GUI.Label(new Rect(num64 + 100f, num65 + 90f, 160f, 22f), "Custom Titan Number:", "Label");
						GUI.Label(new Rect(num64 + 100f, num65 + 112f, 200f, 22f), "Amount (Integer):", "Label");
						Settings[204] = GUI.TextField(new Rect(num64 + 250f, num65 + 112f, 50f, 22f), (string)Settings[204]);
						bool flag44 = false;
						if ((int)Settings[203] == 1)
						{
							flag44 = true;
						}
						bool flag45 = GUI.Toggle(new Rect(num64 + 250f, num65 + 90f, 40f, 20f), flag44, "On");
						if (flag44 != flag45)
						{
							if (flag45)
							{
								Settings[203] = 1;
							}
							else
							{
								Settings[203] = 0;
							}
						}
						GUI.Label(new Rect(num64 + 100f, num65 + 152f, 160f, 22f), "Custom Titan Spawns:", "Label");
						flag44 = false;
						if ((int)Settings[210] == 1)
						{
							flag44 = true;
						}
						flag45 = GUI.Toggle(new Rect(num64 + 250f, num65 + 152f, 40f, 20f), flag44, "On");
						if (flag44 != flag45)
						{
							if (flag45)
							{
								Settings[210] = 1;
							}
							else
							{
								Settings[210] = 0;
							}
						}
						GUI.Label(new Rect(num64 + 100f, num65 + 174f, 150f, 22f), "Normal (Decimal):", "Label");
						GUI.Label(new Rect(num64 + 100f, num65 + 196f, 150f, 22f), "Aberrant (Decimal):", "Label");
						GUI.Label(new Rect(num64 + 100f, num65 + 218f, 150f, 22f), "Jumper (Decimal):", "Label");
						GUI.Label(new Rect(num64 + 100f, num65 + 240f, 150f, 22f), "Crawler (Decimal):", "Label");
						GUI.Label(new Rect(num64 + 100f, num65 + 262f, 150f, 22f), "Punk (Decimal):", "Label");
						Settings[211] = GUI.TextField(new Rect(num64 + 250f, num65 + 174f, 50f, 22f), (string)Settings[211]);
						Settings[212] = GUI.TextField(new Rect(num64 + 250f, num65 + 196f, 50f, 22f), (string)Settings[212]);
						Settings[213] = GUI.TextField(new Rect(num64 + 250f, num65 + 218f, 50f, 22f), (string)Settings[213]);
						Settings[214] = GUI.TextField(new Rect(num64 + 250f, num65 + 240f, 50f, 22f), (string)Settings[214]);
						Settings[215] = GUI.TextField(new Rect(num64 + 250f, num65 + 262f, 50f, 22f), (string)Settings[215]);
						GUI.Label(new Rect(num64 + 100f, num65 + 302f, 160f, 22f), "Titan Size Mode:", "Label");
						GUI.Label(new Rect(num64 + 100f, num65 + 324f, 150f, 22f), "Minimum (Decimal):", "Label");
						GUI.Label(new Rect(num64 + 100f, num65 + 346f, 150f, 22f), "Maximum (Decimal):", "Label");
						Settings[208] = GUI.TextField(new Rect(num64 + 250f, num65 + 324f, 50f, 22f), (string)Settings[208]);
						Settings[209] = GUI.TextField(new Rect(num64 + 250f, num65 + 346f, 50f, 22f), (string)Settings[209]);
						flag44 = false;
						if ((int)Settings[207] == 1)
						{
							flag44 = true;
						}
						flag45 = GUI.Toggle(new Rect(num64 + 250f, num65 + 302f, 40f, 20f), flag44, "On");
						if (flag45 != flag44)
						{
							if (flag45)
							{
								Settings[207] = 1;
							}
							else
							{
								Settings[207] = 0;
							}
						}
						GUI.Label(new Rect(num64 + 400f, num65 + 90f, 160f, 22f), "Titan Health Mode:", "Label");
						GUI.Label(new Rect(num64 + 400f, num65 + 161f, 150f, 22f), "Minimum (Integer):", "Label");
						GUI.Label(new Rect(num64 + 400f, num65 + 183f, 150f, 22f), "Maximum (Integer):", "Label");
						Settings[198] = GUI.TextField(new Rect(num64 + 550f, num65 + 161f, 50f, 22f), (string)Settings[198]);
						Settings[199] = GUI.TextField(new Rect(num64 + 550f, num65 + 183f, 50f, 22f), (string)Settings[199]);
						string[] texts3 = new string[3] { "Off", "Fixed", "Scaled" };
						Settings[197] = GUI.SelectionGrid(new Rect(num64 + 550f, num65 + 90f, 100f, 66f), (int)Settings[197], texts3, 1, GUI.skin.toggle);
						GUI.Label(new Rect(num64 + 400f, num65 + 223f, 160f, 22f), "Titan Damage Mode:", "Label");
						GUI.Label(new Rect(num64 + 400f, num65 + 245f, 150f, 22f), "Damage (Integer):", "Label");
						Settings[206] = GUI.TextField(new Rect(num64 + 550f, num65 + 245f, 50f, 22f), (string)Settings[206]);
						flag44 = false;
						if ((int)Settings[205] == 1)
						{
							flag44 = true;
						}
						flag45 = GUI.Toggle(new Rect(num64 + 550f, num65 + 223f, 40f, 20f), flag44, "On");
						if (flag44 != flag45)
						{
							if (flag45)
							{
								Settings[205] = 1;
							}
							else
							{
								Settings[205] = 0;
							}
						}
						GUI.Label(new Rect(num64 + 400f, num65 + 285f, 160f, 22f), "Titan Explode Mode:", "Label");
						GUI.Label(new Rect(num64 + 400f, num65 + 307f, 160f, 22f), "Radius (Integer):", "Label");
						Settings[196] = GUI.TextField(new Rect(num64 + 550f, num65 + 307f, 50f, 22f), (string)Settings[196]);
						flag44 = false;
						if ((int)Settings[195] == 1)
						{
							flag44 = true;
						}
						flag45 = GUI.Toggle(new Rect(num64 + 550f, num65 + 285f, 40f, 20f), flag44, "On");
						if (flag44 != flag45)
						{
							if (flag45)
							{
								Settings[195] = 1;
							}
							else
							{
								Settings[195] = 0;
							}
						}
						GUI.Label(new Rect(num64 + 400f, num65 + 347f, 160f, 22f), "Disable Rock Throwing:", "Label");
						flag44 = false;
						if ((int)Settings[194] == 1)
						{
							flag44 = true;
						}
						flag45 = GUI.Toggle(new Rect(num64 + 550f, num65 + 347f, 40f, 20f), flag44, "On");
						if (flag44 != flag45)
						{
							if (flag45)
							{
								Settings[194] = 1;
							}
							else
							{
								Settings[194] = 0;
							}
						}
					}
					else if ((int)Settings[230] == 1)
					{
						GUI.Label(new Rect(num64 + 100f, num65 + 90f, 160f, 22f), "Point Mode:", "Label");
						GUI.Label(new Rect(num64 + 100f, num65 + 112f, 160f, 22f), "Max Points (Integer):", "Label");
						Settings[227] = GUI.TextField(new Rect(num64 + 250f, num65 + 112f, 50f, 22f), (string)Settings[227]);
						bool flag46 = false;
						if ((int)Settings[226] == 1)
						{
							flag46 = true;
						}
						bool flag47 = GUI.Toggle(new Rect(num64 + 250f, num65 + 90f, 40f, 20f), flag46, "On");
						if (flag46 != flag47)
						{
							if (flag47)
							{
								Settings[226] = 1;
							}
							else
							{
								Settings[226] = 0;
							}
						}
						GUI.Label(new Rect(num64 + 100f, num65 + 152f, 160f, 22f), "PVP Bomb Mode:", "Label");
						flag46 = false;
						if ((int)Settings[192] == 1)
						{
							flag46 = true;
						}
						flag47 = GUI.Toggle(new Rect(num64 + 250f, num65 + 152f, 40f, 20f), flag46, "On");
						if (flag46 != flag47)
						{
							if (flag47)
							{
								Settings[192] = 1;
							}
							else
							{
								Settings[192] = 0;
							}
						}
						GUI.Label(new Rect(num64 + 100f, num65 + 182f, 100f, 66f), "Team Mode:", "Label");
						string[] texts4 = new string[4] { "Off", "No Sort", "Size-Lock", "Skill-Lock" };
						Settings[193] = GUI.SelectionGrid(new Rect(num64 + 250f, num65 + 182f, 120f, 88f), (int)Settings[193], texts4, 1, GUI.skin.toggle);
						GUI.Label(new Rect(num64 + 100f, num65 + 278f, 160f, 22f), "Infection Mode:", "Label");
						GUI.Label(new Rect(num64 + 100f, num65 + 300f, 160f, 22f), "Starting Titans (Integer):", "Label");
						Settings[201] = GUI.TextField(new Rect(num64 + 250f, num65 + 300f, 50f, 22f), (string)Settings[201]);
						flag46 = false;
						if ((int)Settings[200] == 1)
						{
							flag46 = true;
						}
						flag47 = GUI.Toggle(new Rect(num64 + 250f, num65 + 278f, 40f, 20f), flag46, "On");
						if (flag46 != flag47)
						{
							if (flag47)
							{
								Settings[200] = 1;
							}
							else
							{
								Settings[200] = 0;
							}
						}
						GUI.Label(new Rect(num64 + 100f, num65 + 330f, 160f, 22f), "Friendly Mode:", "Label");
						flag46 = false;
						if ((int)Settings[219] == 1)
						{
							flag46 = true;
						}
						flag47 = GUI.Toggle(new Rect(num64 + 250f, num65 + 330f, 40f, 20f), flag46, "On");
						if (flag46 != flag47)
						{
							if (flag47)
							{
								Settings[219] = 1;
							}
							else
							{
								Settings[219] = 0;
							}
						}
						GUI.Label(new Rect(num64 + 400f, num65 + 90f, 160f, 22f), "Sword/AHSS PVP:", "Label");
						texts4 = new string[3] { "Off", "Teams", "FFA" };
						Settings[220] = GUI.SelectionGrid(new Rect(num64 + 550f, num65 + 90f, 100f, 66f), (int)Settings[220], texts4, 1, GUI.skin.toggle);
						GUI.Label(new Rect(num64 + 400f, num65 + 164f, 160f, 22f), "No AHSS Air-Reloading:", "Label");
						flag46 = false;
						if ((int)Settings[228] == 1)
						{
							flag46 = true;
						}
						flag47 = GUI.Toggle(new Rect(num64 + 550f, num65 + 164f, 40f, 20f), flag46, "On");
						if (flag46 != flag47)
						{
							if (flag47)
							{
								Settings[228] = 1;
							}
							else
							{
								Settings[228] = 0;
							}
						}
						GUI.Label(new Rect(num64 + 400f, num65 + 194f, 160f, 22f), "Cannons kill humans:", "Label");
						flag46 = false;
						if ((int)Settings[261] == 1)
						{
							flag46 = true;
						}
						flag47 = GUI.Toggle(new Rect(num64 + 550f, num65 + 194f, 40f, 20f), flag46, "On");
						if (flag46 != flag47)
						{
							if (flag47)
							{
								Settings[261] = 1;
							}
							else
							{
								Settings[261] = 0;
							}
						}
					}
					else
					{
						if ((int)Settings[230] != 2)
						{
							break;
						}
						GUI.Label(new Rect(num64 + 100f, num65 + 90f, 160f, 22f), "Custom Titans/Wave:", "Label");
						GUI.Label(new Rect(num64 + 100f, num65 + 112f, 160f, 22f), "Amount (Integer):", "Label");
						Settings[218] = GUI.TextField(new Rect(num64 + 250f, num65 + 112f, 50f, 22f), (string)Settings[218]);
						bool flag48 = false;
						if ((int)Settings[217] == 1)
						{
							flag48 = true;
						}
						bool flag49 = GUI.Toggle(new Rect(num64 + 250f, num65 + 90f, 40f, 20f), flag48, "On");
						if (flag48 != flag49)
						{
							if (flag49)
							{
								Settings[217] = 1;
							}
							else
							{
								Settings[217] = 0;
							}
						}
						GUI.Label(new Rect(num64 + 100f, num65 + 152f, 160f, 22f), "Maximum Waves:", "Label");
						GUI.Label(new Rect(num64 + 100f, num65 + 174f, 160f, 22f), "Amount (Integer):", "Label");
						Settings[222] = GUI.TextField(new Rect(num64 + 250f, num65 + 174f, 50f, 22f), (string)Settings[222]);
						flag48 = false;
						if ((int)Settings[221] == 1)
						{
							flag48 = true;
						}
						flag49 = GUI.Toggle(new Rect(num64 + 250f, num65 + 152f, 40f, 20f), flag48, "On");
						if (flag48 != flag49)
						{
							if (flag49)
							{
								Settings[221] = 1;
							}
							else
							{
								Settings[221] = 0;
							}
						}
						GUI.Label(new Rect(num64 + 100f, num65 + 214f, 160f, 22f), "Punks every 5 waves:", "Label");
						flag48 = false;
						if ((int)Settings[229] == 1)
						{
							flag48 = true;
						}
						flag49 = GUI.Toggle(new Rect(num64 + 250f, num65 + 214f, 40f, 20f), flag48, "On");
						if (flag48 != flag49)
						{
							if (flag49)
							{
								Settings[229] = 1;
							}
							else
							{
								Settings[229] = 0;
							}
						}
						GUI.Label(new Rect(num64 + 100f, num65 + 244f, 160f, 22f), "Global Minimap Disable:", "Label");
						flag48 = false;
						if ((int)Settings[235] == 1)
						{
							flag48 = true;
						}
						flag49 = GUI.Toggle(new Rect(num64 + 250f, num65 + 244f, 40f, 20f), flag48, "On");
						if (flag48 != flag49)
						{
							if (flag49)
							{
								Settings[235] = 1;
							}
							else
							{
								Settings[235] = 0;
							}
						}
						GUI.Label(new Rect(num64 + 400f, num65 + 90f, 160f, 22f), "Endless Respawn:", "Label");
						GUI.Label(new Rect(num64 + 400f, num65 + 112f, 160f, 22f), "Respawn Time (Integer):", "Label");
						Settings[224] = GUI.TextField(new Rect(num64 + 550f, num65 + 112f, 50f, 22f), (string)Settings[224]);
						flag48 = false;
						if ((int)Settings[223] == 1)
						{
							flag48 = true;
						}
						flag49 = GUI.Toggle(new Rect(num64 + 550f, num65 + 90f, 40f, 20f), flag48, "On");
						if (flag48 != flag49)
						{
							if (flag49)
							{
								Settings[223] = 1;
							}
							else
							{
								Settings[223] = 0;
							}
						}
						GUI.Label(new Rect(num64 + 400f, num65 + 152f, 160f, 22f), "Kick Eren Titan:", "Label");
						flag48 = false;
						if ((int)Settings[202] == 1)
						{
							flag48 = true;
						}
						flag49 = GUI.Toggle(new Rect(num64 + 550f, num65 + 152f, 40f, 20f), flag48, "On");
						if (flag48 != flag49)
						{
							if (flag49)
							{
								Settings[202] = 1;
							}
							else
							{
								Settings[202] = 0;
							}
						}
						GUI.Label(new Rect(num64 + 400f, num65 + 182f, 160f, 22f), "Allow Horses:", "Label");
						flag48 = false;
						if ((int)Settings[216] == 1)
						{
							flag48 = true;
						}
						flag49 = GUI.Toggle(new Rect(num64 + 550f, num65 + 182f, 40f, 20f), flag48, "On");
						if (flag48 != flag49)
						{
							if (flag49)
							{
								Settings[216] = 1;
							}
							else
							{
								Settings[216] = 0;
							}
						}
						GUI.Label(new Rect(num64 + 400f, num65 + 212f, 180f, 22f), "Message of the Day:", "Label");
						Settings[225] = GUI.TextArea(new Rect(num64 + 400f, num65 + 234f, 200f, 100f), (string)Settings[225]);
					}
					break;
				case 11:
				{
					GUI.Label(new Rect(num64 + 150f, num65 + 80f, 185f, 22f), "Bomb Mode", "Label");
					GUI.Label(new Rect(num64 + 80f, num65 + 110f, 80f, 22f), "Color:", "Label");
					Texture2D texture2D2 = new Texture2D(1, 1, TextureFormat.ARGB32, mipmap: false);
					texture2D2.SetPixel(0, 0, new Color((float)Settings[246], (float)Settings[247], (float)Settings[248], (float)Settings[249]));
					texture2D2.Apply();
					GUI.DrawTexture(new Rect(num64 + 120f, num65 + 113f, 40f, 15f), texture2D2, ScaleMode.StretchToFill);
					UnityEngine.Object.Destroy(texture2D2);
					GUI.Label(new Rect(num64 + 72f, num65 + 135f, 20f, 22f), "R:", "Label");
					Settings[246] = GUI.HorizontalSlider(new Rect(num64 + 92f, num65 + 138f, 100f, 20f), (float)Settings[246], 0f, 1f);
					GUI.Label(new Rect(num64 + 72f, num65 + 160f, 20f, 22f), "G:", "Label");
					Settings[247] = GUI.HorizontalSlider(new Rect(num64 + 92f, num65 + 163f, 100f, 20f), (float)Settings[247], 0f, 1f);
					GUI.Label(new Rect(num64 + 72f, num65 + 185f, 20f, 22f), "B:", "Label");
					Settings[248] = GUI.HorizontalSlider(new Rect(num64 + 92f, num65 + 188f, 100f, 20f), (float)Settings[248], 0f, 1f);
					GUI.Label(new Rect(num64 + 72f, num65 + 210f, 20f, 22f), "A:", "Label");
					Settings[249] = GUI.HorizontalSlider(new Rect(num64 + 92f, num65 + 213f, 100f, 20f), (float)Settings[249], 0.5f, 1f);
					GUI.Label(new Rect(num64 + 72f, num65 + 235f, 95f, 22f), "Bomb Radius:", "Label");
					GUI.Label(new Rect(num64 + 168f, num65 + 235f, 20f, 22f), ((float)Settings[250]).ToString(), "Label");
					GUI.Label(new Rect(num64 + 72f, num65 + 260f, 95f, 22f), "Bomb Range:", "Label");
					GUI.Label(new Rect(num64 + 168f, num65 + 260f, 20f, 22f), ((float)Settings[251]).ToString(), "Label");
					GUI.Label(new Rect(num64 + 72f, num65 + 285f, 95f, 22f), "Bomb Speed:", "Label");
					GUI.Label(new Rect(num64 + 168f, num65 + 285f, 20f, 22f), ((float)Settings[252]).ToString(), "Label");
					GUI.Label(new Rect(num64 + 72f, num65 + 310f, 95f, 22f), "Bomb CD:", "Label");
					GUI.Label(new Rect(num64 + 168f, num65 + 310f, 20f, 22f), ((float)Settings[253]).ToString(), "Label");
					float num66 = 20f - (float)Settings[250] - (float)Settings[251] - (float)Settings[252] - (float)Settings[253];
					if (GUI.Button(new Rect(num64 + 190f, num65 + 235f, 20f, 20f), "-"))
					{
						if ((float)Settings[250] > 0f)
						{
							Settings[250] = (float)Settings[250] - 1f;
						}
					}
					else if (GUI.Button(new Rect(num64 + 215f, num65 + 235f, 20f, 20f), "+") && (float)Settings[250] < 10f && num66 > 0f)
					{
						Settings[250] = (float)Settings[250] + 1f;
					}
					if (GUI.Button(new Rect(num64 + 190f, num65 + 260f, 20f, 20f), "-"))
					{
						if ((float)Settings[251] > 0f)
						{
							Settings[251] = (float)Settings[251] - 1f;
						}
					}
					else if (GUI.Button(new Rect(num64 + 215f, num65 + 260f, 20f, 20f), "+") && (float)Settings[251] < 3f && num66 > 0f)
					{
						Settings[251] = (float)Settings[251] + 1f;
					}
					if (GUI.Button(new Rect(num64 + 190f, num65 + 285f, 20f, 20f), "-"))
					{
						if ((float)Settings[252] > 0f)
						{
							Settings[252] = (float)Settings[252] - 1f;
						}
					}
					else if (GUI.Button(new Rect(num64 + 215f, num65 + 285f, 20f, 20f), "+") && (float)Settings[252] < 10f && num66 > 0f)
					{
						Settings[252] = (float)Settings[252] + 1f;
					}
					if (GUI.Button(new Rect(num64 + 190f, num65 + 310f, 20f, 20f), "-"))
					{
						if ((float)Settings[253] > 4f)
						{
							Settings[253] = (float)Settings[253] - 1f;
						}
					}
					else if (GUI.Button(new Rect(num64 + 215f, num65 + 310f, 20f, 20f), "+") && (float)Settings[253] < 10f && num66 > 0f)
					{
						Settings[253] = (float)Settings[253] + 1f;
					}
					GUI.Label(new Rect(num64 + 72f, num65 + 335f, 95f, 22f), "Unused Points:", "Label");
					GUI.Label(new Rect(num64 + 168f, num65 + 335f, 20f, 22f), num66.ToString(), "Label");
					if (GUI.Button(new Rect(num64 + 72f, num65 + 360f, 163f, 20f), "Reset Points"))
					{
						Settings[250] = 0f;
						Settings[251] = 0f;
						Settings[252] = 0f;
						Settings[253] = 4f;
					}
					break;
				}
				}
				if (GUI.Button(new Rect(num64 + 416f, num65 + 468f, 42f, 25f), "Save"))
				{
					PlayerPrefs.SetInt("human", (int)Settings[0]);
					PlayerPrefs.SetInt("titan", (int)Settings[1]);
					PlayerPrefs.SetInt("level", (int)Settings[2]);
					PlayerPrefs.SetString("horse", (string)Settings[3]);
					PlayerPrefs.SetString("hair", (string)Settings[4]);
					PlayerPrefs.SetString("eye", (string)Settings[5]);
					PlayerPrefs.SetString("glass", (string)Settings[6]);
					PlayerPrefs.SetString("face", (string)Settings[7]);
					PlayerPrefs.SetString("skin", (string)Settings[8]);
					PlayerPrefs.SetString("costume", (string)Settings[9]);
					PlayerPrefs.SetString("logo", (string)Settings[10]);
					PlayerPrefs.SetString("bladel", (string)Settings[11]);
					PlayerPrefs.SetString("blader", (string)Settings[12]);
					PlayerPrefs.SetString("gas", (string)Settings[13]);
					PlayerPrefs.SetString("haircolor", (string)Settings[14]);
					PlayerPrefs.SetInt("gasenable", (int)Settings[15]);
					PlayerPrefs.SetInt("titantype1", (int)Settings[16]);
					PlayerPrefs.SetInt("titantype2", (int)Settings[17]);
					PlayerPrefs.SetInt("titantype3", (int)Settings[18]);
					PlayerPrefs.SetInt("titantype4", (int)Settings[19]);
					PlayerPrefs.SetInt("titantype5", (int)Settings[20]);
					PlayerPrefs.SetString("titanhair1", (string)Settings[21]);
					PlayerPrefs.SetString("titanhair2", (string)Settings[22]);
					PlayerPrefs.SetString("titanhair3", (string)Settings[23]);
					PlayerPrefs.SetString("titanhair4", (string)Settings[24]);
					PlayerPrefs.SetString("titanhair5", (string)Settings[25]);
					PlayerPrefs.SetString("titaneye1", (string)Settings[26]);
					PlayerPrefs.SetString("titaneye2", (string)Settings[27]);
					PlayerPrefs.SetString("titaneye3", (string)Settings[28]);
					PlayerPrefs.SetString("titaneye4", (string)Settings[29]);
					PlayerPrefs.SetString("titaneye5", (string)Settings[30]);
					PlayerPrefs.SetInt("titanR", (int)Settings[32]);
					PlayerPrefs.SetString("tree1", (string)Settings[33]);
					PlayerPrefs.SetString("tree2", (string)Settings[34]);
					PlayerPrefs.SetString("tree3", (string)Settings[35]);
					PlayerPrefs.SetString("tree4", (string)Settings[36]);
					PlayerPrefs.SetString("tree5", (string)Settings[37]);
					PlayerPrefs.SetString("tree6", (string)Settings[38]);
					PlayerPrefs.SetString("tree7", (string)Settings[39]);
					PlayerPrefs.SetString("tree8", (string)Settings[40]);
					PlayerPrefs.SetString("leaf1", (string)Settings[41]);
					PlayerPrefs.SetString("leaf2", (string)Settings[42]);
					PlayerPrefs.SetString("leaf3", (string)Settings[43]);
					PlayerPrefs.SetString("leaf4", (string)Settings[44]);
					PlayerPrefs.SetString("leaf5", (string)Settings[45]);
					PlayerPrefs.SetString("leaf6", (string)Settings[46]);
					PlayerPrefs.SetString("leaf7", (string)Settings[47]);
					PlayerPrefs.SetString("leaf8", (string)Settings[48]);
					PlayerPrefs.SetString("forestG", (string)Settings[49]);
					PlayerPrefs.SetInt("forestR", (int)Settings[50]);
					PlayerPrefs.SetString("house1", (string)Settings[51]);
					PlayerPrefs.SetString("house2", (string)Settings[52]);
					PlayerPrefs.SetString("house3", (string)Settings[53]);
					PlayerPrefs.SetString("house4", (string)Settings[54]);
					PlayerPrefs.SetString("house5", (string)Settings[55]);
					PlayerPrefs.SetString("house6", (string)Settings[56]);
					PlayerPrefs.SetString("house7", (string)Settings[57]);
					PlayerPrefs.SetString("house8", (string)Settings[58]);
					PlayerPrefs.SetString("cityG", (string)Settings[59]);
					PlayerPrefs.SetString("cityW", (string)Settings[60]);
					PlayerPrefs.SetString("cityH", (string)Settings[61]);
					PlayerPrefs.SetInt("skinQ", QualitySettings.masterTextureLimit);
					PlayerPrefs.SetInt("skinQL", (int)Settings[63]);
					PlayerPrefs.SetString("eren", (string)Settings[65]);
					PlayerPrefs.SetString("annie", (string)Settings[66]);
					PlayerPrefs.SetString("colossal", (string)Settings[67]);
					PlayerPrefs.SetString("hoodie", (string)Settings[14]);
					PlayerPrefs.SetString("cnumber", (string)Settings[82]);
					PlayerPrefs.SetString("cmax", (string)Settings[85]);
					PlayerPrefs.SetString("titanbody1", (string)Settings[86]);
					PlayerPrefs.SetString("titanbody2", (string)Settings[87]);
					PlayerPrefs.SetString("titanbody3", (string)Settings[88]);
					PlayerPrefs.SetString("titanbody4", (string)Settings[89]);
					PlayerPrefs.SetString("titanbody5", (string)Settings[90]);
					PlayerPrefs.SetInt("customlevel", (int)Settings[91]);
					PlayerPrefs.SetInt("traildisable", (int)Settings[92]);
					PlayerPrefs.SetInt("wind", (int)Settings[93]);
					PlayerPrefs.SetString("trailskin", (string)Settings[94]);
					PlayerPrefs.SetString("snapshot", (string)Settings[95]);
					PlayerPrefs.SetString("trailskin2", (string)Settings[96]);
					PlayerPrefs.SetInt("reel", (int)Settings[97]);
					PlayerPrefs.SetString("reelin", (string)Settings[98]);
					PlayerPrefs.SetString("reelout", (string)Settings[99]);
					PlayerPrefs.SetFloat("vol", AudioListener.volume);
					PlayerPrefs.SetString("tforward", (string)Settings[101]);
					PlayerPrefs.SetString("tback", (string)Settings[102]);
					PlayerPrefs.SetString("tleft", (string)Settings[103]);
					PlayerPrefs.SetString("tright", (string)Settings[104]);
					PlayerPrefs.SetString("twalk", (string)Settings[105]);
					PlayerPrefs.SetString("tjump", (string)Settings[106]);
					PlayerPrefs.SetString("tpunch", (string)Settings[107]);
					PlayerPrefs.SetString("tslam", (string)Settings[108]);
					PlayerPrefs.SetString("tgrabfront", (string)Settings[109]);
					PlayerPrefs.SetString("tgrabback", (string)Settings[110]);
					PlayerPrefs.SetString("tgrabnape", (string)Settings[111]);
					PlayerPrefs.SetString("tantiae", (string)Settings[112]);
					PlayerPrefs.SetString("tbite", (string)Settings[113]);
					PlayerPrefs.SetString("tcover", (string)Settings[114]);
					PlayerPrefs.SetString("tsit", (string)Settings[115]);
					PlayerPrefs.SetInt("reel2", (int)Settings[116]);
					PlayerPrefs.SetInt("humangui", (int)Settings[133]);
					PlayerPrefs.SetString("horse2", (string)Settings[134]);
					PlayerPrefs.SetString("hair2", (string)Settings[135]);
					PlayerPrefs.SetString("eye2", (string)Settings[136]);
					PlayerPrefs.SetString("glass2", (string)Settings[137]);
					PlayerPrefs.SetString("face2", (string)Settings[138]);
					PlayerPrefs.SetString("skin2", (string)Settings[139]);
					PlayerPrefs.SetString("costume2", (string)Settings[140]);
					PlayerPrefs.SetString("logo2", (string)Settings[141]);
					PlayerPrefs.SetString("bladel2", (string)Settings[142]);
					PlayerPrefs.SetString("blader2", (string)Settings[143]);
					PlayerPrefs.SetString("gas2", (string)Settings[144]);
					PlayerPrefs.SetString("hoodie2", (string)Settings[145]);
					PlayerPrefs.SetString("trail2", (string)Settings[146]);
					PlayerPrefs.SetString("horse3", (string)Settings[147]);
					PlayerPrefs.SetString("hair3", (string)Settings[148]);
					PlayerPrefs.SetString("eye3", (string)Settings[149]);
					PlayerPrefs.SetString("glass3", (string)Settings[150]);
					PlayerPrefs.SetString("face3", (string)Settings[151]);
					PlayerPrefs.SetString("skin3", (string)Settings[152]);
					PlayerPrefs.SetString("costume3", (string)Settings[153]);
					PlayerPrefs.SetString("logo3", (string)Settings[154]);
					PlayerPrefs.SetString("bladel3", (string)Settings[155]);
					PlayerPrefs.SetString("blader3", (string)Settings[156]);
					PlayerPrefs.SetString("gas3", (string)Settings[157]);
					PlayerPrefs.SetString("hoodie3", (string)Settings[158]);
					PlayerPrefs.SetString("trail3", (string)Settings[159]);
					PlayerPrefs.SetString("customGround", (string)Settings[162]);
					PlayerPrefs.SetString("forestskyfront", (string)Settings[163]);
					PlayerPrefs.SetString("forestskyback", (string)Settings[164]);
					PlayerPrefs.SetString("forestskyleft", (string)Settings[165]);
					PlayerPrefs.SetString("forestskyright", (string)Settings[166]);
					PlayerPrefs.SetString("forestskyup", (string)Settings[167]);
					PlayerPrefs.SetString("forestskydown", (string)Settings[168]);
					PlayerPrefs.SetString("cityskyfront", (string)Settings[169]);
					PlayerPrefs.SetString("cityskyback", (string)Settings[170]);
					PlayerPrefs.SetString("cityskyleft", (string)Settings[171]);
					PlayerPrefs.SetString("cityskyright", (string)Settings[172]);
					PlayerPrefs.SetString("cityskyup", (string)Settings[173]);
					PlayerPrefs.SetString("cityskydown", (string)Settings[174]);
					PlayerPrefs.SetString("customskyfront", (string)Settings[175]);
					PlayerPrefs.SetString("customskyback", (string)Settings[176]);
					PlayerPrefs.SetString("customskyleft", (string)Settings[177]);
					PlayerPrefs.SetString("customskyright", (string)Settings[178]);
					PlayerPrefs.SetString("customskyup", (string)Settings[179]);
					PlayerPrefs.SetString("customskydown", (string)Settings[180]);
					PlayerPrefs.SetInt("dashenable", (int)Settings[181]);
					PlayerPrefs.SetString("dashkey", (string)Settings[182]);
					PlayerPrefs.SetInt("vsync", (int)Settings[183]);
					PlayerPrefs.SetString("fpscap", (string)Settings[184]);
					PlayerPrefs.SetInt("speedometer", (int)Settings[189]);
					PlayerPrefs.SetInt("bombMode", (int)Settings[192]);
					PlayerPrefs.SetInt("teamMode", (int)Settings[193]);
					PlayerPrefs.SetInt("rockThrow", (int)Settings[194]);
					PlayerPrefs.SetInt("explodeModeOn", (int)Settings[195]);
					PlayerPrefs.SetString("explodeModeNum", (string)Settings[196]);
					PlayerPrefs.SetInt("healthMode", (int)Settings[197]);
					PlayerPrefs.SetString("healthLower", (string)Settings[198]);
					PlayerPrefs.SetString("healthUpper", (string)Settings[199]);
					PlayerPrefs.SetInt("infectionModeOn", (int)Settings[200]);
					PlayerPrefs.SetString("infectionModeNum", (string)Settings[201]);
					PlayerPrefs.SetInt("banEren", (int)Settings[202]);
					PlayerPrefs.SetInt("moreTitanOn", (int)Settings[203]);
					PlayerPrefs.SetString("moreTitanNum", (string)Settings[204]);
					PlayerPrefs.SetInt("damageModeOn", (int)Settings[205]);
					PlayerPrefs.SetString("damageModeNum", (string)Settings[206]);
					PlayerPrefs.SetInt("sizeMode", (int)Settings[207]);
					PlayerPrefs.SetString("sizeLower", (string)Settings[208]);
					PlayerPrefs.SetString("sizeUpper", (string)Settings[209]);
					PlayerPrefs.SetInt("spawnModeOn", (int)Settings[210]);
					PlayerPrefs.SetString("nRate", (string)Settings[211]);
					PlayerPrefs.SetString("aRate", (string)Settings[212]);
					PlayerPrefs.SetString("jRate", (string)Settings[213]);
					PlayerPrefs.SetString("cRate", (string)Settings[214]);
					PlayerPrefs.SetString("pRate", (string)Settings[215]);
					PlayerPrefs.SetInt("horseMode", (int)Settings[216]);
					PlayerPrefs.SetInt("waveModeOn", (int)Settings[217]);
					PlayerPrefs.SetString("waveModeNum", (string)Settings[218]);
					PlayerPrefs.SetInt("friendlyMode", (int)Settings[219]);
					PlayerPrefs.SetInt("pvpMode", (int)Settings[220]);
					PlayerPrefs.SetInt("maxWaveOn", (int)Settings[221]);
					PlayerPrefs.SetString("maxWaveNum", (string)Settings[222]);
					PlayerPrefs.SetInt("endlessModeOn", (int)Settings[223]);
					PlayerPrefs.SetString("endlessModeNum", (string)Settings[224]);
					PlayerPrefs.SetString("motd", (string)Settings[225]);
					PlayerPrefs.SetInt("pointModeOn", (int)Settings[226]);
					PlayerPrefs.SetString("pointModeNum", (string)Settings[227]);
					PlayerPrefs.SetInt("ahssReload", (int)Settings[228]);
					PlayerPrefs.SetInt("punkWaves", (int)Settings[229]);
					PlayerPrefs.SetInt("mapOn", (int)Settings[231]);
					PlayerPrefs.SetString("mapMaximize", (string)Settings[232]);
					PlayerPrefs.SetString("mapToggle", (string)Settings[233]);
					PlayerPrefs.SetString("mapReset", (string)Settings[234]);
					PlayerPrefs.SetInt("globalDisableMinimap", (int)Settings[235]);
					PlayerPrefs.SetString("chatRebind", (string)Settings[236]);
					PlayerPrefs.SetString("hforward", (string)Settings[237]);
					PlayerPrefs.SetString("hback", (string)Settings[238]);
					PlayerPrefs.SetString("hleft", (string)Settings[239]);
					PlayerPrefs.SetString("hright", (string)Settings[240]);
					PlayerPrefs.SetString("hwalk", (string)Settings[241]);
					PlayerPrefs.SetString("hjump", (string)Settings[242]);
					PlayerPrefs.SetString("hmount", (string)Settings[243]);
					PlayerPrefs.SetInt("chatfeed", (int)Settings[244]);
					PlayerPrefs.SetFloat("bombR", (float)Settings[246]);
					PlayerPrefs.SetFloat("bombG", (float)Settings[247]);
					PlayerPrefs.SetFloat("bombB", (float)Settings[248]);
					PlayerPrefs.SetFloat("bombA", (float)Settings[249]);
					PlayerPrefs.SetFloat("bombRadius", (float)Settings[250]);
					PlayerPrefs.SetFloat("bombRange", (float)Settings[251]);
					PlayerPrefs.SetFloat("bombSpeed", (float)Settings[252]);
					PlayerPrefs.SetFloat("bombCD", (float)Settings[253]);
					PlayerPrefs.SetString("cannonUp", (string)Settings[254]);
					PlayerPrefs.SetString("cannonDown", (string)Settings[255]);
					PlayerPrefs.SetString("cannonLeft", (string)Settings[256]);
					PlayerPrefs.SetString("cannonRight", (string)Settings[257]);
					PlayerPrefs.SetString("cannonFire", (string)Settings[258]);
					PlayerPrefs.SetString("cannonMount", (string)Settings[259]);
					PlayerPrefs.SetString("cannonSlow", (string)Settings[260]);
					PlayerPrefs.SetInt("deadlyCannon", (int)Settings[261]);
					PlayerPrefs.SetString("liveCam", (string)Settings[262]);
					Settings[64] = 4;
				}
				else if (GUI.Button(new Rect(num64 + 463f, num65 + 468f, 40f, 25f), "Load"))
				{
					LoadConfig();
					Settings[64] = 5;
				}
				else if (GUI.Button(new Rect(num64 + 508f, num65 + 468f, 60f, 25f), "Default"))
				{
					GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().setToDefault();
				}
				else if (GUI.Button(new Rect(num64 + 573f, num65 + 468f, 75f, 25f), "Continue"))
				{
					if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
					{
						Time.timeScale = 1f;
					}
					if (!mainCamera.enabled)
					{
						Screen.showCursor = true;
						Screen.lockCursor = true;
						GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = false;
						Camera.main.GetComponent<SpectatorMovement>().disable = false;
						Camera.main.GetComponent<MouseLook>().disable = false;
						return;
					}
					IN_GAME_MAIN_CAMERA.IsPausing = false;
					if (IN_GAME_MAIN_CAMERA.CameraMode == CameraType.TPS)
					{
						Screen.showCursor = false;
						Screen.lockCursor = true;
					}
					else
					{
						Screen.showCursor = false;
						Screen.lockCursor = false;
					}
					GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = false;
					GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().justUPDATEME();
				}
				else if (GUI.Button(new Rect(num64 + 653f, num65 + 468f, 40f, 25f), "Quit"))
				{
					if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
					{
						Time.timeScale = 1f;
					}
					else
					{
						PhotonNetwork.Disconnect();
					}
					Screen.lockCursor = false;
					Screen.showCursor = true;
					IN_GAME_MAIN_CAMERA.Gametype = GameType.Stop;
					gameStart = false;
					GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = false;
					DestroyAllExistingCloths();
					UnityEngine.Object.Destroy(GameObject.Find("MultiplayerManager"));
					Application.LoadLevel("menu");
				}
			}
			else
			{
				if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer)
				{
					return;
				}
				float num118 = (float)Screen.width / 2f;
				float num119 = (float)Screen.height / 2f;
				if (Time.timeScale <= 0.1f)
				{
					GUI.Box(new Rect(num118 - 100f, num119 - 50f, 200f, 100f), string.Empty);
					if (pauseWaitTime > 3f)
					{
						GUI.Label(new Rect(num118 - 43f, num119 - 10f, 200f, 22f), "Game Paused.");
						return;
					}
					GUI.Label(new Rect(num118 - 43f, num119 - 15f, 200f, 22f), "Unpausing in:");
					GUI.Label(new Rect(num118 - 8f, num119 + 5f, 200f, 22f), pauseWaitTime.ToString("F1"));
				}
				else if (!LogicLoaded || !CustomLevelLoaded)
				{
					GUI.Box(new Rect(num118 - 100f, num119 - 50f, 200f, 150f), string.Empty);
					int length = GExtensions.AsString(PhotonNetwork.player.customProperties[PhotonPlayerProperty.CurrentLevel]).Length;
					int length2 = GExtensions.AsString(PhotonNetwork.masterClient.customProperties[PhotonPlayerProperty.CurrentLevel]).Length;
					GUI.Label(new Rect(num118 - 60f, num119 - 30f, 200f, 22f), "Loading Level (" + length + "/" + length2 + ")");
					retryTime += Time.deltaTime;
					Screen.lockCursor = false;
					Screen.showCursor = true;
					if (GUI.Button(new Rect(num118 - 20f, num119 + 50f, 40f, 30f), "Quit"))
					{
						PhotonNetwork.Disconnect();
						Screen.lockCursor = false;
						Screen.showCursor = true;
						IN_GAME_MAIN_CAMERA.Gametype = GameType.Stop;
						gameStart = false;
						GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>().menuOn = false;
						DestroyAllExistingCloths();
						UnityEngine.Object.Destroy(GameObject.Find("MultiplayerManager"));
						Application.LoadLevel("menu");
					}
				}
			}
		}
	}

	public void SpawnTitanCustom(string place, int abnormal, int rate, bool punk)
	{
		int num = rate;
		if (Level.Name.StartsWith("Custom"))
		{
			num = 5;
			if (RCSettings.GameType == 1)
			{
				num = 3;
			}
			else if (RCSettings.GameType == 2 || RCSettings.GameType == 3)
			{
				num = 0;
			}
		}
		if (RCSettings.MoreTitans > 0 || (RCSettings.MoreTitans == 0 && Level.Name.StartsWith("Custom") && RCSettings.GameType >= 2))
		{
			num = RCSettings.MoreTitans;
		}
		if (IN_GAME_MAIN_CAMERA.Gamemode == GameMode.Survival)
		{
			if (punk)
			{
				num = rate;
			}
			else if (RCSettings.MoreTitans == 0)
			{
				int num2 = 1;
				if (RCSettings.WaveModeOn == 1)
				{
					num2 = RCSettings.WaveModeNum;
				}
				num += (wave - 1) * (num2 - 1);
			}
			else if (RCSettings.MoreTitans > 0)
			{
				int num3 = 1;
				if (RCSettings.WaveModeOn == 1)
				{
					num3 = RCSettings.WaveModeNum;
				}
				num += (wave - 1) * num3;
			}
		}
		num = Math.Min(50, num);
		if (RCSettings.SpawnMode == 1)
		{
			float num4 = RCSettings.NormalRate;
			float num5 = RCSettings.AberrantRate;
			float num6 = RCSettings.JumperRate;
			float num7 = RCSettings.CrawlerRate;
			float num8 = RCSettings.PunkRate;
			if (punk && RCSettings.PunkWaves == 1)
			{
				num4 = 0f;
				num5 = 0f;
				num6 = 0f;
				num7 = 0f;
				num8 = 100f;
				num = rate;
			}
			for (int i = 0; i < num; i++)
			{
				Vector3 position = new Vector3(UnityEngine.Random.Range(-400f, 400f), 0f, UnityEngine.Random.Range(-400f, 400f));
				Quaternion rotation = new Quaternion(0f, 0f, 0f, 1f);
				if (titanSpawns.Count > 0)
				{
					position = titanSpawns[UnityEngine.Random.Range(0, titanSpawns.Count)];
				}
				else
				{
					GameObject[] array = GameObject.FindGameObjectsWithTag(place);
					if (array.Length != 0)
					{
						int num9 = UnityEngine.Random.Range(0, array.Length);
						GameObject gameObject = array[num9];
						while (array[num9] == null)
						{
							num9 = UnityEngine.Random.Range(0, array.Length);
							gameObject = array[num9];
						}
						array[num9] = null;
						position = gameObject.transform.position;
						rotation = gameObject.transform.rotation;
					}
				}
				float num10 = UnityEngine.Random.Range(0f, 100f);
				if (num10 <= num4 + num5 + num6 + num7 + num8)
				{
					GameObject gameObject2 = SpawnTitanRaw(position, rotation);
					if (num10 < num4)
					{
						gameObject2.GetComponent<TITAN>().setAbnormalType2(TitanClass.Normal, forceCrawler: false);
					}
					else if (num10 >= num4 && num10 < num4 + num5)
					{
						gameObject2.GetComponent<TITAN>().setAbnormalType2(TitanClass.Aberrant, forceCrawler: false);
					}
					else if (num10 >= num4 + num5 && num10 < num4 + num5 + num6)
					{
						gameObject2.GetComponent<TITAN>().setAbnormalType2(TitanClass.Jumper, forceCrawler: false);
					}
					else if (num10 >= num4 + num5 + num6 && num10 < num4 + num5 + num6 + num7)
					{
						gameObject2.GetComponent<TITAN>().setAbnormalType2(TitanClass.Crawler, forceCrawler: true);
					}
					else if (num10 >= num4 + num5 + num6 + num7 && num10 < num4 + num5 + num6 + num7 + num8)
					{
						gameObject2.GetComponent<TITAN>().setAbnormalType2(TitanClass.Punk, forceCrawler: false);
					}
					else
					{
						gameObject2.GetComponent<TITAN>().setAbnormalType2(TitanClass.Normal, forceCrawler: false);
					}
				}
				else
				{
					SpawnTitan(abnormal, position, rotation, punk);
				}
			}
		}
		else if (Level.Name.StartsWith("Custom"))
		{
			for (int j = 0; j < num; j++)
			{
				Vector3 position2 = new Vector3(UnityEngine.Random.Range(-400f, 400f), 0f, UnityEngine.Random.Range(-400f, 400f));
				Quaternion rotation2 = new Quaternion(0f, 0f, 0f, 1f);
				if (titanSpawns.Count > 0)
				{
					position2 = titanSpawns[UnityEngine.Random.Range(0, titanSpawns.Count)];
				}
				else
				{
					GameObject[] array2 = GameObject.FindGameObjectsWithTag(place);
					if (array2.Length != 0)
					{
						int num11 = UnityEngine.Random.Range(0, array2.Length);
						GameObject gameObject3 = array2[num11];
						while (array2[num11] == null)
						{
							num11 = UnityEngine.Random.Range(0, array2.Length);
							gameObject3 = array2[num11];
						}
						array2[num11] = null;
						position2 = gameObject3.transform.position;
						rotation2 = gameObject3.transform.rotation;
					}
				}
				SpawnTitan(abnormal, position2, rotation2, punk);
			}
		}
		else
		{
			for (int k = 0; k < num; k++)
			{
				SpawnTitanRandom("titanRespawn", abnormal, punk);
			}
		}
	}

	public void SpawnTitanAction(int type, float size, int health, int number)
	{
		Vector3 position = new Vector3(UnityEngine.Random.Range(-400f, 400f), 0f, UnityEngine.Random.Range(-400f, 400f));
		Quaternion rotation = new Quaternion(0f, 0f, 0f, 1f);
		if (titanSpawns.Count > 0)
		{
			position = titanSpawns[UnityEngine.Random.Range(0, titanSpawns.Count)];
		}
		else
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("titanRespawn");
			if (array.Length != 0)
			{
				int num = UnityEngine.Random.Range(0, array.Length);
				GameObject gameObject = array[num];
				while (array[num] == null)
				{
					num = UnityEngine.Random.Range(0, array.Length);
					gameObject = array[num];
				}
				array[num] = null;
				position = gameObject.transform.position;
				rotation = gameObject.transform.rotation;
			}
		}
		for (int i = 0; i < number; i++)
		{
			GameObject gameObject2 = SpawnTitanRaw(position, rotation);
			gameObject2.GetComponent<TITAN>().SetLevel(size);
			gameObject2.GetComponent<TITAN>().hasSetLevel = true;
			if ((float)health > 0f)
			{
				gameObject2.GetComponent<TITAN>().currentHealth = health;
				gameObject2.GetComponent<TITAN>().maxHealth = health;
			}
			switch (type)
			{
			case 0:
				gameObject2.GetComponent<TITAN>().setAbnormalType2(TitanClass.Normal, forceCrawler: false);
				break;
			case 1:
				gameObject2.GetComponent<TITAN>().setAbnormalType2(TitanClass.Aberrant, forceCrawler: false);
				break;
			case 2:
				gameObject2.GetComponent<TITAN>().setAbnormalType2(TitanClass.Jumper, forceCrawler: false);
				break;
			case 3:
				gameObject2.GetComponent<TITAN>().setAbnormalType2(TitanClass.Crawler, forceCrawler: true);
				break;
			case 4:
				gameObject2.GetComponent<TITAN>().setAbnormalType2(TitanClass.Punk, forceCrawler: false);
				break;
			}
		}
	}

	public void SpawnTitanAtAction(int type, float size, int health, int number, float posX, float posY, float posZ)
	{
		Vector3 position = new Vector3(posX, posY, posZ);
		Quaternion rotation = new Quaternion(0f, 0f, 0f, 1f);
		for (int i = 0; i < number; i++)
		{
			GameObject gameObject = SpawnTitanRaw(position, rotation);
			gameObject.GetComponent<TITAN>().SetLevel(size);
			gameObject.GetComponent<TITAN>().hasSetLevel = true;
			if ((float)health > 0f)
			{
				gameObject.GetComponent<TITAN>().currentHealth = health;
				gameObject.GetComponent<TITAN>().maxHealth = health;
			}
			switch (type)
			{
			case 0:
				gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Normal, forceCrawler: false);
				break;
			case 1:
				gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Aberrant, forceCrawler: false);
				break;
			case 2:
				gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Jumper, forceCrawler: false);
				break;
			case 3:
				gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Crawler, forceCrawler: true);
				break;
			case 4:
				gameObject.GetComponent<TITAN>().setAbnormalType2(TitanClass.Punk, forceCrawler: false);
				break;
			}
		}
	}

	[RPC]
	private void spawnTitanRPC(PhotonMessageInfo info)
	{
		if (!info.sender.isMasterClient)
		{
			return;
		}
		foreach (TITAN titan in titans)
		{
			if (titan.photonView.isMine && (!PhotonNetwork.isMasterClient || titan.nonAI))
			{
				PhotonNetwork.Destroy(titan.gameObject);
			}
		}
		SpawnNonAITitan2(myLastHero);
	}

	[RPC]
	private void setTeamRPC(int newTeam, PhotonMessageInfo info)
	{
		if (info.sender.isMasterClient || info.sender.isLocal)
		{
			SetTeam(newTeam);
		}
	}

	private void SetTeam(int newTeam)
	{
		switch (newTeam)
		{
		case 0:
		{
			ExitGames.Client.Photon.Hashtable hashtable3 = new ExitGames.Client.Photon.Hashtable();
			hashtable3.Add(PhotonPlayerProperty.RCTeam, 0);
			hashtable3.Add(PhotonPlayerProperty.Name, LoginFengKAI.Player.Name);
			PhotonNetwork.player.SetCustomProperties(hashtable3);
			break;
		}
		case 1:
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add(PhotonPlayerProperty.RCTeam, 1);
			hashtable.Add(PhotonPlayerProperty.Name, "[00FFFF]" + LoginFengKAI.Player.Name.StripNGUI());
			PhotonNetwork.player.SetCustomProperties(hashtable);
			break;
		}
		case 2:
		{
			ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable();
			hashtable2.Add(PhotonPlayerProperty.RCTeam, 2);
			hashtable2.Add(PhotonPlayerProperty.Name, "[FF00FF]" + LoginFengKAI.Player.Name.StripNGUI());
			PhotonNetwork.player.SetCustomProperties(hashtable2);
			break;
		}
		case 3:
		{
			int num = 0;
			int num2 = 0;
			int team = 1;
			PhotonPlayer[] array = PhotonNetwork.playerList;
			for (int i = 0; i < array.Length; i++)
			{
				switch (GExtensions.AsInt(array[i].customProperties[PhotonPlayerProperty.RCTeam]))
				{
				case 1:
					num++;
					break;
				case 2:
					num2++;
					break;
				}
			}
			if (num > num2)
			{
				team = 2;
			}
			SetTeam(team);
			break;
		}
		}
		HERO[] array2 = UnityEngine.Object.FindObjectsOfType<HERO>();
		foreach (HERO hERO in array2)
		{
			if (hERO.photonView.isMine)
			{
				base.photonView.RPC("labelRPC", PhotonTargets.All, hERO.photonView.viewID);
			}
		}
	}

	[RPC]
	private void settingRPC(ExitGames.Client.Photon.Hashtable settings, PhotonMessageInfo info)
	{
		if (info.sender.isMasterClient)
		{
			SetGameSettings(settings);
		}
	}

	private void SetGameSettings(ExitGames.Client.Photon.Hashtable settings)
	{
		restartingEren = false;
		restartingBomb = false;
		restartingHorse = false;
		restartingTitan = false;
		if (settings.ContainsKey("bomb"))
		{
			if (RCSettings.BombMode != (int)settings["bomb"])
			{
				RCSettings.BombMode = (int)settings["bomb"];
				InRoomChat.Instance.AddLine("PVP Bomb Mode enabled.".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.BombMode != 0)
		{
			RCSettings.BombMode = 0;
			InRoomChat.Instance.AddLine("PVP Bomb Mode disabled.".AsColor("FFCC00"));
			if (PhotonNetwork.isMasterClient)
			{
				restartingBomb = true;
			}
		}
		if (settings.ContainsKey("globalDisableMinimap"))
		{
			if (RCSettings.GlobalDisableMinimap != (int)settings["globalDisableMinimap"])
			{
				RCSettings.GlobalDisableMinimap = (int)settings["globalDisableMinimap"];
				InRoomChat.Instance.AddLine("Minimaps are not allowed.".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.GlobalDisableMinimap != 0)
		{
			RCSettings.GlobalDisableMinimap = 0;
			InRoomChat.Instance.AddLine("Minimaps are allowed.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("horse"))
		{
			if (RCSettings.HorseMode != (int)settings["horse"])
			{
				RCSettings.HorseMode = (int)settings["horse"];
				InRoomChat.Instance.AddLine("Horses enabled.".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.HorseMode != 0)
		{
			RCSettings.HorseMode = 0;
			InRoomChat.Instance.AddLine("Horses disabled.".AsColor("FFCC00"));
			if (PhotonNetwork.isMasterClient)
			{
				restartingHorse = true;
			}
		}
		if (settings.ContainsKey("punkWaves"))
		{
			if (RCSettings.PunkWaves != (int)settings["punkWaves"])
			{
				RCSettings.PunkWaves = (int)settings["punkWaves"];
				InRoomChat.Instance.AddLine("Punk override every 5 waves enabled.".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.PunkWaves != 0)
		{
			RCSettings.PunkWaves = 0;
			InRoomChat.Instance.AddLine("Punk override every 5 waves disabled.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("ahssReload"))
		{
			if (RCSettings.AhssReload != (int)settings["ahssReload"])
			{
				RCSettings.AhssReload = (int)settings["ahssReload"];
				InRoomChat.Instance.AddLine("AHSS Air-Reload is not allowed.".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.AhssReload != 0)
		{
			RCSettings.AhssReload = 0;
			InRoomChat.Instance.AddLine("AHSS Air-Reload is allowed.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("team"))
		{
			if (RCSettings.TeamMode != (int)settings["team"])
			{
				RCSettings.TeamMode = (int)settings["team"];
				string text = string.Empty;
				switch (RCSettings.TeamMode)
				{
				case 1:
					text = "No sort";
					break;
				case 2:
					text = "Locked by Size";
					break;
				case 3:
					text = "Locked by Skill";
					break;
				}
				InRoomChat.Instance.AddLine("Team Mode enabled</color> (".AsColor("FFCC00") + text + ").".AsColor("FFCC00"));
				if (GExtensions.AsInt(PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCTeam]) == 0)
				{
					SetTeam(3);
				}
			}
		}
		else if (RCSettings.TeamMode != 0)
		{
			RCSettings.TeamMode = 0;
			SetTeam(0);
			InRoomChat.Instance.AddLine("Team Mode disabled.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("point"))
		{
			if (RCSettings.PointMode != (int)settings["point"])
			{
				RCSettings.PointMode = (int)settings["point"];
				InRoomChat.Instance.AddLine("Point Limit enabled (".AsColor("FFCC00") + RCSettings.PointMode + ").".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.PointMode != 0)
		{
			RCSettings.PointMode = 0;
			InRoomChat.Instance.AddLine("Point limit disabled.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("rock"))
		{
			if (RCSettings.DisableRock != (int)settings["rock"])
			{
				RCSettings.DisableRock = (int)settings["rock"];
				InRoomChat.Instance.AddLine("Punk rock throwing disabled.".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.DisableRock != 0)
		{
			RCSettings.DisableRock = 0;
			InRoomChat.Instance.AddLine("Punk rock throwing enabled.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("explode"))
		{
			if (RCSettings.ExplodeMode != (int)settings["explode"])
			{
				RCSettings.ExplodeMode = (int)settings["explode"];
				InRoomChat.Instance.AddLine("Titan Explode Mode enabled (Radius ".AsColor("FFCC00") + RCSettings.ExplodeMode + ").".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.ExplodeMode != 0)
		{
			RCSettings.ExplodeMode = 0;
			InRoomChat.Instance.AddLine("Titan Explode Mode disabled.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("healthMode") && settings.ContainsKey("healthLower") && settings.ContainsKey("healthUpper"))
		{
			if (RCSettings.HealthMode != (int)settings["healthMode"] || RCSettings.HealthLower != (int)settings["healthLower"] || RCSettings.HealthUpper != (int)settings["healthUpper"])
			{
				RCSettings.HealthMode = (int)settings["healthMode"];
				RCSettings.HealthLower = (int)settings["healthLower"];
				RCSettings.HealthUpper = (int)settings["healthUpper"];
				string text2 = "Static";
				if (RCSettings.HealthMode == 2)
				{
					text2 = "Scaled";
				}
				InRoomChat.Instance.AddLine("Titan Health (".AsColor("FFCC00") + text2 + ", ".AsColor("FFCC00") + RCSettings.HealthLower + " to ".AsColor("FFCC00") + RCSettings.HealthUpper + ") enabled.".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.HealthMode != 0 || RCSettings.HealthLower != 0 || RCSettings.HealthUpper != 0)
		{
			RCSettings.HealthMode = 0;
			RCSettings.HealthLower = 0;
			RCSettings.HealthUpper = 0;
			InRoomChat.Instance.AddLine("Titan Health disabled.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("infection"))
		{
			if (RCSettings.InfectionMode != (int)settings["infection"])
			{
				RCSettings.InfectionMode = (int)settings["infection"];
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable.Add(PhotonPlayerProperty.RCTeam, 0);
				PhotonNetwork.player.SetCustomProperties(hashtable);
				InRoomChat.Instance.AddLine("Infection mode (".AsColor("FFCC00") + RCSettings.InfectionMode + ") enabled. Make sure your first character is human.".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.InfectionMode != 0)
		{
			RCSettings.InfectionMode = 0;
			ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable();
			hashtable2.Add(PhotonPlayerProperty.IsTitan, 1);
			PhotonNetwork.player.SetCustomProperties(hashtable2);
			InRoomChat.Instance.AddLine("Infection Mode disabled.".AsColor("FFCC00"));
			if (PhotonNetwork.isMasterClient)
			{
				restartingTitan = true;
			}
		}
		if (settings.ContainsKey("eren"))
		{
			if (RCSettings.BanEren != (int)settings["eren"])
			{
				RCSettings.BanEren = (int)settings["eren"];
				InRoomChat.Instance.AddLine("Anti-Eren enabled. Using Titan Eren will get you kicked.".AsColor("FFCC00"));
				if (PhotonNetwork.isMasterClient)
				{
					restartingEren = true;
				}
			}
		}
		else if (RCSettings.BanEren != 0)
		{
			RCSettings.BanEren = 0;
			InRoomChat.Instance.AddLine("Anti-Eren disabled. Titan Eren is allowed.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("titanc"))
		{
			if (RCSettings.MoreTitans != (int)settings["titanc"])
			{
				RCSettings.MoreTitans = (int)settings["titanc"];
				InRoomChat.Instance.AddLine(RCSettings.MoreTitans + " Titans will spawn each round.".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.MoreTitans != 0)
		{
			RCSettings.MoreTitans = 0;
			InRoomChat.Instance.AddLine("Default titan amount will spawn each round.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("damage"))
		{
			if (RCSettings.MinimumDamage != (int)settings["damage"])
			{
				RCSettings.MinimumDamage = (int)settings["damage"];
				InRoomChat.Instance.AddLine("Minimum nape damage (".AsColor("FFCC00") + RCSettings.MinimumDamage + ") enabled.".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.MinimumDamage != 0)
		{
			RCSettings.MinimumDamage = 0;
			InRoomChat.Instance.AddLine("Minimum nape damage disabled.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("sizeMode") && settings.ContainsKey("sizeLower") && settings.ContainsKey("sizeUpper"))
		{
			if (settings["sizeMode"] is bool flag)
			{
				settings["sizeMode"] = (flag ? 1 : 0);
				GuardianClient.Logger.Debug("RC2020 'sizeMode' as <b>bool</b> detected, replacing with <b>int</b> equivalent.");
			}
			if (RCSettings.SizeMode != (int)settings["sizeMode"] || RCSettings.SizeLower != (float)settings["sizeLower"] || RCSettings.SizeUpper != (float)settings["sizeUpper"])
			{
				RCSettings.SizeMode = (int)settings["sizeMode"];
				RCSettings.SizeLower = (float)settings["sizeLower"];
				RCSettings.SizeUpper = (float)settings["sizeUpper"];
				InRoomChat.Instance.AddLine("Custom titan size (".AsColor("FFCC00") + RCSettings.SizeLower.ToString("F2") + ", ".AsColor("FFCC00") + RCSettings.SizeUpper.ToString("F2") + ") enabled.".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.SizeMode != 0 || RCSettings.SizeLower != 0f || RCSettings.SizeUpper != 0f)
		{
			RCSettings.SizeMode = 0;
			RCSettings.SizeLower = 0f;
			RCSettings.SizeUpper = 0f;
			InRoomChat.Instance.AddLine("Custom titan size disabled.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("spawnMode") && settings.ContainsKey("nRate") && settings.ContainsKey("aRate") && settings.ContainsKey("jRate") && settings.ContainsKey("cRate") && settings.ContainsKey("pRate"))
		{
			if (RCSettings.SpawnMode != (int)settings["spawnMode"] || RCSettings.NormalRate != (float)settings["nRate"] || RCSettings.AberrantRate != (float)settings["aRate"] || RCSettings.JumperRate != (float)settings["jRate"] || RCSettings.CrawlerRate != (float)settings["cRate"] || RCSettings.PunkRate != (float)settings["pRate"])
			{
				RCSettings.SpawnMode = (int)settings["spawnMode"];
				RCSettings.NormalRate = (float)settings["nRate"];
				RCSettings.AberrantRate = (float)settings["aRate"];
				RCSettings.JumperRate = (float)settings["jRate"];
				RCSettings.CrawlerRate = (float)settings["cRate"];
				RCSettings.PunkRate = (float)settings["pRate"];
				InRoomChat.Instance.AddLine("Custom spawn rate enabled (".AsColor("FFCC00") + RCSettings.NormalRate.ToString("F2") + "% Normal, ".AsColor("FFCC00") + RCSettings.AberrantRate.ToString("F2") + "% Abnormal, ".AsColor("FFCC00") + RCSettings.JumperRate.ToString("F2") + "% Jumper, ".AsColor("FFCC00") + RCSettings.CrawlerRate.ToString("F2") + "% Crawler, ".AsColor("FFCC00") + RCSettings.PunkRate.ToString("F2") + "% Punk)".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.SpawnMode != 0 || RCSettings.NormalRate != 0f || RCSettings.AberrantRate != 0f || RCSettings.JumperRate != 0f || RCSettings.CrawlerRate != 0f || RCSettings.PunkRate != 0f)
		{
			RCSettings.SpawnMode = 0;
			RCSettings.NormalRate = 0f;
			RCSettings.AberrantRate = 0f;
			RCSettings.JumperRate = 0f;
			RCSettings.CrawlerRate = 0f;
			RCSettings.PunkRate = 0f;
			InRoomChat.Instance.AddLine("Custom spawn rate disabled.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("waveModeOn") && settings.ContainsKey("waveModeNum"))
		{
			if (RCSettings.WaveModeOn != (int)settings["waveModeOn"] || RCSettings.WaveModeNum != (int)settings["waveModeNum"])
			{
				RCSettings.WaveModeOn = (int)settings["waveModeOn"];
				RCSettings.WaveModeNum = (int)settings["waveModeNum"];
				InRoomChat.Instance.AddLine("Custom Wave Mode (".AsColor("FFCC00") + RCSettings.WaveModeNum + ") enabled.".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.WaveModeOn != 0 || RCSettings.WaveModeNum != 0)
		{
			RCSettings.WaveModeOn = 0;
			RCSettings.WaveModeNum = 0;
			InRoomChat.Instance.AddLine("Custom Wave Mode disabled.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("friendly"))
		{
			if (RCSettings.FriendlyMode != (int)settings["friendly"])
			{
				RCSettings.FriendlyMode = (int)settings["friendly"];
				InRoomChat.Instance.AddLine("Friendly Fire disabled, PVP is not allowed.".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.FriendlyMode != 0)
		{
			RCSettings.FriendlyMode = 0;
			InRoomChat.Instance.AddLine("Friendly Fire enabled, PVP is allowed.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("pvp"))
		{
			if (RCSettings.PvPMode != (int)settings["pvp"])
			{
				RCSettings.PvPMode = (int)settings["pvp"];
				string text3 = string.Empty;
				if (RCSettings.PvPMode == 1)
				{
					text3 = "Team-Based";
				}
				else if (RCSettings.PvPMode == 2)
				{
					text3 = "FFA";
				}
				InRoomChat.Instance.AddLine("Blade/AHSS PVP enabled (".AsColor("FFCC00") + text3 + ").".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.PvPMode != 0)
		{
			RCSettings.PvPMode = 0;
			InRoomChat.Instance.AddLine("Blade/AHSS PVP disabled.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("maxwave"))
		{
			if (RCSettings.MaxWave != (int)settings["maxwave"])
			{
				RCSettings.MaxWave = (int)settings["maxwave"];
				InRoomChat.Instance.AddLine("Max Wave is ".AsColor("FFCC00") + RCSettings.MaxWave + ".".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.MaxWave != 0)
		{
			RCSettings.MaxWave = 0;
			InRoomChat.Instance.AddLine("Max Wave set to default (20)".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("endless"))
		{
			if (RCSettings.EndlessMode != (int)settings["endless"])
			{
				RCSettings.EndlessMode = (int)settings["endless"];
				InRoomChat.Instance.AddLine("Endless Respawn enabled (".AsColor("FFCC00") + RCSettings.EndlessMode + "s).".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.EndlessMode != 0)
		{
			RCSettings.EndlessMode = 0;
			InRoomChat.Instance.AddLine("Endless Respawn disabled.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("deadlycannons"))
		{
			if (RCSettings.DeadlyCannons != (int)settings["deadlycannons"])
			{
				RCSettings.DeadlyCannons = (int)settings["deadlycannons"];
				InRoomChat.Instance.AddLine("Cannons will now kill humans.".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.DeadlyCannons != 0)
		{
			RCSettings.DeadlyCannons = 0;
			InRoomChat.Instance.AddLine("Cannons will no longer kill humans.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("asoracing"))
		{
			if (RCSettings.RacingStatic != (int)settings["asoracing"])
			{
				RCSettings.RacingStatic = (int)settings["asoracing"];
				InRoomChat.Instance.AddLine("Racing will not restart on win.".AsColor("FFCC00"));
			}
		}
		else if (RCSettings.RacingStatic != 0)
		{
			RCSettings.RacingStatic = 0;
			InRoomChat.Instance.AddLine("Racing will restart on win.".AsColor("FFCC00"));
		}
		if (settings.ContainsKey("motd"))
		{
			if (RCSettings.Motd != (string)settings["motd"])
			{
				RCSettings.Motd = (string)settings["motd"];
				InRoomChat.Instance.AddLine("MOTD: ".AsColor("FFCC00") + RCSettings.Motd);
			}
		}
		else if (RCSettings.Motd.Length > 0)
		{
			RCSettings.Motd = string.Empty;
		}
	}

	[RPC]
	private void labelRPC(int viewId, PhotonMessageInfo info)
	{
		if ((info.timeInt - 1000000) * -1 == info.sender.Id)
		{
			info.sender.IsAnarchyMod = true;
		}
		PhotonView photonView = PhotonView.Find(viewId);
		if (photonView == null || photonView.owner != info.sender || photonView.gameObject == null)
		{
			return;
		}
		PhotonPlayer owner = photonView.owner;
		string text = GExtensions.AsString(owner.customProperties[PhotonPlayerProperty.Guild]);
		string text2 = GExtensions.AsString(owner.customProperties[PhotonPlayerProperty.Name]);
		HERO component = photonView.gameObject.GetComponent<HERO>();
		if (component != null)
		{
			UILabel component2 = component.myNetWorkName.GetComponent<UILabel>();
			component2.text = text2;
			if (text.Length > 0)
			{
				component2.text = "[FFFF00]" + text + "\n[FFFFFF]" + component2.text;
			}
		}
	}

	[RPC]
	private void setMasterRC(PhotonMessageInfo info)
	{
		if (info.sender.isMasterClient)
		{
			MasterRC = true;
		}
	}

	private string HairType(int type)
	{
		if (type < 0)
		{
			return "Random";
		}
		return "Male " + type;
	}

	private string MasterTextureType(int type)
	{
		switch (type)
		{
		case 0:
			return "Highest";
		case 1:
			return "Medium";
		case 2:
			return "Low";
		case 3:
			return "Lower";
		case 4:
			return "Lowest";
		case 5:
			return "Ultra-Low";
		case 6:
			return "Ultra-Lower";
		case 7:
			return "Ultra-Lowest";
		case 8:
			return "NVIDIA GT 520";
		default:
			return type.ToString();
		}
	}

	public void RestartRC()
	{
		IntVariables.Clear();
		BoolVariables.Clear();
		StringVariables.Clear();
		FloatVariables.Clear();
		PlayerVariables.Clear();
		TitanVariables.Clear();
		if (RCSettings.InfectionMode > 0)
		{
			EndGameInfection();
		}
		else
		{
			EndGameRC();
		}
	}
}
