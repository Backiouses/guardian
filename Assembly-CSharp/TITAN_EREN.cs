using System;
using System.Collections;
using Guardian.AntiAbuse.Validators;
using Photon;
using UnityEngine;

public class TITAN_EREN : Photon.MonoBehaviour
{
	public FengCustomInputs inputManager;

	public Camera currentCamera;

	public float speed = 80f;

	private float gravity = 500f;

	public float maxVelocityChange = 100f;

	public bool canJump = true;

	public float jumpHeight = 2f;

	private bool grounded;

	private float facingDirection;

	private bool justGrounded;

	private bool isAttack;

	public bool isHit;

	private bool isNextAttack;

	private string attackAnimation;

	public bool hasDied;

	public GameObject bottomObject;

	private Vector3 oldCorePosition;

	private Transform attackBox;

	private ArrayList hitTargets;

	private bool attackChkOnce;

	private float hitPause;

	public GameObject realBody;

	public float lifeTime = 9999f;

	private float lifeTimeMax = 9999f;

	public bool rockLift;

	private bool hasDieSteam;

	private float dieTime;

	private int stepSoundPhase = 2;

	private bool isPlayRoar;

	private bool needFreshCorePosition;

	private bool needRoar;

	private string hitAnimation;

	private int rockPhase;

	private float waitCounter;

	private Vector3 targetCheckPt;

	private ArrayList checkPoints = new ArrayList();

	public GameObject rock;

	private bool isROCKMOVE;

	private bool rockHitGround;

	private bool isHitWhileCarryingRock;

	public bool hasSpawn;

	private GameObject mainCamera;

	private void OnDestroy()
	{
		GameObject gameObject = GameObject.Find("MultiplayerManager");
		if (gameObject != null)
		{
			gameObject.GetComponent<FengGameManagerMKII>().RemoveEren(this);
		}
	}

	public bool IsGrounded()
	{
		return bottomObject.GetComponent<CheckHitGround>().isGrounded;
	}

	public void PlayAnimation(string aniName)
	{
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && base.photonView.isMine)
		{
			base.photonView.RPC("netPlayAnimation", PhotonTargets.Others, aniName);
		}
		LocalPlayAnimation(aniName);
	}

	private void PlayAnimationAt(string aniName, float normalizedTime)
	{
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && base.photonView.isMine)
		{
			base.photonView.RPC("netPlayAnimationAt", PhotonTargets.Others, aniName, normalizedTime);
		}
		LocalPlayAnimationAt(aniName, normalizedTime);
	}

	private void CrossFade(string aniName, float time)
	{
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && base.photonView.isMine)
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
		if (ErenChecker.IsAnimationPlayValid(this, info))
		{
			LocalPlayAnimation(aniName);
		}
	}

	[RPC]
	private void netPlayAnimationAt(string aniName, float normalizedTime, PhotonMessageInfo info)
	{
		if (ErenChecker.IsAnimationSeekedPlayValid(this, info))
		{
			LocalPlayAnimationAt(aniName, normalizedTime);
		}
	}

	[RPC]
	private void netCrossFade(string aniName, float time, PhotonMessageInfo info)
	{
		if (ErenChecker.IsCrossFadeValid(this, info))
		{
			LocalCrossFade(aniName, time);
		}
	}

	[RPC]
	private void removeMe(PhotonMessageInfo info)
	{
		if (ErenChecker.IsRemovalValid(info))
		{
			PhotonNetwork.RemoveRPCs(base.photonView);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void update()
	{
		if ((IN_GAME_MAIN_CAMERA.IsPausing && IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer) || rockLift)
		{
			return;
		}
		if (base.animation.IsPlaying("run"))
		{
			if (base.animation["run"].normalizedTime % 1f > 0.3f && base.animation["run"].normalizedTime % 1f < 0.75f && stepSoundPhase == 2)
			{
				stepSoundPhase = 1;
				Transform obj = base.transform.Find("snd_eren_foot");
				obj.GetComponent<AudioSource>().Stop();
				obj.GetComponent<AudioSource>().Play();
			}
			if (base.animation["run"].normalizedTime % 1f > 0.75f && stepSoundPhase == 1)
			{
				stepSoundPhase = 2;
				Transform obj2 = base.transform.Find("snd_eren_foot");
				obj2.GetComponent<AudioSource>().Stop();
				obj2.GetComponent<AudioSource>().Play();
			}
		}
		if (IN_GAME_MAIN_CAMERA.Gametype != 0 && !base.photonView.isMine)
		{
			return;
		}
		if (hasDied)
		{
			if (!(base.animation["die"].normalizedTime >= 1f) && !(hitAnimation == "hit_annie_3"))
			{
				return;
			}
			if (realBody != null)
			{
				realBody.GetComponent<HERO>().backToHuman();
				realBody.transform.position = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position + Vector3.up * 2f;
				realBody = null;
			}
			dieTime += Time.deltaTime;
			if (dieTime > 2f && !hasDieSteam)
			{
				hasDieSteam = true;
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
				{
					GameObject obj3 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/FXtitanDie1"));
					obj3.transform.position = base.transform.Find("Amarture/Core/Controller_Body/hip").position;
					obj3.transform.localScale = base.transform.localScale;
				}
				else if (base.photonView.isMine)
				{
					PhotonNetwork.Instantiate("FX/FXtitanDie1", base.transform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0).transform.localScale = base.transform.localScale;
				}
			}
			if (dieTime > 5f)
			{
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
				{
					GameObject obj4 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/FXtitanDie"));
					obj4.transform.position = base.transform.Find("Amarture/Core/Controller_Body/hip").position;
					obj4.transform.localScale = base.transform.localScale;
					UnityEngine.Object.Destroy(base.gameObject);
				}
				else if (base.photonView.isMine)
				{
					PhotonNetwork.Instantiate("FX/FXtitanDie", base.transform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0).transform.localScale = base.transform.localScale;
					PhotonNetwork.Destroy(base.photonView);
				}
			}
		}
		else
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != 0 && !base.photonView.isMine)
			{
				return;
			}
			if (isHit)
			{
				if (base.animation[hitAnimation].normalizedTime >= 1f)
				{
					isHit = false;
					falseAttack();
					PlayAnimation("idle");
				}
				return;
			}
			if (lifeTime > 0f)
			{
				lifeTime -= Time.deltaTime;
				if (lifeTime <= 0f)
				{
					hasDied = true;
					PlayAnimation("die");
					return;
				}
			}
			if (grounded && !isAttack && !base.animation.IsPlaying("jump_land") && !isAttack && !base.animation.IsPlaying("born"))
			{
				if (inputManager.isInputDown[InputCode.Attack0] || inputManager.isInputDown[InputCode.Attack1])
				{
					bool flag = false;
					if ((IN_GAME_MAIN_CAMERA.CameraMode == CameraType.WOW && inputManager.isInput[InputCode.Down]) || inputManager.isInputDown[InputCode.Attack1])
					{
						if (IN_GAME_MAIN_CAMERA.CameraMode == CameraType.WOW && inputManager.isInputDown[InputCode.Attack1] && inputManager.inputKey[11] == KeyCode.Mouse1)
						{
							flag = true;
						}
						if (!flag)
						{
							attackAnimation = "attack_kick";
						}
					}
					else
					{
						attackAnimation = "attack_combo_001";
					}
					if (!flag)
					{
						PlayAnimation(attackAnimation);
						base.animation[attackAnimation].time = 0f;
						isAttack = true;
						needFreshCorePosition = true;
						if (attackAnimation == "attack_combo_001" || attackAnimation == "attack_combo_001")
						{
							attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R");
						}
						else if (attackAnimation == "attack_combo_002")
						{
							attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L");
						}
						else if (attackAnimation == "attack_kick")
						{
							attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_R/shin_R/foot_R");
						}
						hitTargets = new ArrayList();
					}
				}
				if (inputManager.isInputDown[InputCode.Salute])
				{
					CrossFade("born", 0.1f);
					base.animation["born"].normalizedTime = 0.28f;
					isPlayRoar = false;
				}
			}
			if (!isAttack)
			{
				if ((grounded || base.animation.IsPlaying("idle")) && !base.animation.IsPlaying("jump_start") && !base.animation.IsPlaying("jump_air") && !base.animation.IsPlaying("jump_land") && inputManager.isInput[InputCode.HookBoth])
				{
					CrossFade("jump_start", 0.1f);
				}
			}
			else
			{
				if (base.animation[attackAnimation].time >= 0.1f && inputManager.isInputDown[InputCode.Attack0])
				{
					isNextAttack = true;
				}
				float num = 0f;
				float num2 = 0f;
				float num3 = 0f;
				string text = string.Empty;
				switch (attackAnimation)
				{
				case "attack_combo_001":
					num = 0.4f;
					num2 = 0.5f;
					num3 = 0.66f;
					text = "attack_combo_002";
					break;
				case "attack_combo_002":
					num = 0.15f;
					num2 = 0.25f;
					num3 = 0.43f;
					text = "attack_combo_003";
					break;
				case "attack_combo_003":
					num3 = 0f;
					num = 0.31f;
					num2 = 0.37f;
					break;
				case "attack_kick":
					num3 = 0f;
					num = 0.32f;
					num2 = 0.38f;
					break;
				default:
					num = 0.5f;
					num2 = 0.85f;
					break;
				}
				if (hitPause > 0f)
				{
					hitPause -= Time.deltaTime;
					if (hitPause <= 0f)
					{
						base.animation[attackAnimation].speed = 1f;
						hitPause = 0f;
					}
				}
				if (num3 > 0f && isNextAttack && base.animation[attackAnimation].normalizedTime >= num3)
				{
					if (hitTargets.Count > 0)
					{
						Transform transform = (Transform)hitTargets[0];
						if ((bool)transform)
						{
							base.transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(transform.position - base.transform.position).eulerAngles.y, 0f);
							facingDirection = base.transform.rotation.eulerAngles.y;
						}
					}
					falseAttack();
					attackAnimation = text;
					CrossFade(attackAnimation, 0.1f);
					base.animation[attackAnimation].time = 0f;
					base.animation[attackAnimation].speed = 1f;
					isAttack = true;
					needFreshCorePosition = true;
					if (attackAnimation == "attack_combo_002")
					{
						attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L");
					}
					else if (attackAnimation == "attack_combo_003")
					{
						attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R");
					}
					hitTargets = new ArrayList();
				}
				if ((base.animation[attackAnimation].normalizedTime >= num && base.animation[attackAnimation].normalizedTime <= num2) || (!attackChkOnce && base.animation[attackAnimation].normalizedTime >= num))
				{
					if (!attackChkOnce)
					{
						switch (attackAnimation)
						{
						case "attack_combo_001":
							playSound("snd_eren_swing1");
							break;
						case "attack_combo_002":
							playSound("snd_eren_swing2");
							break;
						case "attack_combo_003":
							playSound("snd_eren_swing3");
							break;
						}
						attackChkOnce = true;
					}
					Collider[] array = Physics.OverlapSphere(attackBox.transform.position, 8f);
					for (int i = 0; i < array.Length; i++)
					{
						if (!array[i].gameObject.transform.root.GetComponent<TITAN>())
						{
							continue;
						}
						bool flag2 = false;
						for (int j = 0; j < hitTargets.Count; j++)
						{
							if (array[i].gameObject.transform.root == hitTargets[j])
							{
								flag2 = true;
								break;
							}
						}
						if (flag2 || array[i].gameObject.transform.root.GetComponent<TITAN>().hasDie)
						{
							continue;
						}
						base.animation[attackAnimation].speed = 0f;
						switch (attackAnimation)
						{
						case "attack_combo_001":
							currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().StartShake(1.2f, 0.04f);
							hitPause = 0.08f;
							array[i].gameObject.transform.root.GetComponent<TITAN>().HitRight(base.transform.position, hitPause);
							break;
						case "attack_combo_002":
							hitPause = 0.05f;
							array[i].gameObject.transform.root.GetComponent<TITAN>().HitLeft(base.transform.position, hitPause);
							currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().StartShake(1f, 0.03f);
							break;
						case "attack_combo_003":
							currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().StartShake(3f, 0.1f);
							hitPause = 0.3f;
							array[i].gameObject.transform.root.GetComponent<TITAN>().DieHeadBlow(base.transform.position, hitPause);
							break;
						case "attack_kick":
							currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().StartShake(3f, 0.1f);
							hitPause = 0.2f;
							if (array[i].gameObject.transform.root.GetComponent<TITAN>().abnormalType == TitanClass.Crawler)
							{
								array[i].gameObject.transform.root.GetComponent<TITAN>().DieBlow(base.transform.position, hitPause);
							}
							else if (array[i].gameObject.transform.root.transform.localScale.x < 2f)
							{
								array[i].gameObject.transform.root.GetComponent<TITAN>().DieBlow(base.transform.position, hitPause);
							}
							else
							{
								array[i].gameObject.transform.root.GetComponent<TITAN>().HitRight(base.transform.position, hitPause);
							}
							break;
						}
						hitTargets.Add(array[i].gameObject.transform.root);
						if (IN_GAME_MAIN_CAMERA.Gametype != 0)
						{
							PhotonNetwork.Instantiate("hitMeatBIG", (array[i].transform.position + attackBox.position) * 0.5f, Quaternion.Euler(270f, 0f, 0f), 0);
						}
						else
						{
							UnityEngine.Object.Instantiate(Resources.Load("hitMeatBIG"), (array[i].transform.position + attackBox.position) * 0.5f, Quaternion.Euler(270f, 0f, 0f));
						}
					}
				}
				if (base.animation[attackAnimation].normalizedTime >= 1f)
				{
					falseAttack();
					PlayAnimation("idle");
				}
			}
			if (base.animation.IsPlaying("jump_land") && base.animation["jump_land"].normalizedTime >= 1f)
			{
				CrossFade("idle", 0.1f);
			}
			if (base.animation.IsPlaying("born"))
			{
				if (base.animation["born"].normalizedTime >= 0.28f && !isPlayRoar)
				{
					isPlayRoar = true;
					playSound("snd_eren_roar");
				}
				if (base.animation["born"].normalizedTime >= 0.5f && base.animation["born"].normalizedTime <= 0.7f)
				{
					currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().StartShake(0.5f, 1f);
				}
				if (base.animation["born"].normalizedTime >= 1f)
				{
					CrossFade("idle", 0.1f);
					if (IN_GAME_MAIN_CAMERA.Gametype != 0)
					{
						if (PhotonNetwork.isMasterClient)
						{
							base.photonView.RPC("netTauntAttack", PhotonTargets.MasterClient, 10f, 500f);
						}
						else
						{
							netTauntAttack(10f, 500f);
						}
					}
					else
					{
						netTauntAttack(10f, 500f);
					}
				}
			}
			showAimUI();
			showSkillCD();
		}
	}

	private void showSkillCD()
	{
		GameObject.Find("skill_cd_eren").GetComponent<UISprite>().fillAmount = lifeTime / lifeTimeMax;
	}

	private void playSound(string sndname)
	{
		playsoundRPC(sndname);
		if (IN_GAME_MAIN_CAMERA.Gametype != 0)
		{
			base.photonView.RPC("playsoundRPC", PhotonTargets.Others, sndname);
		}
	}

	[RPC]
	private void playsoundRPC(string sndname)
	{
		base.transform.Find(sndname).GetComponent<AudioSource>().Play();
	}

	private void showAimUI()
	{
		GameObject gameObject = GameObject.Find("cross1");
		GameObject obj = GameObject.Find("cross2");
		GameObject obj2 = GameObject.Find("crossL1");
		GameObject obj3 = GameObject.Find("crossL2");
		GameObject obj4 = GameObject.Find("crossR1");
		GameObject obj5 = GameObject.Find("crossR2");
		GameObject gameObject2 = GameObject.Find("LabelDistance");
		Transform transform = gameObject.transform;
		Vector3 localPosition = Vector3.up * 10000f;
		obj5.transform.localPosition = localPosition;
		obj4.transform.localPosition = localPosition;
		obj3.transform.localPosition = localPosition;
		obj2.transform.localPosition = localPosition;
		gameObject2.transform.localPosition = localPosition;
		obj.transform.localPosition = localPosition;
		transform.localPosition = localPosition;
	}

	public void lateUpdate()
	{
		if ((!IN_GAME_MAIN_CAMERA.IsPausing || IN_GAME_MAIN_CAMERA.Gametype != 0) && !rockLift && (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer || base.photonView.isMine))
		{
			Vector3 eulerAngles = mainCamera.transform.rotation.eulerAngles;
			Quaternion to = Quaternion.Euler(eulerAngles.x, eulerAngles.y, 0f);
			mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, to, Time.deltaTime * 2f);
		}
	}

	[RPC]
	private void netTauntAttack(float tauntTime, float distance = 100f)
	{
		foreach (GameObject allTitan in FengGameManagerMKII.Instance.AllTitans)
		{
			if (Vector3.Distance(allTitan.transform.position, base.transform.position) < distance && (bool)allTitan.GetComponent<TITAN>())
			{
				allTitan.GetComponent<TITAN>().beTauntedBy(base.gameObject, tauntTime);
			}
			if ((bool)allTitan.GetComponent<FEMALE_TITAN>())
			{
				allTitan.GetComponent<FEMALE_TITAN>().erenIsHere(base.gameObject);
			}
		}
	}

	private void falseAttack()
	{
		isAttack = false;
		isNextAttack = false;
		hitTargets = new ArrayList();
		attackChkOnce = false;
	}

	private void FixedUpdate()
	{
		if (IN_GAME_MAIN_CAMERA.IsPausing && IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
		{
			return;
		}
		if (rockLift)
		{
			RockUpdate();
		}
		else
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != 0 && !base.photonView.isMine)
			{
				return;
			}
			if (hitPause > 0f)
			{
				base.rigidbody.velocity = Vector3.zero;
			}
			else if (hasDied)
			{
				Rigidbody obj = base.rigidbody;
				Vector3 zero = Vector3.zero;
				Vector3 up = Vector3.up;
				obj.velocity = zero + up * base.rigidbody.velocity.y;
				base.rigidbody.AddForce(new Vector3(0f, (0f - gravity) * base.rigidbody.mass, 0f));
			}
			else
			{
				if (IN_GAME_MAIN_CAMERA.Gametype != 0 && !base.photonView.isMine)
				{
					return;
				}
				if (base.rigidbody.velocity.magnitude > 50f)
				{
					currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(currentCamera.GetComponent<Camera>().fieldOfView, Mathf.Min(100f, base.rigidbody.velocity.magnitude), 0.1f);
				}
				else
				{
					currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(currentCamera.GetComponent<Camera>().fieldOfView, 50f, 0.1f);
				}
				if (bottomObject.GetComponent<CheckHitGround>().isGrounded)
				{
					if (!grounded)
					{
						justGrounded = true;
					}
					grounded = true;
					bottomObject.GetComponent<CheckHitGround>().isGrounded = false;
				}
				else
				{
					grounded = false;
				}
				float num = 0f;
				float num2 = 0f;
				if (!IN_GAME_MAIN_CAMERA.IsTyping)
				{
					num2 = (inputManager.isInput[InputCode.Up] ? 1f : ((!inputManager.isInput[InputCode.Down]) ? 0f : (-1f)));
					num = (inputManager.isInput[InputCode.Left] ? (-1f) : ((!inputManager.isInput[InputCode.Right]) ? 0f : 1f));
				}
				if (needFreshCorePosition)
				{
					oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
					needFreshCorePosition = false;
				}
				if (isAttack || isHit)
				{
					Vector3 vector = base.transform.position - base.transform.Find("Amarture/Core").position - oldCorePosition;
					oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
					Rigidbody obj2 = base.rigidbody;
					Vector3 vector2 = vector / Time.deltaTime;
					Vector3 up2 = Vector3.up;
					obj2.velocity = vector2 + up2 * base.rigidbody.velocity.y;
					base.rigidbody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, facingDirection, 0f), Time.deltaTime * 10f);
					if (justGrounded)
					{
						justGrounded = false;
					}
				}
				else if (grounded)
				{
					Vector3 vector3 = Vector3.zero;
					if (justGrounded)
					{
						justGrounded = false;
						vector3 = base.rigidbody.velocity;
						if (base.animation.IsPlaying("jump_air"))
						{
							((GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/boom2_eren"), base.transform.position, Quaternion.Euler(270f, 0f, 0f))).transform.localScale = Vector3.one * 1.5f;
							if (needRoar)
							{
								PlayAnimation("born");
								needRoar = false;
								isPlayRoar = false;
							}
							else
							{
								PlayAnimation("jump_land");
							}
						}
					}
					if (!base.animation.IsPlaying("jump_land") && !isAttack && !isHit && !base.animation.IsPlaying("born"))
					{
						Vector3 vector4 = new Vector3(num, 0f, num2);
						float y = currentCamera.transform.rotation.eulerAngles.y;
						float num3 = Mathf.Atan2(num2, num) * 57.29578f;
						num3 = 0f - num3 + 90f;
						float num4 = y + num3;
						float num5 = 0f - num4 + 90f;
						float x = Mathf.Cos(num5 * ((float)Math.PI / 180f));
						float z = Mathf.Sin(num5 * ((float)Math.PI / 180f));
						vector3 = new Vector3(x, 0f, z);
						float num6 = ((vector4.magnitude > 0.95f) ? 1f : ((!(vector4.magnitude < 0.25f)) ? vector4.magnitude : 0f));
						vector3 *= num6;
						vector3 *= speed;
						if (num != 0f || num2 != 0f)
						{
							if (!base.animation.IsPlaying("run") && !base.animation.IsPlaying("jump_start") && !base.animation.IsPlaying("jump_air"))
							{
								CrossFade("run", 0.1f);
							}
						}
						else
						{
							if (!base.animation.IsPlaying("idle") && !base.animation.IsPlaying("dash_land") && !base.animation.IsPlaying("dodge") && !base.animation.IsPlaying("jump_start") && !base.animation.IsPlaying("jump_air") && !base.animation.IsPlaying("jump_land"))
							{
								CrossFade("idle", 0.1f);
								vector3 *= 0f;
							}
							num4 = -874f;
						}
						if (num4 != -874f)
						{
							facingDirection = num4;
						}
					}
					Vector3 velocity = base.rigidbody.velocity;
					Vector3 force = vector3 - velocity;
					force.x = Mathf.Clamp(force.x, 0f - maxVelocityChange, maxVelocityChange);
					force.z = Mathf.Clamp(force.z, 0f - maxVelocityChange, maxVelocityChange);
					force.y = 0f;
					if (base.animation.IsPlaying("jump_start") && base.animation["jump_start"].normalizedTime >= 1f)
					{
						PlayAnimation("jump_air");
						force.y += 240f;
					}
					else if (base.animation.IsPlaying("jump_start"))
					{
						force = -base.rigidbody.velocity;
					}
					base.rigidbody.AddForce(force, ForceMode.VelocityChange);
					base.rigidbody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, facingDirection, 0f), Time.deltaTime * 10f);
				}
				else
				{
					if (base.animation.IsPlaying("jump_start") && base.animation["jump_start"].normalizedTime >= 1f)
					{
						PlayAnimation("jump_air");
						base.rigidbody.AddForce(Vector3.up * 240f, ForceMode.VelocityChange);
					}
					if (!base.animation.IsPlaying("jump") && !isHit)
					{
						Vector3 vector5 = new Vector3(num, 0f, num2);
						float y2 = currentCamera.transform.rotation.eulerAngles.y;
						float num7 = Mathf.Atan2(num2, num) * 57.29578f;
						num7 = 0f - num7 + 90f;
						float num8 = y2 + num7;
						float num9 = 0f - num8 + 90f;
						float x2 = Mathf.Cos(num9 * ((float)Math.PI / 180f));
						float z2 = Mathf.Sin(num9 * ((float)Math.PI / 180f));
						Vector3 force2 = new Vector3(x2, 0f, z2);
						float num10 = ((vector5.magnitude > 0.95f) ? 1f : ((!(vector5.magnitude < 0.25f)) ? vector5.magnitude : 0f));
						force2 *= num10;
						force2 *= speed * 2f;
						if (num != 0f || num2 != 0f)
						{
							base.rigidbody.AddForce(force2, ForceMode.Impulse);
						}
						else
						{
							num8 = -874f;
						}
						if (num8 != -874f)
						{
							facingDirection = num8;
						}
						if (!base.animation.IsPlaying(string.Empty) && !base.animation.IsPlaying("attack3_2") && !base.animation.IsPlaying("attack5"))
						{
							base.rigidbody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, facingDirection, 0f), Time.deltaTime * 6f);
						}
					}
				}
				base.rigidbody.AddForce(new Vector3(0f, (0f - gravity) * base.rigidbody.mass, 0f));
			}
		}
	}

	public void born()
	{
		foreach (FEMALE_TITAN anny in FengGameManagerMKII.Instance.Annies)
		{
			anny.erenIsHere(base.gameObject);
		}
		if (!bottomObject.GetComponent<CheckHitGround>().isGrounded)
		{
			PlayAnimation("jump_air");
			needRoar = true;
		}
		else
		{
			needRoar = false;
			PlayAnimation("born");
			isPlayRoar = false;
		}
		playSound("snd_eren_shift");
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
		{
			UnityEngine.Object.Instantiate(Resources.Load("FX/Thunder"), base.transform.position + Vector3.up * 23f, Quaternion.Euler(270f, 0f, 0f));
		}
		else if (base.photonView.isMine)
		{
			PhotonNetwork.Instantiate("FX/Thunder", base.transform.position + Vector3.up * 23f, Quaternion.Euler(270f, 0f, 0f), 0);
		}
		lifeTimeMax = (lifeTime = 30f);
	}

	public void HitByTitan()
	{
		if (!isHit && !hasDied && !base.animation.IsPlaying("born"))
		{
			if (rockLift)
			{
				CrossFade("die", 0.1f);
				isHitWhileCarryingRock = true;
				FengGameManagerMKII.Instance.FinishGame(isLoss: true);
				base.photonView.RPC("rockPlayAnimation", PhotonTargets.All, "set");
			}
			else
			{
				isHit = true;
				hitAnimation = "hit_titan";
				falseAttack();
				PlayAnimation(hitAnimation);
				needFreshCorePosition = true;
			}
		}
	}

	public void hitByFT(int phase)
	{
		if (hasDied)
		{
			return;
		}
		isHit = true;
		hitAnimation = "hit_annie_" + phase;
		falseAttack();
		PlayAnimation(hitAnimation);
		needFreshCorePosition = true;
		if (phase == 3)
		{
			hasDied = true;
			Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
			GameObject gameObject = ((IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || !PhotonNetwork.isMasterClient) ? ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("bloodExplore"), transform.position + Vector3.up * 1f * 4f, Quaternion.Euler(270f, 0f, 0f))) : PhotonNetwork.Instantiate("bloodExplore", transform.position + Vector3.up * 1f * 4f, Quaternion.Euler(270f, 0f, 0f), 0));
			gameObject.transform.localScale = base.transform.localScale;
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
			{
				Vector3 position = transform.position;
				float x = 90f + transform.rotation.eulerAngles.x;
				float y = transform.rotation.eulerAngles.y;
				gameObject = PhotonNetwork.Instantiate("bloodsplatter", position, Quaternion.Euler(x, y, transform.rotation.eulerAngles.z), 0);
			}
			else
			{
				UnityEngine.Object original = Resources.Load("bloodsplatter");
				Vector3 position2 = transform.position;
				float x2 = 90f + transform.rotation.eulerAngles.x;
				float y2 = transform.rotation.eulerAngles.y;
				gameObject = (GameObject)UnityEngine.Object.Instantiate(original, position2, Quaternion.Euler(x2, y2, transform.rotation.eulerAngles.z));
			}
			gameObject.transform.localScale = base.transform.localScale;
			gameObject.transform.parent = transform;
			gameObject = ((IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || !PhotonNetwork.isMasterClient) ? ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("FX/justSmoke"), transform.position, Quaternion.Euler(270f, 0f, 0f))) : PhotonNetwork.Instantiate("FX/justSmoke", transform.position, Quaternion.Euler(270f, 0f, 0f), 0));
			gameObject.transform.parent = transform;
		}
	}

	public void hitByFTByServer(int phase)
	{
		base.photonView.RPC("hitByFTRPC", PhotonTargets.All, phase);
	}

	[RPC]
	private void hitByFTRPC(int phase)
	{
		if (base.photonView.isMine)
		{
			hitByFT(phase);
		}
	}

	public void hitByTitanByServer()
	{
		base.photonView.RPC("hitByTitanRPC", PhotonTargets.All);
	}

	[RPC]
	private void hitByTitanRPC()
	{
		if (base.photonView.isMine)
		{
			HitByTitan();
		}
	}

	private void RockUpdate()
	{
		if (isHitWhileCarryingRock)
		{
			return;
		}
		if (isROCKMOVE)
		{
			rock.transform.position = base.transform.position;
			rock.transform.rotation = base.transform.rotation;
		}
		if (IN_GAME_MAIN_CAMERA.Gametype != 0 && !base.photonView.isMine)
		{
			return;
		}
		switch (rockPhase)
		{
		case 0:
			base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
			base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
			waitCounter += Time.deltaTime;
			if (waitCounter > 20f)
			{
				rockPhase++;
				CrossFade("idle", 1f);
				waitCounter = 0f;
				setRoute();
			}
			break;
		case 1:
			base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
			base.rigidbody.AddForce(new Vector3(0f, (0f - gravity) * base.rigidbody.mass, 0f));
			waitCounter += Time.deltaTime;
			if (waitCounter > 2f)
			{
				rockPhase++;
				CrossFade("run", 0.2f);
				waitCounter = 0f;
			}
			break;
		case 2:
		{
			Vector3 vector5 = base.transform.forward * 30f;
			Vector3 velocity2 = base.rigidbody.velocity;
			Vector3 force2 = vector5 - velocity2;
			force2.x = Mathf.Clamp(force2.x, 0f - maxVelocityChange, maxVelocityChange);
			force2.z = Mathf.Clamp(force2.z, 0f - maxVelocityChange, maxVelocityChange);
			force2.y = 0f;
			base.rigidbody.AddForce(force2, ForceMode.VelocityChange);
			if (base.transform.position.z < -238f)
			{
				base.transform.position = new Vector3(base.transform.position.x, 0f, -238f);
				rockPhase++;
				CrossFade("idle", 0.2f);
				waitCounter = 0f;
			}
			break;
		}
		case 3:
			base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
			base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
			waitCounter += Time.deltaTime;
			if (waitCounter > 1f)
			{
				rockPhase++;
				CrossFade("rock_lift", 0.1f);
				base.photonView.RPC("rockPlayAnimation", PhotonTargets.All, "lift");
				waitCounter = 0f;
				targetCheckPt = (Vector3)checkPoints[0];
			}
			break;
		case 4:
			base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
			base.rigidbody.AddForce(new Vector3(0f, (0f - gravity) * base.rigidbody.mass, 0f));
			waitCounter += Time.deltaTime;
			if (waitCounter > 4.2f)
			{
				rockPhase++;
				CrossFade("rock_walk", 0.1f);
				base.photonView.RPC("rockPlayAnimation", PhotonTargets.All, "move");
				rock.animation["move"].normalizedTime = base.animation["rock_walk"].normalizedTime;
				waitCounter = 0f;
				base.photonView.RPC("startMovingRock", PhotonTargets.All);
			}
			break;
		case 5:
		{
			if (Vector3.Distance(base.transform.position, targetCheckPt) < 10f)
			{
				if (checkPoints.Count > 0)
				{
					if (checkPoints.Count == 1)
					{
						rockPhase++;
					}
					else
					{
						targetCheckPt = (Vector3)checkPoints[0];
						checkPoints.RemoveAt(0);
						GameObject[] array = GameObject.FindGameObjectsWithTag("titanRespawn2");
						GameObject gameObject = GameObject.Find("titanRespawnTrost" + (7 - checkPoints.Count));
						if (gameObject != null)
						{
							GameObject[] array2 = array;
							foreach (GameObject gameObject2 in array2)
							{
								if (gameObject2.transform.parent.gameObject == gameObject)
								{
									TITAN component = FengGameManagerMKII.Instance.SpawnTitan(70, gameObject2.transform.position, gameObject2.transform.rotation).GetComponent<TITAN>();
									component.isAlarm = true;
									component.chaseDistance = 999999f;
								}
							}
						}
					}
				}
				else
				{
					rockPhase++;
				}
			}
			if (checkPoints.Count > 0 && UnityEngine.Random.Range(0, 3000) < 10 - checkPoints.Count)
			{
				Vector3 vector = ((UnityEngine.Random.Range(0, 10) <= 5) ? (base.transform.rotation * Quaternion.Euler(0f, UnityEngine.Random.Range(-30f, 30f), 0f)) : (base.transform.rotation * Quaternion.Euler(0f, UnityEngine.Random.Range(150f, 210f), 0f))) * new Vector3(UnityEngine.Random.Range(100f, 200f), 0f, 0f);
				Vector3 vector2 = base.transform.position + vector;
				LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
				float num = 0f;
				if (Physics.Raycast(vector2 + Vector3.up * 500f, -Vector3.up, out var hitInfo, 1000f, layerMask.value))
				{
					num = hitInfo.point.y;
				}
				vector2 += Vector3.up * num;
				TITAN component2 = FengGameManagerMKII.Instance.SpawnTitan(70, vector2, base.transform.rotation).GetComponent<TITAN>();
				component2.isAlarm = true;
				component2.chaseDistance = 999999f;
			}
			Vector3 vector3 = base.transform.forward * 6f;
			Vector3 velocity = base.rigidbody.velocity;
			Vector3 force = vector3 - velocity;
			force.x = Mathf.Clamp(force.x, 0f - maxVelocityChange, maxVelocityChange);
			force.z = Mathf.Clamp(force.z, 0f - maxVelocityChange, maxVelocityChange);
			force.y = 0f;
			base.rigidbody.AddForce(force, ForceMode.VelocityChange);
			base.rigidbody.AddForce(new Vector3(0f, (0f - gravity) * base.rigidbody.mass, 0f));
			Vector3 vector4 = targetCheckPt - base.transform.position;
			float current = (0f - Mathf.Atan2(vector4.z, vector4.x)) * 57.29578f;
			float num2 = 0f - Mathf.DeltaAngle(current, base.gameObject.transform.rotation.eulerAngles.y - 90f);
			Transform obj = base.gameObject.transform;
			Quaternion rotation = base.gameObject.transform.rotation;
			obj.rotation = Quaternion.Lerp(rotation, Quaternion.Euler(0f, base.gameObject.transform.rotation.eulerAngles.y + num2, 0f), 0.8f * Time.deltaTime);
			break;
		}
		case 6:
			base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
			base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
			base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			rockPhase++;
			CrossFade("rock_fix_hole", 0.1f);
			base.photonView.RPC("rockPlayAnimation", PhotonTargets.All, "set");
			base.photonView.RPC("endMovingRock", PhotonTargets.All);
			break;
		case 7:
			base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
			base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
			if (base.animation["rock_fix_hole"].normalizedTime >= 1.2f)
			{
				CrossFade("die", 0.1f);
				rockPhase++;
				FengGameManagerMKII.Instance.FinishGame();
			}
			if (base.animation["rock_fix_hole"].normalizedTime >= 0.62f && !rockHitGround)
			{
				rockHitGround = true;
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
				{
					PhotonNetwork.Instantiate("FX/boom1_CT_KICK", new Vector3(0f, 30f, 684f), Quaternion.Euler(270f, 0f, 0f), 0);
				}
				else
				{
					UnityEngine.Object.Instantiate(Resources.Load("FX/boom1_CT_KICK"), new Vector3(0f, 30f, 684f), Quaternion.Euler(270f, 0f, 0f));
				}
			}
			break;
		}
	}

	[RPC]
	private void rockPlayAnimation(string anim)
	{
		rock.animation.Play(anim);
		rock.animation[anim].speed = 1f;
	}

	[RPC]
	private void startMovingRock()
	{
		isROCKMOVE = true;
	}

	[RPC]
	private void endMovingRock()
	{
		isROCKMOVE = false;
	}

	public void setRoute()
	{
		GameObject gameObject = GameObject.Find("routeTrost");
		checkPoints = new ArrayList();
		for (int i = 1; i <= 7; i++)
		{
			checkPoints.Add(gameObject.transform.Find("r" + i).position);
		}
		checkPoints.Add("end");
	}

	private void Start()
	{
		loadskin();
		FengGameManagerMKII.Instance.AddEren(this);
		if (rockLift)
		{
			rock = GameObject.Find("rock");
			rock.animation["lift"].speed = 0f;
		}
		else
		{
			mainCamera = GameObject.Find("MainCamera");
			currentCamera = mainCamera.GetComponent<Camera>();
			inputManager = GameObject.Find("InputManagerController").GetComponent<FengCustomInputs>();
			oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
			base.animation["hit_annie_1"].speed = 0.8f;
			base.animation["hit_annie_2"].speed = 0.7f;
			base.animation["hit_annie_3"].speed = 0.7f;
		}
		hasSpawn = true;
	}

	public void loadskin()
	{
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
		{
			string text = (string)FengGameManagerMKII.Settings[65];
			if ((int)FengGameManagerMKII.Settings[1] == 1 && (text.EndsWith(".jpg") || text.EndsWith(".png") || text.EndsWith(".jpeg")))
			{
				StartCoroutine(CoLoadSkin(text));
			}
		}
		else if (base.photonView.isMine && (int)FengGameManagerMKII.Settings[1] == 1)
		{
			base.photonView.RPC("loadskinRPC", PhotonTargets.AllBuffered, (string)FengGameManagerMKII.Settings[65]);
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
		while (!hasSpawn)
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
}
