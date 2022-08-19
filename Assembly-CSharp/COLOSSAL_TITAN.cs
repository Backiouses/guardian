using System.Collections;
using Guardian.AntiAbuse.Validators;
using Photon;
using UnityEngine;

public class COLOSSAL_TITAN : Photon.MonoBehaviour
{
	public static float minusDistance = 99999f;

	public static GameObject minusDistanceEnemy;

	public bool hasDie;

	public GameObject myHero;

	private string state = "idle";

	public float myDistance;

	private int attackCount;

	public GameObject bottomObject;

	public int NapeArmor = 10000;

	public int NapeArmorTotal = 10000;

	private string actionName;

	private Transform checkHitCapsuleStart;

	private Transform checkHitCapsuleEnd;

	private Vector3 checkHitCapsuleEndOld;

	private float checkHitCapsuleR;

	private float attackCheckTime;

	private float attackCheckTimeA;

	private float attackCheckTimeB;

	private bool attackChkOnce;

	private string attackAnimation;

	private int attackPattern = -1;

	public GameObject door_broken;

	public GameObject door_closed;

	public GameObject neckSteamObject;

	public GameObject sweepSmokeObject;

	private float waitTime = 2f;

	private bool isSteamNeed;

	public GameObject healthLabel;

	public float lagMax;

	public float healthTime;

	public int maxHealth;

	public float size;

	public bool hasspawn;

	private void OnDestroy()
	{
		if (GameObject.Find("MultiplayerManager") != null)
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().RemoveColossal(this);
		}
	}

	private void StartMain()
	{
		FengGameManagerMKII.Instance.AddColossal(this);
		if (myHero == null)
		{
			FindNearestHero();
		}
		base.name = "COLOSSAL_TITAN";
		NapeArmor = 1000;
		bool flag = false;
		if (FengGameManagerMKII.Level.RespawnMode == RespawnMode.Never)
		{
			flag = true;
		}
		if (IN_GAME_MAIN_CAMERA.Difficulty == 0)
		{
			NapeArmor = ((!flag) ? 5000 : 2000);
		}
		else if (IN_GAME_MAIN_CAMERA.Difficulty == 1)
		{
			NapeArmor = ((!flag) ? 8000 : 3500);
			foreach (AnimationState item in base.animation)
			{
				item.speed = 1.02f;
			}
		}
		else if (IN_GAME_MAIN_CAMERA.Difficulty == 2)
		{
			NapeArmor = ((!flag) ? 12000 : 5000);
			foreach (AnimationState item2 in base.animation)
			{
				item2.speed = 1.05f;
			}
		}
		NapeArmorTotal = NapeArmor;
		state = "wait";
		base.transform.position += -Vector3.up * 10000f;
		door_broken = GameObject.Find("door_broke");
		door_closed = GameObject.Find("door_fine");
		door_broken.SetActive(value: false);
		door_closed.SetActive(value: true);
	}

	private RaycastHit[] checkHitCapsule(Vector3 start, Vector3 end, float r)
	{
		return Physics.SphereCastAll(start, r, end - start, Vector3.Distance(start, end));
	}

	private void Awake()
	{
		base.rigidbody.freezeRotation = true;
		base.rigidbody.useGravity = false;
		base.rigidbody.isKinematic = true;
	}

	private void PlayAnimation(string aniName)
	{
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("netPlayAnimation", PhotonTargets.Others, aniName);
		}
		LocalPlayAnimation(aniName);
	}

	private void PlayAnimationAt(string aniName, float normalizedTime)
	{
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("netPlayAnimationAt", PhotonTargets.Others, aniName, normalizedTime);
		}
		LocalPlayAnimationAt(aniName, normalizedTime);
	}

	private void CrossFade(string aniName, float time)
	{
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("netCrossFade", PhotonTargets.Others, aniName, time);
		}
		LocalCrossFade(aniName, time);
	}

	private void LocalPlayAnimation(string aniName)
	{
		base.animation.Play(aniName);
	}

	private void LocalPlayAnimationAt(string aniName, float normalizedTime)
	{
		base.animation.Play(aniName);
		base.animation[aniName].normalizedTime = normalizedTime;
	}

	private void LocalCrossFade(string aniName, float time)
	{
		base.animation.CrossFade(aniName, time);
	}

	[RPC]
	private void netPlayAnimation(string aniName, PhotonMessageInfo info)
	{
		if (ColossalChecker.IsAnimationPlayValid(this, info))
		{
			LocalPlayAnimation(aniName);
		}
	}

	[RPC]
	private void netPlayAnimationAt(string aniName, float normalizedTime, PhotonMessageInfo info)
	{
		if (ColossalChecker.IsAnimationSeekedPlayValid(this, info))
		{
			LocalPlayAnimationAt(aniName, normalizedTime);
		}
	}

	[RPC]
	private void netCrossFade(string aniName, float time, PhotonMessageInfo info)
	{
		if (ColossalChecker.IsCrossFadeValid(this, info))
		{
			LocalCrossFade(aniName, time);
		}
	}

	private void FindNearestHero()
	{
		myHero = GetNearestHero();
	}

	private GameObject GetNearestHero()
	{
		GameObject result = null;
		float num = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		foreach (GameObject player in FengGameManagerMKII.Instance.Players)
		{
			if ((!player.GetComponent<HERO>() || !player.GetComponent<HERO>().HasDied()) && (!player.GetComponent<TITAN_EREN>() || !player.GetComponent<TITAN_EREN>().hasDied))
			{
				Vector3 position2 = player.transform.position;
				float num2 = position2.x - position.x;
				float num3 = position2.z - position.z;
				float num4 = num2 * num2 + num3 * num3;
				if (position2.y - position.y < 450f && num4 < num)
				{
					result = player;
					num = num4;
				}
			}
		}
		return result;
	}

	[RPC]
	private void changeDoor(PhotonMessageInfo info)
	{
		door_broken.SetActive(value: true);
		door_closed.SetActive(value: false);
	}

	private void idle()
	{
		state = "idle";
		CrossFade("idle", 0.2f);
	}

	private void callTitanHAHA()
	{
		attackCount++;
		int num = 4;
		int num2 = 7;
		if (IN_GAME_MAIN_CAMERA.Difficulty != 0)
		{
			if (IN_GAME_MAIN_CAMERA.Difficulty == 1)
			{
				num = 4;
				num2 = 6;
			}
			else if (IN_GAME_MAIN_CAMERA.Difficulty == 2)
			{
				num = 3;
				num2 = 5;
			}
		}
		if (attackCount % num == 0)
		{
			CallTitan();
		}
		if ((double)NapeArmor < (double)NapeArmorTotal * 0.3)
		{
			if (attackCount % (int)((float)num2 * 0.5f) == 0)
			{
				CallTitan(special: true);
			}
		}
		else if (attackCount % num2 == 0)
		{
			CallTitan(special: true);
		}
	}

	private void attack_sweep(string type = "")
	{
		callTitanHAHA();
		state = "attack_sweep";
		attackAnimation = "sweep" + type;
		attackCheckTimeA = 0.4f;
		attackCheckTimeB = 0.57f;
		checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R");
		checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
		checkHitCapsuleR = 20f;
		CrossFade("attack_" + attackAnimation, 0.1f);
		attackChkOnce = false;
		sweepSmokeObject.GetComponent<ParticleSystem>().enableEmission = true;
		sweepSmokeObject.GetComponent<ParticleSystem>().Play();
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("startSweepSmoke", PhotonTargets.Others);
		}
	}

	private void KickWall()
	{
		state = "kick";
		actionName = "attack_kick_wall";
		attackCheckTime = 0.64f;
		attackChkOnce = false;
		CrossFade(actionName, 0.1f);
	}

	private void Slap(string type)
	{
		callTitanHAHA();
		state = "slap";
		attackAnimation = type;
		if (type == "r1" || type == "r2")
		{
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
		}
		if (type == "l1" || type == "l2")
		{
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
		}
		attackCheckTime = 0.57f;
		attackChkOnce = false;
		CrossFade("attack_slap_" + attackAnimation, 0.1f);
	}

	private void Steam()
	{
		callTitanHAHA();
		state = "steam";
		actionName = "attack_steam";
		attackCheckTime = 0.45f;
		CrossFade(actionName, 0.1f);
		attackChkOnce = false;
	}

	private void Kill(GameObject hitHero)
	{
		if (hitHero == null)
		{
			return;
		}
		Vector3 position = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest").position;
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
		{
			if (!hitHero.GetComponent<HERO>().HasDied())
			{
				hitHero.GetComponent<HERO>().Die((hitHero.transform.position - position) * 15f * 4f, isBite: false);
			}
		}
		else if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && !hitHero.GetComponent<HERO>().HasDied())
		{
			hitHero.GetComponent<HERO>().MarkDead();
			hitHero.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, (hitHero.transform.position - position) * 15f * 4f, false, -1, "Colossal Titan", true);
		}
	}

	private void playSound(string sndname)
	{
		playsoundRPC(sndname);
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("playsoundRPC", PhotonTargets.Others, sndname);
		}
	}

	[RPC]
	private void playsoundRPC(string sndname)
	{
		base.transform.Find(sndname).GetComponent<AudioSource>().Play();
	}

	[RPC]
	private void startNeckSteam()
	{
		neckSteamObject.GetComponent<ParticleSystem>().Stop();
		neckSteamObject.GetComponent<ParticleSystem>().Play();
	}

	[RPC]
	private void startSweepSmoke()
	{
		sweepSmokeObject.GetComponent<ParticleSystem>().enableEmission = true;
		sweepSmokeObject.GetComponent<ParticleSystem>().Play();
	}

	[RPC]
	private void stopSweepSmoke()
	{
		sweepSmokeObject.GetComponent<ParticleSystem>().enableEmission = false;
		sweepSmokeObject.GetComponent<ParticleSystem>().Stop();
	}

	private void neckSteam()
	{
		neckSteamObject.GetComponent<ParticleSystem>().Stop();
		neckSteamObject.GetComponent<ParticleSystem>().Play();
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("startNeckSteam", PhotonTargets.Others);
		}
		isSteamNeed = true;
		Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
		float radius = 30f;
		Collider[] array = Physics.OverlapSphere(transform.transform.position - base.transform.forward * 10f, radius);
		foreach (Collider collider in array)
		{
			if (collider.transform.root.tag == "Player")
			{
				GameObject gameObject = collider.transform.root.gameObject;
				if (!gameObject.GetComponent<TITAN_EREN>() && (bool)gameObject.GetComponent<HERO>())
				{
					blowPlayer(gameObject, transform);
				}
			}
		}
	}

	public void blowPlayer(GameObject player, Transform neck)
	{
		Vector3 vector = -(neck.position + base.transform.forward * 50f - player.transform.position);
		float num = 20f;
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
		{
			player.GetComponent<HERO>().blowAway(vector.normalized * num + Vector3.up * 1f);
		}
		else if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
		{
			player.GetComponent<HERO>().photonView.RPC("blowAway", PhotonTargets.All, vector.normalized * num + Vector3.up * 1f);
		}
	}

	[RPC]
	public void titanGetHit(int viewId, int speed)
	{
		Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
		PhotonView photonView = PhotonView.Find(viewId);
		if (photonView == null || !((photonView.gameObject.transform.position - transform.transform.position).magnitude < lagMax) || !(healthTime <= 0f))
		{
			return;
		}
		if (speed >= RCSettings.MinimumDamage)
		{
			NapeArmor -= speed;
		}
		if ((float)maxHealth > 0f)
		{
			base.photonView.RPC("labelRPC", PhotonTargets.AllBuffered, NapeArmor, maxHealth);
		}
		neckSteam();
		if (NapeArmor <= 0)
		{
			NapeArmor = 0;
			if (!hasDie)
			{
				base.photonView.RPC("netDie", PhotonTargets.OthersBuffered);
				netDie();
				FengGameManagerMKII.Instance.titanGetKill(photonView.owner, speed, base.name);
			}
		}
		else
		{
			FengGameManagerMKII.Instance.SendKillInfo(isKillerTitan: false, (string)photonView.owner.customProperties[PhotonPlayerProperty.Name], isVictimTitan: true, "Colossal Titan's neck", speed);
			FengGameManagerMKII.Instance.photonView.RPC("netShowDamage", photonView.owner, speed);
		}
		healthTime = 0.2f;
	}

	[RPC]
	private void removeMe(PhotonMessageInfo info)
	{
		if (ColossalChecker.IsRemovalValid(info))
		{
			Object.Destroy(base.gameObject);
		}
	}

	[RPC]
	public void netDie()
	{
		if (!hasDie)
		{
			hasDie = true;
		}
	}

	private void CallTitan(bool special = false)
	{
		if (!special && FengGameManagerMKII.Instance.AllTitans.Count > 6)
		{
			return;
		}
		ArrayList arrayList = new ArrayList();
		GameObject[] array = GameObject.FindGameObjectsWithTag("titanRespawn");
		foreach (GameObject gameObject in array)
		{
			if (gameObject.transform.parent.name == "titanRespawnCT")
			{
				arrayList.Add(gameObject);
			}
		}
		GameObject gameObject2 = (GameObject)arrayList[Random.Range(0, arrayList.Count)];
		string[] array2 = new string[1] { "TITAN_VER3.1" };
		GameObject gameObject3 = PhotonNetwork.Instantiate(array2[Random.Range(0, array2.Length)], gameObject2.transform.position, gameObject2.transform.rotation, 0);
		if (special)
		{
			GameObject[] array3 = GameObject.FindGameObjectsWithTag("route");
			GameObject gameObject4 = array3[Random.Range(0, array3.Length)];
			while (gameObject4.name != "routeCT")
			{
				gameObject4 = array3[Random.Range(0, array3.Length)];
			}
			gameObject3.GetComponent<TITAN>().SetRoute(gameObject4);
			gameObject3.GetComponent<TITAN>().setAbnormalType2(TitanClass.Aberrant, forceCrawler: false);
			gameObject3.GetComponent<TITAN>().activeRad = 0;
			gameObject3.GetComponent<TITAN>().ToCheckpoint((Vector3)gameObject3.GetComponent<TITAN>().checkPoints[0], 10f);
		}
		else
		{
			float num = 0.7f;
			float num2 = 0.7f;
			if (IN_GAME_MAIN_CAMERA.Difficulty != 0)
			{
				if (IN_GAME_MAIN_CAMERA.Difficulty == 1)
				{
					num = 0.4f;
					num2 = 0.7f;
				}
				else if (IN_GAME_MAIN_CAMERA.Difficulty == 2)
				{
					num = -1f;
					num2 = 0.7f;
				}
			}
			if (FengGameManagerMKII.Instance.AllTitans.Count == 5)
			{
				gameObject3.GetComponent<TITAN>().setAbnormalType2(TitanClass.Jumper, forceCrawler: false);
			}
			else if (!(Random.Range(0f, 1f) < num))
			{
				if (Random.Range(0f, 1f) < num2)
				{
					gameObject3.GetComponent<TITAN>().setAbnormalType2(TitanClass.Jumper, forceCrawler: false);
				}
				else
				{
					gameObject3.GetComponent<TITAN>().setAbnormalType2(TitanClass.Crawler, forceCrawler: false);
				}
			}
			gameObject3.GetComponent<TITAN>().activeRad = 200;
		}
		PhotonNetwork.Instantiate("FX/FXtitanSpawn", gameObject3.transform.position, Quaternion.Euler(-90f, 0f, 0f), 0).transform.localScale = gameObject3.transform.localScale;
	}

	public void update()
	{
		healthTime -= Time.deltaTime;
		UpdateLabel();
		switch (state)
		{
		case "null":
			return;
		case "wait":
			waitTime -= Time.deltaTime;
			if (waitTime <= 0f)
			{
				base.transform.position = new Vector3(30f, 0f, 784f);
				Object.Instantiate(Resources.Load("FX/ThunderCT"), base.transform.position + Vector3.up * 350f, Quaternion.Euler(270f, 0f, 0f));
				Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().Flash();
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer || base.photonView.isMine)
				{
					idle();
				}
				else
				{
					state = "null";
				}
			}
			return;
		case "attack_sweep":
			if (attackCheckTimeA != 0f && ((base.animation["attack_" + attackAnimation].normalizedTime >= attackCheckTimeA && base.animation["attack_" + attackAnimation].normalizedTime <= attackCheckTimeB) || (!attackChkOnce && base.animation["attack_" + attackAnimation].normalizedTime >= attackCheckTimeA)))
			{
				if (!attackChkOnce)
				{
					attackChkOnce = true;
				}
				RaycastHit[] array = checkHitCapsule(checkHitCapsuleStart.position, checkHitCapsuleEnd.position, checkHitCapsuleR);
				foreach (RaycastHit raycastHit in array)
				{
					GameObject gameObject2 = raycastHit.collider.gameObject;
					if (gameObject2.tag == "Player")
					{
						Kill(gameObject2);
					}
					if (gameObject2.tag == "erenHitbox" && attackAnimation == "combo_3" && IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
					{
						gameObject2.transform.root.gameObject.GetComponent<TITAN_EREN>().hitByFTByServer(3);
					}
				}
				array = checkHitCapsule(checkHitCapsuleEndOld, checkHitCapsuleEnd.position, checkHitCapsuleR);
				foreach (RaycastHit raycastHit2 in array)
				{
					GameObject gameObject3 = raycastHit2.collider.gameObject;
					if (gameObject3.tag == "Player")
					{
						Kill(gameObject3);
					}
				}
				checkHitCapsuleEndOld = checkHitCapsuleEnd.position;
			}
			if (base.animation["attack_" + attackAnimation].normalizedTime >= 1f)
			{
				sweepSmokeObject.GetComponent<ParticleSystem>().enableEmission = false;
				sweepSmokeObject.GetComponent<ParticleSystem>().Stop();
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
				{
					base.photonView.RPC("stopSweepSmoke", PhotonTargets.Others);
				}
				FindNearestHero();
				idle();
				PlayAnimation("idle");
			}
			return;
		case "kick":
			if (!attackChkOnce && base.animation[actionName].normalizedTime >= attackCheckTime)
			{
				attackChkOnce = true;
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
				{
					base.photonView.RPC("changeDoor", PhotonTargets.AllBuffered);
				}
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
				{
					PhotonNetwork.Instantiate("FX/boom1_CT_KICK", base.transform.position + base.transform.forward * 120f + base.transform.right * 30f, Quaternion.Euler(270f, 0f, 0f), 0);
					PhotonNetwork.Instantiate("rock", base.transform.position + base.transform.forward * 120f + base.transform.right * 30f, Quaternion.Euler(0f, 0f, 0f), 0);
				}
				else
				{
					Object.Instantiate(Resources.Load("FX/boom1_CT_KICK"), base.transform.position + base.transform.forward * 120f + base.transform.right * 30f, Quaternion.Euler(270f, 0f, 0f));
					Object.Instantiate(Resources.Load("rock"), base.transform.position + base.transform.forward * 120f + base.transform.right * 30f, Quaternion.Euler(0f, 0f, 0f));
				}
			}
			if (base.animation[actionName].normalizedTime >= 1f)
			{
				FindNearestHero();
				idle();
				PlayAnimation("idle");
			}
			return;
		case "slap":
			if (!attackChkOnce && base.animation["attack_slap_" + attackAnimation].normalizedTime >= attackCheckTime)
			{
				attackChkOnce = true;
				GameObject gameObject4;
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
				{
					gameObject4 = PhotonNetwork.Instantiate("FX/boom1", checkHitCapsuleStart.position, Quaternion.Euler(270f, 0f, 0f), 0);
					if (gameObject4.GetComponent<EnemyfxIDcontainer>() != null)
					{
						gameObject4.GetComponent<EnemyfxIDcontainer>().titanName = base.name;
					}
				}
				else
				{
					gameObject4 = (GameObject)Object.Instantiate(Resources.Load("FX/boom1"), checkHitCapsuleStart.position, Quaternion.Euler(270f, 0f, 0f));
				}
				gameObject4.transform.localScale = new Vector3(5f, 5f, 5f);
			}
			if (base.animation["attack_slap_" + attackAnimation].normalizedTime >= 1f)
			{
				FindNearestHero();
				idle();
				PlayAnimation("idle");
			}
			return;
		case "steam":
			if (!attackChkOnce && base.animation[actionName].normalizedTime >= attackCheckTime)
			{
				attackChkOnce = true;
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
				{
					PhotonNetwork.Instantiate("FX/colossal_steam", base.transform.position + base.transform.up * 185f, Quaternion.Euler(270f, 0f, 0f), 0);
					PhotonNetwork.Instantiate("FX/colossal_steam", base.transform.position + base.transform.up * 303f, Quaternion.Euler(270f, 0f, 0f), 0);
					PhotonNetwork.Instantiate("FX/colossal_steam", base.transform.position + base.transform.up * 50f, Quaternion.Euler(270f, 0f, 0f), 0);
				}
				else
				{
					Object.Instantiate(Resources.Load("FX/colossal_steam"), base.transform.position + base.transform.forward * 185f, Quaternion.Euler(270f, 0f, 0f));
					Object.Instantiate(Resources.Load("FX/colossal_steam"), base.transform.position + base.transform.forward * 303f, Quaternion.Euler(270f, 0f, 0f));
					Object.Instantiate(Resources.Load("FX/colossal_steam"), base.transform.position + base.transform.forward * 50f, Quaternion.Euler(270f, 0f, 0f));
				}
			}
			if (!(base.animation[actionName].normalizedTime >= 1f))
			{
				return;
			}
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
			{
				GameObject gameObject = PhotonNetwork.Instantiate("FX/colossal_steam_dmg", base.transform.position + base.transform.up * 185f, Quaternion.Euler(270f, 0f, 0f), 0);
				if (gameObject.GetComponent<EnemyfxIDcontainer>() != null)
				{
					gameObject.GetComponent<EnemyfxIDcontainer>().titanName = base.name;
				}
				gameObject = PhotonNetwork.Instantiate("FX/colossal_steam_dmg", base.transform.position + base.transform.up * 303f, Quaternion.Euler(270f, 0f, 0f), 0);
				if (gameObject.GetComponent<EnemyfxIDcontainer>() != null)
				{
					gameObject.GetComponent<EnemyfxIDcontainer>().titanName = base.name;
				}
				gameObject = PhotonNetwork.Instantiate("FX/colossal_steam_dmg", base.transform.position + base.transform.up * 50f, Quaternion.Euler(270f, 0f, 0f), 0);
				if (gameObject.GetComponent<EnemyfxIDcontainer>() != null)
				{
					gameObject.GetComponent<EnemyfxIDcontainer>().titanName = base.name;
				}
			}
			else
			{
				Object.Instantiate(Resources.Load("FX/colossal_steam_dmg"), base.transform.position + base.transform.forward * 185f, Quaternion.Euler(270f, 0f, 0f));
				Object.Instantiate(Resources.Load("FX/colossal_steam_dmg"), base.transform.position + base.transform.forward * 303f, Quaternion.Euler(270f, 0f, 0f));
				Object.Instantiate(Resources.Load("FX/colossal_steam_dmg"), base.transform.position + base.transform.forward * 50f, Quaternion.Euler(270f, 0f, 0f));
			}
			if (hasDie)
			{
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
				{
					Object.Destroy(base.gameObject);
				}
				else if (PhotonNetwork.isMasterClient)
				{
					PhotonNetwork.Destroy(base.photonView);
				}
				FengGameManagerMKII.Instance.FinishGame();
			}
			FindNearestHero();
			idle();
			PlayAnimation("idle");
			return;
		}
		switch (attackPattern)
		{
		case -1:
			Slap("r1");
			attackPattern++;
			return;
		case 0:
			attack_sweep(string.Empty);
			attackPattern++;
			return;
		case 1:
			Steam();
			attackPattern++;
			return;
		case 2:
			KickWall();
			attackPattern++;
			return;
		}
		if (isSteamNeed || hasDie)
		{
			Steam();
			isSteamNeed = false;
			return;
		}
		if (myHero == null)
		{
			FindNearestHero();
			return;
		}
		Vector3 vector = myHero.transform.position - base.transform.position;
		float current = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
		float f = 0f - Mathf.DeltaAngle(current, base.gameObject.transform.rotation.eulerAngles.y - 90f);
		myDistance = Mathf.Sqrt((myHero.transform.position.x - base.transform.position.x) * (myHero.transform.position.x - base.transform.position.x) + (myHero.transform.position.z - base.transform.position.z) * (myHero.transform.position.z - base.transform.position.z));
		float num = myHero.transform.position.y - base.transform.position.y;
		if (myDistance < 85f && Random.Range(0, 100) < 5)
		{
			Steam();
			return;
		}
		if (num > 310f && num < 350f)
		{
			if (Vector3.Distance(myHero.transform.position, base.transform.Find("APL1").position) < 40f)
			{
				Slap("l1");
				return;
			}
			if (Vector3.Distance(myHero.transform.position, base.transform.Find("APL2").position) < 40f)
			{
				Slap("l2");
				return;
			}
			if (Vector3.Distance(myHero.transform.position, base.transform.Find("APR1").position) < 40f)
			{
				Slap("r1");
				return;
			}
			if (Vector3.Distance(myHero.transform.position, base.transform.Find("APR2").position) < 40f)
			{
				Slap("r2");
				return;
			}
			if (myDistance < 150f && Mathf.Abs(f) < 80f)
			{
				attack_sweep(string.Empty);
				return;
			}
		}
		if (num < 300f && Mathf.Abs(f) < 80f && myDistance < 85f)
		{
			attack_sweep("_vertical");
			return;
		}
		switch (Random.Range(0, 7))
		{
		case 0:
			Slap("l1");
			break;
		case 1:
			Slap("l2");
			break;
		case 2:
			Slap("r1");
			break;
		case 3:
			Slap("r2");
			break;
		case 4:
			attack_sweep(string.Empty);
			break;
		case 5:
			attack_sweep("_vertical");
			break;
		case 6:
			Steam();
			break;
		}
	}

	private void Start()
	{
		StartMain();
		size = 20f;
		if (Minimap.Instance != null)
		{
			Minimap.Instance.TrackGameObjectOnMinimap(base.gameObject, Color.black, trackOrientation: false, depthAboveAll: true);
		}
		if (base.photonView.isMine)
		{
			if (RCSettings.SizeMode > 0)
			{
				float sizeLower = RCSettings.SizeLower;
				float sizeUpper = RCSettings.SizeUpper;
				size = Random.Range(sizeLower, sizeUpper);
				base.photonView.RPC("setSize", PhotonTargets.AllBuffered, size);
			}
			lagMax = 150f + size * 3f;
			healthTime = 0f;
			maxHealth = NapeArmor;
			if (RCSettings.HealthMode > 0)
			{
				maxHealth = (NapeArmor = Random.Range(RCSettings.HealthLower, RCSettings.HealthUpper));
			}
			if (NapeArmor > 0)
			{
				base.photonView.RPC("labelRPC", PhotonTargets.AllBuffered, NapeArmor, maxHealth);
			}
			LoadSkin();
		}
		hasspawn = true;
	}

	[RPC]
	public void setSize(float size, PhotonMessageInfo info)
	{
		size = Mathf.Clamp(size, 0.1f, 50f);
		if (info.sender.isMasterClient)
		{
			base.transform.localScale *= size * 0.05f;
			this.size = size;
		}
	}

	public void LoadSkin()
	{
		if (PhotonNetwork.isMasterClient && (int)FengGameManagerMKII.Settings[1] == 1)
		{
			base.photonView.RPC("loadskinRPC", PhotonTargets.AllBuffered, (string)FengGameManagerMKII.Settings[67]);
		}
	}

	[RPC]
	public void loadskinRPC(string url, PhotonMessageInfo info)
	{
		if (info.sender.isMasterClient && (int)FengGameManagerMKII.Settings[1] == 1 && (url.EndsWith(".jpg") || url.EndsWith(".png") || url.EndsWith(".jpeg")))
		{
			StartCoroutine(CoLoadSkin(url));
		}
	}

	public IEnumerator CoLoadSkin(string url)
	{
		while (!hasspawn)
		{
			yield return null;
		}
		bool flag = true;
		bool unload = false;
		if ((int)FengGameManagerMKII.Settings[63] == 1)
		{
			flag = false;
		}
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer31 in componentsInChildren)
		{
			if (!renderer31.name.Contains("hair"))
			{
				continue;
			}
			if (!FengGameManagerMKII.LinkHash[2].ContainsKey(url))
			{
				WWW link = SkinChecker.CreateWWW(url);
				if (link != null)
				{
					yield return link;
					Texture2D mainTexture = RCextensions.LoadImage(link, flag, 2000000);
					link.Dispose();
					if (!FengGameManagerMKII.LinkHash[2].ContainsKey(url))
					{
						unload = true;
						renderer31.material.mainTexture = mainTexture;
						FengGameManagerMKII.LinkHash[2].Add(url, renderer31.material);
					}
					renderer31.material = (Material)FengGameManagerMKII.LinkHash[2][url];
				}
			}
			else
			{
				renderer31.material = (Material)FengGameManagerMKII.LinkHash[2][url];
			}
		}
		if (unload)
		{
			FengGameManagerMKII.Instance.UnloadAssets();
		}
	}

	public void UpdateLabel()
	{
		if (healthLabel != null && healthLabel.GetComponent<UILabel>().isVisible)
		{
			healthLabel.transform.LookAt(2f * healthLabel.transform.position - Camera.main.transform.position);
		}
	}

	[RPC]
	public void labelRPC(int health, int maxHealth)
	{
		if (health < 0)
		{
			if (healthLabel != null)
			{
				Object.Destroy(healthLabel);
			}
			return;
		}
		if (healthLabel == null)
		{
			healthLabel = (GameObject)Object.Instantiate(Resources.Load("UI/LabelNameOverHead"));
			healthLabel.name = "LabelNameOverHead";
			healthLabel.transform.parent = base.transform;
			healthLabel.transform.localPosition = new Vector3(0f, 430f, 0f);
			float num = 15f;
			if (size > 0f && size < 1f)
			{
				num = 15f / size;
				num = Mathf.Min(num, 100f);
			}
			healthLabel.transform.localScale = new Vector3(num, num, num);
		}
		string text = "[7FFF00]";
		float num2 = (float)health / (float)maxHealth;
		if (num2 < 0.75f && num2 >= 0.5f)
		{
			text = "[F2B50F]";
		}
		else if (num2 < 0.5f && num2 >= 0.25f)
		{
			text = "[FF8100]";
		}
		else if (num2 < 0.25f)
		{
			text = "[FF3333]";
		}
		healthLabel.GetComponent<UILabel>().text = text + health;
	}
}
