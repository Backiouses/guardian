using System;
using System.Collections;
using Guardian.AntiAbuse.Validators;
using Photon;
using UnityEngine;

public class FEMALE_TITAN : Photon.MonoBehaviour
{
	public static float minusDistance = 99999f;

	public static GameObject minusDistanceEnemy;

	public bool hasDie;

	public GameObject myHero;

	private string state = "idle";

	public float speed = 80f;

	private float gravity = 120f;

	public float maxVelocityChange = 80f;

	public GameObject currentCamera;

	public float chaseDistance = 80f;

	public float attackDistance = 13f;

	public float attackWait = 1f;

	public float myDistance;

	private Vector3 oldCorePosition;

	public GameObject bottomObject;

	public GameObject grabTF;

	private float tauntTime;

	private GameObject whoHasTauntMe;

	public int NapeArmor = 1000;

	public int AnkleLHP = 200;

	public int AnkleRHP = 200;

	private int AnkleLHPMAX = 200;

	private int AnkleRHPMAX = 200;

	private Transform checkHitCapsuleStart;

	private Transform checkHitCapsuleEnd;

	private Vector3 checkHitCapsuleEndOld;

	private float checkHitCapsuleR;

	private Vector3 abnorma_jump_bite_horizon_v;

	private float attention = 10f;

	private GameObject eren;

	private bool hasDieSteam;

	private float dieTime;

	private bool startJump;

	private bool needFreshCorePosition;

	private int stepSoundPhase = 2;

	private bool grounded;

	private bool attacked;

	private string attackAnimation;

	private string nextAttackAnimation;

	private bool isAttackMoveByCore;

	private string fxName;

	private Vector3 fxPosition;

	private Quaternion fxRotation;

	private float attackCheckTime;

	private float attackCheckTimeA;

	private float attackCheckTimeB;

	private bool attackChkOnce;

	private Transform currentGrabHand;

	private bool isGrabHandLeft;

	private GameObject grabbedTarget;

	private float sbtime;

	private float turnDeg;

	private float desDeg;

	private string turnAnimation;

	public GameObject healthLabel;

	public float lagMax;

	public float healthTime;

	public int maxHealth;

	public float size;

	public bool hasspawn;

	private void Awake()
	{
		base.rigidbody.freezeRotation = true;
		base.rigidbody.useGravity = false;
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
		if (AnnieChecker.IsAnimationPlayValid(this, info))
		{
			LocalPlayAnimation(aniName);
		}
	}

	[RPC]
	private void netPlayAnimationAt(string aniName, float normalizedTime, PhotonMessageInfo info)
	{
		if (AnnieChecker.IsAnimationSeekedPlayValid(this, info))
		{
			LocalPlayAnimationAt(aniName, normalizedTime);
		}
	}

	[RPC]
	private void netCrossFade(string aniName, float time, PhotonMessageInfo info)
	{
		if (AnnieChecker.IsCrossFadeValid(this, info))
		{
			LocalCrossFade(aniName, time);
		}
	}

	private void OnDestroy()
	{
		if (GameObject.Find("MultiplayerManager") != null)
		{
			GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().RemoveAnnie(this);
		}
	}

	private void StartMain()
	{
		FengGameManagerMKII.Instance.AddAnnie(this);
		base.name = "Female Titan";
		grabTF = new GameObject();
		grabTF.name = "titansTmpGrabTF";
		currentCamera = GameObject.Find("MainCamera");
		oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
		if (myHero == null)
		{
			findNearestHero();
		}
		foreach (AnimationState item in base.animation)
		{
			item.speed = 0.7f;
		}
		base.animation["turn180"].speed = 0.5f;
		NapeArmor = 1000;
		AnkleLHP = 50;
		AnkleRHP = 50;
		AnkleLHPMAX = 50;
		AnkleRHPMAX = 50;
		bool flag = false;
		if (FengGameManagerMKII.Level.RespawnMode == RespawnMode.Never)
		{
			flag = true;
		}
		if (IN_GAME_MAIN_CAMERA.Difficulty == 0)
		{
			NapeArmor = ((!flag) ? 1000 : 1000);
			AnkleLHP = (AnkleLHPMAX = ((!flag) ? 50 : 50));
			AnkleRHP = (AnkleRHPMAX = ((!flag) ? 50 : 50));
		}
		else if (IN_GAME_MAIN_CAMERA.Difficulty == 1)
		{
			NapeArmor = ((!flag) ? 3000 : 2500);
			AnkleLHP = (AnkleLHPMAX = ((!flag) ? 200 : 100));
			AnkleRHP = (AnkleRHPMAX = ((!flag) ? 200 : 100));
			foreach (AnimationState item2 in base.animation)
			{
				item2.speed = 0.7f;
			}
			base.animation["turn180"].speed = 0.7f;
		}
		else if (IN_GAME_MAIN_CAMERA.Difficulty == 2)
		{
			NapeArmor = ((!flag) ? 6000 : 4000);
			AnkleLHP = (AnkleLHPMAX = ((!flag) ? 1000 : 200));
			AnkleRHP = (AnkleRHPMAX = ((!flag) ? 1000 : 200));
			foreach (AnimationState item3 in base.animation)
			{
				item3.speed = 1f;
			}
			base.animation["turn180"].speed = 0.9f;
		}
		if (IN_GAME_MAIN_CAMERA.Gamemode == GameMode.PvPCapture)
		{
			NapeArmor = (int)((float)NapeArmor * 0.8f);
		}
		base.animation["legHurt"].speed = 1f;
		base.animation["legHurt_loop"].speed = 1f;
		base.animation["legHurt_getup"].speed = 1f;
	}

	private void findNearestHero()
	{
		myHero = getNearestHero();
		attention = UnityEngine.Random.Range(5f, 10f);
	}

	private float getNearestHeroDistance()
	{
		float num = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		foreach (GameObject player in FengGameManagerMKII.Instance.Players)
		{
			float magnitude = (player.transform.position - position).magnitude;
			if (magnitude < num)
			{
				num = magnitude;
			}
		}
		return num;
	}

	private GameObject getNearestHero()
	{
		GameObject result = null;
		float num = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		foreach (GameObject player in FengGameManagerMKII.Instance.Players)
		{
			if ((!player.GetComponent<HERO>() || !player.GetComponent<HERO>().HasDied()) && (!player.GetComponent<TITAN_EREN>() || !player.GetComponent<TITAN_EREN>().hasDied))
			{
				float sqrMagnitude = (player.transform.position - position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = player;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	public bool IsGrounded()
	{
		return bottomObject.GetComponent<CheckHitGround>().isGrounded;
	}

	private GameObject checkIfHitHand(Transform hand)
	{
		float num = 9.6f;
		Collider[] array = Physics.OverlapSphere(hand.GetComponent<SphereCollider>().transform.position, num + 1f);
		foreach (Collider collider in array)
		{
			if (!(collider.transform.root.tag == "Player"))
			{
				continue;
			}
			GameObject gameObject = collider.transform.root.gameObject;
			if ((bool)gameObject.GetComponent<TITAN_EREN>())
			{
				if (!gameObject.GetComponent<TITAN_EREN>().isHit)
				{
					gameObject.GetComponent<TITAN_EREN>().HitByTitan();
				}
				return gameObject;
			}
			if ((bool)gameObject.GetComponent<HERO>() && !gameObject.GetComponent<HERO>().IsInvincible())
			{
				return gameObject;
			}
		}
		return null;
	}

	private RaycastHit[] checkHitCapsule(Vector3 start, Vector3 end, float r)
	{
		return Physics.SphereCastAll(start, r, end - start, Vector3.Distance(start, end));
	}

	private void killPlayer(GameObject hitHero)
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
		else if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient && !hitHero.GetComponent<HERO>().HasDied())
		{
			hitHero.GetComponent<HERO>().MarkDead();
			hitHero.GetComponent<HERO>().photonView.RPC("netDie", PhotonTargets.All, (hitHero.transform.position - position) * 15f * 4f, false, -1, "Female Titan", true);
		}
	}

	public void beTauntedBy(GameObject target, float tauntTime)
	{
		whoHasTauntMe = target;
		this.tauntTime = tauntTime;
	}

	public void erenIsHere(GameObject target)
	{
		myHero = (eren = target);
	}

	private bool attackTarget(GameObject target)
	{
		float num = 0f;
		Vector3 vector = target.transform.position - base.transform.position;
		float current = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
		num = 0f - Mathf.DeltaAngle(current, base.gameObject.transform.rotation.eulerAngles.y - 90f);
		if (eren != null && myDistance < 35f)
		{
			attack("combo_1");
			return true;
		}
		int num2 = 0;
		string text = string.Empty;
		ArrayList arrayList = new ArrayList();
		if (myDistance < 40f)
		{
			num2 = ((!(Mathf.Abs(num) < 90f)) ? ((!(num > 0f)) ? 3 : 4) : ((num > 0f) ? 1 : 2));
			float num3 = target.transform.position.y - base.transform.position.y;
			if (Mathf.Abs(num) < 90f)
			{
				if (num3 > 0f && num3 < 12f && myDistance < 22f)
				{
					arrayList.Add("attack_sweep");
				}
				if (num3 >= 55f && num3 < 90f)
				{
					arrayList.Add("attack_jumpCombo_1");
				}
			}
			if (Mathf.Abs(num) < 90f && num3 > 12f && num3 < 40f)
			{
				arrayList.Add("attack_combo_1");
			}
			if (Mathf.Abs(num) < 30f)
			{
				if (num3 > 0f && num3 < 12f && myDistance > 20f && myDistance < 30f)
				{
					arrayList.Add("attack_front");
				}
				if (myDistance < 12f && num3 > 33f && num3 < 51f)
				{
					arrayList.Add("grab_up");
				}
			}
			if (Mathf.Abs(num) > 100f && myDistance < 11f && num3 >= 15f && num3 < 32f)
			{
				arrayList.Add("attack_sweep_back");
			}
			switch (num2)
			{
			case 2:
				if (myDistance < 11f)
				{
					if (num3 >= 21f && num3 < 32f)
					{
						arrayList.Add("attack_sweep_front_left");
					}
				}
				else if (myDistance < 20f)
				{
					if (num3 >= 12f && num3 < 21f)
					{
						arrayList.Add("grab_bottom_left");
					}
					else if (num3 >= 21f && num3 < 32f)
					{
						arrayList.Add("grab_mid_left");
					}
					else if (num3 >= 32f && num3 < 47f)
					{
						arrayList.Add("grab_up_left");
					}
				}
				break;
			case 1:
				if (myDistance < 11f)
				{
					if (num3 >= 21f && num3 < 32f)
					{
						arrayList.Add("attack_sweep_front_right");
					}
				}
				else if (myDistance < 20f)
				{
					if (num3 >= 12f && num3 < 21f)
					{
						arrayList.Add("grab_bottom_right");
					}
					else if (num3 >= 21f && num3 < 32f)
					{
						arrayList.Add("grab_mid_right");
					}
					else if (num3 >= 32f && num3 < 47f)
					{
						arrayList.Add("grab_up_right");
					}
				}
				break;
			case 3:
				if (myDistance < 11f)
				{
					if (num3 >= 33f && num3 < 51f)
					{
						arrayList.Add("attack_sweep_head_b_l");
					}
				}
				else
				{
					arrayList.Add("turn180");
				}
				break;
			case 4:
				if (myDistance < 11f)
				{
					if (num3 >= 33f && num3 < 51f)
					{
						arrayList.Add("attack_sweep_head_b_r");
					}
				}
				else
				{
					arrayList.Add("turn180");
				}
				break;
			}
		}
		if (arrayList.Count > 0)
		{
			text = (string)arrayList[UnityEngine.Random.Range(0, arrayList.Count)];
		}
		else if (UnityEngine.Random.Range(0, 100) < 10)
		{
			myHero = FengGameManagerMKII.Instance.Players[UnityEngine.Random.Range(0, FengGameManagerMKII.Instance.Players.Count)];
			attention = UnityEngine.Random.Range(5f, 10f);
			return true;
		}
		switch (text)
		{
		case "grab_bottom_left":
			grab("bottom_left");
			return true;
		case "grab_bottom_right":
			grab("bottom_right");
			return true;
		case "grab_mid_left":
			grab("mid_left");
			return true;
		case "grab_mid_right":
			grab("mid_right");
			return true;
		case "grab_up":
			grab("up");
			return true;
		case "grab_up_left":
			grab("up_left");
			return true;
		case "grab_up_right":
			grab("up_right");
			return true;
		case "attack_combo_1":
			attack("combo_1");
			return true;
		case "attack_front":
			attack("front");
			return true;
		case "attack_jumpCombo_1":
			attack("jumpCombo_1");
			return true;
		case "attack_sweep":
			attack("sweep");
			return true;
		case "attack_sweep_back":
			attack("sweep_back");
			return true;
		case "attack_sweep_front_left":
			attack("sweep_front_left");
			return true;
		case "attack_sweep_front_right":
			attack("sweep_front_right");
			return true;
		case "attack_sweep_head_b_l":
			attack("sweep_head_b_l");
			return true;
		case "attack_sweep_head_b_r":
			attack("sweep_head_b_r");
			return true;
		case "turn180":
			turn180();
			return true;
		default:
			return false;
		}
	}

	public void update()
	{
		if ((IN_GAME_MAIN_CAMERA.IsPausing && IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer) || (IN_GAME_MAIN_CAMERA.Gametype != 0 && !base.photonView.isMine))
		{
			return;
		}
		if (hasDie)
		{
			dieTime += Time.deltaTime;
			if (base.animation["die"].normalizedTime >= 1f)
			{
				PlayAnimation("die_cry");
				if (IN_GAME_MAIN_CAMERA.Gamemode != GameMode.PvPCapture)
				{
					for (int i = 0; i < 15; i++)
					{
						FengGameManagerMKII.Instance.SpawnTitanRandom("titanRespawn", 50).GetComponent<TITAN>().beTauntedBy(base.gameObject, 20f);
					}
				}
			}
			if (dieTime > 2f && !hasDieSteam)
			{
				hasDieSteam = true;
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
				{
					GameObject obj = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/FXtitanDie1"));
					obj.transform.position = base.transform.Find("Amarture/Core/Controller_Body/hip").position;
					obj.transform.localScale = base.transform.localScale;
				}
				else if (base.photonView.isMine)
				{
					PhotonNetwork.Instantiate("FX/FXtitanDie1", base.transform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0).transform.localScale = base.transform.localScale;
				}
			}
			if (dieTime > ((IN_GAME_MAIN_CAMERA.Gamemode != GameMode.PvPCapture) ? 20f : 5f))
			{
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
				{
					GameObject obj2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/FXtitanDie"));
					obj2.transform.position = base.transform.Find("Amarture/Core/Controller_Body/hip").position;
					obj2.transform.localScale = base.transform.localScale;
					UnityEngine.Object.Destroy(base.gameObject);
				}
				else if (base.photonView.isMine)
				{
					PhotonNetwork.Instantiate("FX/FXtitanDie", base.transform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0).transform.localScale = base.transform.localScale;
					PhotonNetwork.Destroy(base.gameObject);
				}
			}
			return;
		}
		if (attention > 0f)
		{
			attention -= Time.deltaTime;
			if (attention < 0f)
			{
				attention = 0f;
				myHero = FengGameManagerMKII.Instance.Players[UnityEngine.Random.Range(0, FengGameManagerMKII.Instance.Players.Count)];
				attention = UnityEngine.Random.Range(5f, 10f);
			}
		}
		if (whoHasTauntMe != null)
		{
			tauntTime -= Time.deltaTime;
			if (tauntTime <= 0f)
			{
				whoHasTauntMe = null;
			}
			myHero = whoHasTauntMe;
		}
		if (eren != null)
		{
			if (!eren.GetComponent<TITAN_EREN>().hasDied)
			{
				myHero = eren;
			}
			else
			{
				eren = null;
				myHero = null;
			}
		}
		if (myHero == null)
		{
			findNearestHero();
			if (myHero != null)
			{
				return;
			}
		}
		if (myHero == null)
		{
			myDistance = float.MaxValue;
		}
		else
		{
			float num = myHero.transform.position.x - base.transform.position.x;
			float x = myHero.transform.position.x;
			float num2 = num * (x - base.transform.position.x);
			float num3 = myHero.transform.position.z - base.transform.position.z;
			float z = myHero.transform.position.z;
			myDistance = Mathf.Sqrt(num2 + num3 * (z - base.transform.position.z));
		}
		switch (state)
		{
		case "idle":
		{
			if (myHero == null)
			{
				break;
			}
			float num4 = 0f;
			Vector3 vector = myHero.transform.position - base.transform.position;
			float current = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
			num4 = 0f - Mathf.DeltaAngle(current, base.gameObject.transform.rotation.eulerAngles.y - 90f);
			if (attackTarget(myHero))
			{
				break;
			}
			if (Mathf.Abs(num4) < 90f)
			{
				chase();
			}
			else if (UnityEngine.Random.Range(0, 100) < 1)
			{
				turn180();
			}
			else if (Mathf.Abs(num4) > 100f)
			{
				if (UnityEngine.Random.Range(0, 100) < 10)
				{
					turn180();
				}
			}
			else if (Mathf.Abs(num4) > 45f && UnityEngine.Random.Range(0, 100) < 30)
			{
				turn(num4);
			}
			break;
		}
		case "attack":
			if (!attacked && attackCheckTime != 0f && base.animation["attack_" + attackAnimation].normalizedTime >= attackCheckTime)
			{
				attacked = true;
				fxPosition = base.transform.Find("ap_" + attackAnimation).position;
				GameObject gameObject = ((IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || !PhotonNetwork.isMasterClient) ? ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/" + fxName), fxPosition, fxRotation)) : PhotonNetwork.Instantiate("FX/" + fxName, fxPosition, fxRotation, 0));
				gameObject.transform.localScale = base.transform.localScale;
				float b = 1f - Vector3.Distance(currentCamera.transform.position, gameObject.transform.position) * 0.05f;
				b = Mathf.Min(1f, b);
				currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().StartShake(b, b);
			}
			if (attackCheckTimeA != 0f && ((base.animation["attack_" + attackAnimation].normalizedTime >= attackCheckTimeA && base.animation["attack_" + attackAnimation].normalizedTime <= attackCheckTimeB) || (!attackChkOnce && base.animation["attack_" + attackAnimation].normalizedTime >= attackCheckTimeA)))
			{
				if (!attackChkOnce)
				{
					attackChkOnce = true;
					playSound("snd_eren_swing" + UnityEngine.Random.Range(1, 3));
				}
				RaycastHit[] array = checkHitCapsule(checkHitCapsuleStart.position, checkHitCapsuleEnd.position, checkHitCapsuleR);
				foreach (RaycastHit raycastHit in array)
				{
					GameObject gameObject2 = raycastHit.collider.gameObject;
					if (gameObject2.tag == "Player")
					{
						killPlayer(gameObject2);
					}
					if (!(gameObject2.tag == "erenHitbox"))
					{
						continue;
					}
					if (attackAnimation == "combo_1")
					{
						if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
						{
							gameObject2.transform.root.gameObject.GetComponent<TITAN_EREN>().hitByFTByServer(1);
						}
					}
					else if (attackAnimation == "combo_2")
					{
						if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
						{
							gameObject2.transform.root.gameObject.GetComponent<TITAN_EREN>().hitByFTByServer(2);
						}
					}
					else if (attackAnimation == "combo_3" && IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
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
						killPlayer(gameObject3);
					}
				}
				checkHitCapsuleEndOld = checkHitCapsuleEnd.position;
			}
			if (attackAnimation == "jumpCombo_1" && base.animation["attack_" + attackAnimation].normalizedTime >= 0.65f && !startJump && myHero != null)
			{
				startJump = true;
				float y = myHero.rigidbody.velocity.y;
				float num5 = -20f;
				float num6 = gravity;
				float y2 = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position.y;
				float num7 = (num5 - num6) * 0.5f;
				float num8 = y;
				float num9 = myHero.transform.position.y - y2;
				float num10 = Mathf.Abs((Mathf.Sqrt(num8 * num8 - 4f * num7 * num9) - num8) / (2f * num7));
				Vector3 vector2 = myHero.transform.position + myHero.rigidbody.velocity * num10 + Vector3.up * 0.5f * num5 * num10 * num10;
				float y3 = vector2.y;
				if (num9 < 0f || y3 - y2 < 0f)
				{
					Idle();
					num10 = 0.5f;
					vector2 = base.transform.position + (y2 + 5f) * Vector3.up;
					y3 = vector2.y;
				}
				float num11 = y3 - y2;
				float num12 = Mathf.Sqrt(2f * num11 / gravity);
				float value = gravity * num12 + 20f;
				value = Mathf.Clamp(value, 20f, 90f);
				Vector3 vector3 = (vector2 - base.transform.position) / num10;
				abnorma_jump_bite_horizon_v = new Vector3(vector3.x, 0f, vector3.z);
				Vector3 velocity = base.rigidbody.velocity;
				Vector3 vector4 = new Vector3(abnorma_jump_bite_horizon_v.x, value, abnorma_jump_bite_horizon_v.z);
				if (vector4.magnitude > 90f)
				{
					vector4 = vector4.normalized * 90f;
				}
				Vector3 force = vector4 - velocity;
				base.rigidbody.AddForce(force, ForceMode.VelocityChange);
				float x2 = base.transform.position.x;
				Vector2 from = new Vector2(x2, base.transform.position.z);
				float x3 = myHero.transform.position.x;
				float num13 = Vector2.Angle(from, new Vector2(x3, myHero.transform.position.z));
				float y4 = myHero.transform.position.x - base.transform.position.x;
				float z2 = myHero.transform.position.z;
				num13 = Mathf.Atan2(y4, z2 - base.transform.position.z) * 57.29578f;
				base.gameObject.transform.rotation = Quaternion.Euler(0f, num13, 0f);
			}
			if (attackAnimation == "jumpCombo_3")
			{
				if (base.animation["attack_" + attackAnimation].normalizedTime >= 1f && IsGrounded())
				{
					attack("jumpCombo_4");
				}
			}
			else
			{
				if (!(base.animation["attack_" + attackAnimation].normalizedTime >= 1f))
				{
					break;
				}
				if (nextAttackAnimation != null)
				{
					attack(nextAttackAnimation);
					if (eren != null)
					{
						base.gameObject.transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(eren.transform.position - base.transform.position).eulerAngles.y, 0f);
					}
				}
				else
				{
					findNearestHero();
					Idle();
				}
			}
			break;
		case "grab":
			if (base.animation["attack_grab_" + attackAnimation].normalizedTime >= attackCheckTimeA && base.animation["attack_grab_" + attackAnimation].normalizedTime <= attackCheckTimeB && grabbedTarget == null)
			{
				GameObject gameObject4 = checkIfHitHand(currentGrabHand);
				if (gameObject4 != null)
				{
					if (isGrabHandLeft)
					{
						eatSetL(gameObject4);
						grabbedTarget = gameObject4;
					}
					else
					{
						eatSet(gameObject4);
						grabbedTarget = gameObject4;
					}
				}
			}
			if (base.animation["attack_grab_" + attackAnimation].normalizedTime > attackCheckTime && (bool)grabbedTarget)
			{
				justEatHero(grabbedTarget, currentGrabHand);
				grabbedTarget = null;
			}
			if (base.animation["attack_grab_" + attackAnimation].normalizedTime >= 1f)
			{
				Idle();
			}
			break;
		case "turn":
			base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, desDeg, 0f), Time.deltaTime * Mathf.Abs(turnDeg) * 0.1f);
			if (base.animation[turnAnimation].normalizedTime >= 1f)
			{
				Idle();
			}
			break;
		case "chase":
			if ((eren == null || !(myDistance < 35f) || !attackTarget(myHero)) && (!(getNearestHeroDistance() < 50f) || UnityEngine.Random.Range(0, 100) >= 20 || !attackTarget(getNearestHero())) && myDistance < attackDistance - 15f)
			{
				Idle(UnityEngine.Random.Range(0.05f, 0.2f));
			}
			break;
		case "turn180":
			if (base.animation[turnAnimation].normalizedTime >= 1f)
			{
				Transform obj3 = base.gameObject.transform;
				float x4 = base.gameObject.transform.rotation.eulerAngles.x;
				float y5 = base.gameObject.transform.rotation.eulerAngles.y + 180f;
				obj3.rotation = Quaternion.Euler(x4, y5, base.gameObject.transform.rotation.eulerAngles.z);
				Idle();
				PlayAnimation("idle");
			}
			break;
		case "anklehurt":
			if (base.animation["legHurt"].normalizedTime >= 1f)
			{
				CrossFade("legHurt_loop", 0.2f);
			}
			if (base.animation["legHurt_loop"].normalizedTime >= 3f)
			{
				CrossFade("legHurt_getup", 0.2f);
			}
			if (base.animation["legHurt_getup"].normalizedTime >= 1f)
			{
				Idle();
				PlayAnimation("idle");
			}
			break;
		}
	}

	private void playSound(string sndname)
	{
		playsoundRPC(sndname);
		if (Network.peerType == NetworkPeerType.Server)
		{
			base.photonView.RPC("playsoundRPC", PhotonTargets.Others, sndname);
		}
	}

	[RPC]
	private void playsoundRPC(string sndname)
	{
		base.transform.Find(sndname).GetComponent<AudioSource>().Play();
	}

	private void FixedUpdate()
	{
		if ((IN_GAME_MAIN_CAMERA.IsPausing && IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer) || (IN_GAME_MAIN_CAMERA.Gametype != 0 && !base.photonView.isMine))
		{
			return;
		}
		if (bottomObject.GetComponent<CheckHitGround>().isGrounded)
		{
			grounded = true;
			bottomObject.GetComponent<CheckHitGround>().isGrounded = false;
		}
		else
		{
			grounded = false;
		}
		if (needFreshCorePosition)
		{
			oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
			needFreshCorePosition = false;
		}
		switch (state)
		{
		case "attack":
		case "hit":
		case "turn180":
		case "anklehurt":
			if (state != "attack" || isAttackMoveByCore)
			{
				Vector3 vector3 = base.transform.position - base.transform.Find("Amarture/Core").position - oldCorePosition;
				Rigidbody rigidbody = base.rigidbody;
				Vector3 vector4 = vector3 / Time.deltaTime;
				Vector3 up = Vector3.up;
				rigidbody.velocity = vector4 + up * base.rigidbody.velocity.y;
				oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
			}
			break;
		case "chase":
		{
			if (myHero == null)
			{
				return;
			}
			Vector3 vector = base.transform.forward * speed;
			Vector3 velocity = base.rigidbody.velocity;
			Vector3 force = vector - velocity;
			force.y = 0f;
			base.rigidbody.AddForce(force, ForceMode.VelocityChange);
			Vector3 vector2 = myHero.transform.position - base.transform.position;
			float current = (0f - Mathf.Atan2(vector2.z, vector2.x)) * 57.29578f;
			float num = 0f - Mathf.DeltaAngle(current, base.gameObject.transform.rotation.eulerAngles.y - 90f);
			Transform obj2 = base.gameObject.transform;
			Quaternion rotation = base.gameObject.transform.rotation;
			obj2.rotation = Quaternion.Lerp(rotation, Quaternion.Euler(0f, base.gameObject.transform.rotation.eulerAngles.y + num, 0f), speed * Time.deltaTime);
			break;
		}
		default:
			if (grounded && !base.animation.IsPlaying("attack_jumpCombo_1"))
			{
				Rigidbody obj = base.rigidbody;
				float x = 0f - base.rigidbody.velocity.x;
				obj.AddForce(new Vector3(x, 0f, 0f - base.rigidbody.velocity.z), ForceMode.VelocityChange);
			}
			break;
		}
		base.rigidbody.AddForce(new Vector3(0f, (0f - gravity) * base.rigidbody.mass, 0f));
	}

	private void attack(string type)
	{
		state = "attack";
		attacked = false;
		if (attackAnimation == type)
		{
			attackAnimation = type;
			PlayAnimationAt("attack_" + type, 0f);
		}
		else
		{
			attackAnimation = type;
			PlayAnimationAt("attack_" + type, 0f);
		}
		startJump = false;
		attackChkOnce = false;
		nextAttackAnimation = null;
		fxName = null;
		isAttackMoveByCore = false;
		attackCheckTime = 0f;
		attackCheckTimeA = 0f;
		attackCheckTimeB = 0f;
		fxRotation = Quaternion.Euler(270f, 0f, 0f);
		switch (type)
		{
		case "combo_1":
			attackCheckTimeA = 0.63f;
			attackCheckTimeB = 0.8f;
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_R/shin_R/foot_R");
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_R");
			checkHitCapsuleR = 5f;
			isAttackMoveByCore = true;
			nextAttackAnimation = "combo_2";
			break;
		case "combo_2":
			attackCheckTimeA = 0.27f;
			attackCheckTimeB = 0.43f;
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_L/shin_L/foot_L");
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_L");
			checkHitCapsuleR = 5f;
			isAttackMoveByCore = true;
			nextAttackAnimation = "combo_3";
			break;
		case "combo_3":
			attackCheckTimeA = 0.15f;
			attackCheckTimeB = 0.3f;
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_R/shin_R/foot_R");
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_R");
			checkHitCapsuleR = 5f;
			isAttackMoveByCore = true;
			break;
		case "combo_blind_1":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.72f;
			attackCheckTimeB = 0.83f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
			checkHitCapsuleR = 4f;
			nextAttackAnimation = "combo_blind_2";
			break;
		case "combo_blind_2":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.5f;
			attackCheckTimeB = 0.6f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
			checkHitCapsuleR = 4f;
			nextAttackAnimation = "combo_blind_3";
			break;
		case "combo_blind_3":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.2f;
			attackCheckTimeB = 0.28f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
			checkHitCapsuleR = 4f;
			break;
		case "front":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.44f;
			attackCheckTimeB = 0.55f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
			checkHitCapsuleR = 4f;
			break;
		case "jumpCombo_1":
			isAttackMoveByCore = false;
			nextAttackAnimation = "jumpCombo_2";
			abnorma_jump_bite_horizon_v = Vector3.zero;
			break;
		case "jumpCombo_2":
			isAttackMoveByCore = false;
			attackCheckTimeA = 0.48f;
			attackCheckTimeB = 0.7f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
			checkHitCapsuleR = 4f;
			nextAttackAnimation = "jumpCombo_3";
			break;
		case "jumpCombo_3":
			isAttackMoveByCore = false;
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_L/shin_L/foot_L");
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_L");
			checkHitCapsuleR = 5f;
			attackCheckTimeA = 0.22f;
			attackCheckTimeB = 0.42f;
			break;
		case "jumpCombo_4":
			isAttackMoveByCore = false;
			break;
		case "sweep":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.39f;
			attackCheckTimeB = 0.6f;
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_R/shin_R/foot_R");
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_R");
			checkHitCapsuleR = 5f;
			break;
		case "sweep_back":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.41f;
			attackCheckTimeB = 0.48f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
			checkHitCapsuleR = 4f;
			break;
		case "sweep_front_left":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.53f;
			attackCheckTimeB = 0.63f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
			checkHitCapsuleR = 4f;
			break;
		case "sweep_front_right":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.5f;
			attackCheckTimeB = 0.62f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
			checkHitCapsuleR = 4f;
			break;
		case "sweep_head_b_l":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.4f;
			attackCheckTimeB = 0.51f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
			checkHitCapsuleR = 4f;
			break;
		case "sweep_head_b_r":
			isAttackMoveByCore = true;
			attackCheckTimeA = 0.4f;
			attackCheckTimeB = 0.51f;
			checkHitCapsuleStart = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
			checkHitCapsuleEnd = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
			checkHitCapsuleR = 4f;
			break;
		}
		checkHitCapsuleEndOld = checkHitCapsuleEnd.transform.position;
		needFreshCorePosition = true;
	}

	private void grab(string type)
	{
		state = "grab";
		attacked = false;
		attackAnimation = type;
		if (base.animation.IsPlaying("attack_grab_" + type))
		{
			base.animation["attack_grab_" + type].normalizedTime = 0f;
			PlayAnimation("attack_grab_" + type);
		}
		else
		{
			CrossFade("attack_grab_" + type, 0.1f);
		}
		isGrabHandLeft = true;
		grabbedTarget = null;
		attackCheckTime = 0f;
		switch (type)
		{
		case "bottom_left":
			attackCheckTimeA = 0.28f;
			attackCheckTimeB = 0.38f;
			attackCheckTime = 0.65f;
			isGrabHandLeft = false;
			break;
		case "bottom_right":
			attackCheckTimeA = 0.27f;
			attackCheckTimeB = 0.37f;
			attackCheckTime = 0.65f;
			break;
		case "mid_left":
			attackCheckTimeA = 0.27f;
			attackCheckTimeB = 0.37f;
			attackCheckTime = 0.65f;
			isGrabHandLeft = false;
			break;
		case "mid_right":
			attackCheckTimeA = 0.27f;
			attackCheckTimeB = 0.36f;
			attackCheckTime = 0.66f;
			break;
		case "up":
			attackCheckTimeA = 0.25f;
			attackCheckTimeB = 0.32f;
			attackCheckTime = 0.67f;
			break;
		case "up_left":
			attackCheckTimeA = 0.26f;
			attackCheckTimeB = 0.4f;
			attackCheckTime = 0.66f;
			break;
		case "up_right":
			attackCheckTimeA = 0.26f;
			attackCheckTimeB = 0.4f;
			attackCheckTime = 0.66f;
			isGrabHandLeft = false;
			break;
		}
		if (isGrabHandLeft)
		{
			currentGrabHand = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
		}
		else
		{
			currentGrabHand = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
		}
	}

	private void eatSet(GameObject grabTarget)
	{
		if (!grabTarget.GetComponent<HERO>().isGrabbed)
		{
			grabToRight();
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
			{
				grabTarget.GetPhotonView().RPC("netGrabbed", PhotonTargets.All, base.photonView.viewID, false);
				grabTarget.GetPhotonView().RPC("netPlayAnimation", PhotonTargets.All, "grabbed");
				base.photonView.RPC("grabToRight", PhotonTargets.Others);
			}
			else
			{
				grabTarget.GetComponent<HERO>().GetGrabbed(base.gameObject, leftHand: false);
				grabTarget.GetComponent<HERO>().animation.Play("grabbed");
			}
		}
	}

	[RPC]
	public void grabToRight()
	{
		Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
		grabTF.transform.parent = transform;
		grabTF.transform.position = transform.GetComponent<SphereCollider>().transform.position;
		grabTF.transform.rotation = transform.GetComponent<SphereCollider>().transform.rotation;
		grabTF.transform.localPosition -= Vector3.right * transform.GetComponent<SphereCollider>().radius * 0.3f;
		grabTF.transform.localPosition += Vector3.up * transform.GetComponent<SphereCollider>().radius * 0.51f;
		grabTF.transform.localPosition -= Vector3.forward * transform.GetComponent<SphereCollider>().radius * 0.3f;
		Transform obj = grabTF.transform;
		float x = grabTF.transform.localRotation.eulerAngles.x;
		float y = grabTF.transform.localRotation.eulerAngles.y + 180f;
		obj.localRotation = Quaternion.Euler(x, y, grabTF.transform.localRotation.eulerAngles.z);
	}

	[RPC]
	public void grabToLeft()
	{
		Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
		grabTF.transform.parent = transform;
		grabTF.transform.parent = transform;
		grabTF.transform.position = transform.GetComponent<SphereCollider>().transform.position;
		grabTF.transform.rotation = transform.GetComponent<SphereCollider>().transform.rotation;
		grabTF.transform.localPosition -= Vector3.right * transform.GetComponent<SphereCollider>().radius * 0.3f;
		grabTF.transform.localPosition -= Vector3.up * transform.GetComponent<SphereCollider>().radius * 0.51f;
		grabTF.transform.localPosition -= Vector3.forward * transform.GetComponent<SphereCollider>().radius * 0.3f;
		Transform obj = grabTF.transform;
		float x = grabTF.transform.localRotation.eulerAngles.x;
		float y = grabTF.transform.localRotation.eulerAngles.y + 180f;
		obj.localRotation = Quaternion.Euler(x, y, grabTF.transform.localRotation.eulerAngles.z + 180f);
	}

	private void eatSetL(GameObject grabTarget)
	{
		if (!grabTarget.GetComponent<HERO>().isGrabbed)
		{
			grabToLeft();
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
			{
				grabTarget.GetPhotonView().RPC("netGrabbed", PhotonTargets.All, base.photonView.viewID, true);
				grabTarget.GetPhotonView().RPC("netPlayAnimation", PhotonTargets.All, "grabbed");
				base.photonView.RPC("grabToLeft", PhotonTargets.Others);
			}
			else
			{
				grabTarget.GetComponent<HERO>().GetGrabbed(base.gameObject, leftHand: true);
				grabTarget.GetComponent<HERO>().animation.Play("grabbed");
			}
		}
	}

	[RPC]
	public void grabbedTargetEscape()
	{
		grabbedTarget = null;
	}

	private void justEatHero(GameObject target, Transform hand)
	{
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
		{
			if (!target.GetComponent<HERO>().HasDied())
			{
				target.GetComponent<HERO>().MarkDead();
				target.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, -1, "Female Titan");
			}
		}
		else if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
		{
			target.GetComponent<HERO>().Die2(hand);
		}
	}

	private void chase()
	{
		state = "chase";
		CrossFade("run", 0.5f);
	}

	private void Idle(float sbtime = 0f)
	{
		this.sbtime = sbtime;
		this.sbtime = Mathf.Max(0.5f, this.sbtime);
		state = "idle";
		CrossFade("idle", 0.2f);
	}

	private void turn(float d)
	{
		if (d > 0f)
		{
			turnAnimation = "turnaround1";
		}
		else
		{
			turnAnimation = "turnaround2";
		}
		PlayAnimation(turnAnimation);
		base.animation[turnAnimation].time = 0f;
		d = Mathf.Clamp(d, -120f, 120f);
		turnDeg = d;
		desDeg = base.gameObject.transform.rotation.eulerAngles.y + turnDeg;
		state = "turn";
	}

	private void turn180()
	{
		turnAnimation = "turn180";
		PlayAnimation(turnAnimation);
		base.animation[turnAnimation].time = 0f;
		state = "turn180";
		needFreshCorePosition = true;
	}

	public void hit(int dmg)
	{
		NapeArmor -= dmg;
		if (NapeArmor <= 0)
		{
			NapeArmor = 0;
		}
	}

	public void hitAnkleL(int dmg)
	{
		if (!hasDie && !(state == "anklehurt"))
		{
			AnkleLHP -= dmg;
			if (AnkleLHP <= 0)
			{
				getDown();
			}
		}
	}

	public void hitAnkleR(int dmg)
	{
		if (!hasDie && !(state == "anklehurt"))
		{
			AnkleRHP -= dmg;
			if (AnkleRHP <= 0)
			{
				getDown();
			}
		}
	}

	[RPC]
	public void hitAnkleLRPC(int viewID, int dmg)
	{
		if (hasDie || state == "anklehurt")
		{
			return;
		}
		PhotonView photonView = PhotonView.Find(viewID);
		if (photonView == null)
		{
			return;
		}
		if (grabbedTarget != null)
		{
			grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
		}
		if ((photonView.gameObject.transform.position - base.transform.Find("Amarture/Core/Controller_Body").transform.position).magnitude < 20f)
		{
			AnkleLHP -= dmg;
			if (AnkleLHP <= 0)
			{
				getDown();
			}
			FengGameManagerMKII.Instance.SendKillInfo(isKillerTitan: false, (string)photonView.owner.customProperties[PhotonPlayerProperty.Name], isVictimTitan: true, "Female Titan's ankle", dmg);
			FengGameManagerMKII.Instance.photonView.RPC("netShowDamage", photonView.owner, dmg);
		}
	}

	[RPC]
	public void hitAnkleRRPC(int viewID, int dmg)
	{
		if (hasDie || state == "anklehurt")
		{
			return;
		}
		PhotonView photonView = PhotonView.Find(viewID);
		if (photonView == null)
		{
			return;
		}
		if (grabbedTarget != null)
		{
			grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
		}
		if ((photonView.gameObject.transform.position - base.transform.Find("Amarture/Core/Controller_Body").transform.position).magnitude < 20f)
		{
			AnkleRHP -= dmg;
			if (AnkleRHP <= 0)
			{
				getDown();
			}
			FengGameManagerMKII.Instance.SendKillInfo(isKillerTitan: false, (string)photonView.owner.customProperties[PhotonPlayerProperty.Name], isVictimTitan: true, "Female Titan's ankle", dmg);
			FengGameManagerMKII.Instance.photonView.RPC("netShowDamage", photonView.owner, dmg);
		}
	}

	private void getDown()
	{
		state = "anklehurt";
		PlayAnimation("legHurt");
		AnkleRHP = AnkleRHPMAX;
		AnkleLHP = AnkleLHPMAX;
		needFreshCorePosition = true;
	}

	private void justHitEye()
	{
		attack("combo_blind_1");
	}

	public void hitEye()
	{
		if (!hasDie)
		{
			justHitEye();
		}
	}

	[RPC]
	public void hitEyeRPC(int viewID)
	{
		if (!hasDie)
		{
			if (grabbedTarget != null)
			{
				grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
			}
			Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
			PhotonView photonView = PhotonView.Find(viewID);
			if (!(photonView == null) && (photonView.gameObject.transform.position - transform.transform.position).magnitude < 20f)
			{
				justHitEye();
			}
		}
	}

	[RPC]
	public void titanGetHit(int viewID, int speed)
	{
		Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
		PhotonView photonView = PhotonView.Find(viewID);
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
		if (NapeArmor <= 0)
		{
			NapeArmor = 0;
			if (!hasDie)
			{
				base.photonView.RPC("netDie", PhotonTargets.OthersBuffered);
				if (grabbedTarget != null)
				{
					grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
				}
				netDie();
				FengGameManagerMKII.Instance.titanGetKill(photonView.owner, speed, base.name);
			}
		}
		else
		{
			FengGameManagerMKII.Instance.SendKillInfo(isKillerTitan: false, (string)photonView.owner.customProperties[PhotonPlayerProperty.Name], isVictimTitan: true, "Female Titan's neck", speed);
			FengGameManagerMKII.Instance.photonView.RPC("netShowDamage", photonView.owner, speed);
		}
		healthTime = 0.2f;
	}

	[RPC]
	public void netDie()
	{
		if (!hasDie)
		{
			hasDie = true;
			CrossFade("die", 0.05f);
		}
	}

	public void lateUpdate2()
	{
		if (IN_GAME_MAIN_CAMERA.IsPausing && IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
		{
			return;
		}
		if (base.animation.IsPlaying("run"))
		{
			if (base.animation["run"].normalizedTime % 1f > 0.1f && base.animation["run"].normalizedTime % 1f < 0.6f && stepSoundPhase == 2)
			{
				stepSoundPhase = 1;
				Transform obj = base.transform.Find("snd_titan_foot");
				obj.GetComponent<AudioSource>().Stop();
				obj.GetComponent<AudioSource>().Play();
			}
			if (base.animation["run"].normalizedTime % 1f > 0.6f && stepSoundPhase == 1)
			{
				stepSoundPhase = 2;
				Transform obj2 = base.transform.Find("snd_titan_foot");
				obj2.GetComponent<AudioSource>().Stop();
				obj2.GetComponent<AudioSource>().Play();
			}
		}
		UpdateLabel();
		healthTime -= Time.deltaTime;
	}

	private void Start()
	{
		StartMain();
		size = 4f;
		if (Minimap.Instance != null)
		{
			Minimap.Instance.TrackGameObjectOnMinimap(base.gameObject, Color.black, trackOrientation: true, depthAboveAll: true);
		}
		if (base.photonView.isMine)
		{
			if (RCSettings.SizeMode > 0)
			{
				float sizeLower = RCSettings.SizeLower;
				float sizeUpper = RCSettings.SizeUpper;
				size = UnityEngine.Random.Range(sizeLower, sizeUpper);
				base.photonView.RPC("setSize", PhotonTargets.AllBuffered, size);
			}
			lagMax = 150f + size * 3f;
			healthTime = 0f;
			maxHealth = NapeArmor;
			if (RCSettings.HealthMode > 0)
			{
				maxHealth = (NapeArmor = UnityEngine.Random.Range(RCSettings.HealthLower, RCSettings.HealthUpper));
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
		size = Mathf.Clamp(size, 0.2f, 30f);
		if (info.sender.isMasterClient)
		{
			base.transform.localScale *= size * 0.25f;
			this.size = size;
		}
	}

	public void LoadSkin()
	{
		if ((int)FengGameManagerMKII.Settings[1] == 1)
		{
			base.photonView.RPC("loadskinRPC", PhotonTargets.AllBuffered, (string)FengGameManagerMKII.Settings[66]);
		}
	}

	[RPC]
	public void loadskinRPC(string url)
	{
		if ((int)FengGameManagerMKII.Settings[1] == 1 && (url.EndsWith(".jpg") || url.EndsWith(".png") || url.EndsWith(".jpeg")))
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
				UnityEngine.Object.Destroy(healthLabel);
			}
			return;
		}
		if (healthLabel == null)
		{
			healthLabel = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("UI/LabelNameOverHead"));
			healthLabel.name = "LabelNameOverHead";
			healthLabel.transform.parent = base.transform;
			healthLabel.transform.localPosition = new Vector3(0f, 52f, 0f);
			float num = 4f;
			if (size > 0f && size < 1f)
			{
				num = 4f / size;
				num = Mathf.Min(num, 15f);
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
		healthLabel.GetComponent<UILabel>().text = text + Convert.ToString(health);
	}
}
