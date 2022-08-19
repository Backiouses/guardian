using System.Collections;
using Guardian;
using Guardian.Utilities;
using UnityEngine;

public class AHSSShotGunCollider : MonoBehaviour
{
	public bool active_me;

	public int dmg = 1;

	private int count;

	public ArrayList currentHits = new ArrayList();

	public GameObject currentCamera;

	private int myTeam = 1;

	public float scoreMulti;

	private int viewID = -1;

	private string ownerName = string.Empty;

	private AudioSource hitSound;

	private void Start()
	{
		hitSound = base.gameObject.AddComponent<AudioSource>();
		if (ResourceLoader.TryGetAsset<AudioClip>("Custom/Audio/titan_die.wav", out var value))
		{
			hitSound.clip = value;
		}
		currentCamera = GameObject.Find("MainCamera");
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
		{
			if (!base.transform.root.gameObject.GetPhotonView().isMine)
			{
				base.enabled = false;
				return;
			}
			EnemyfxIDcontainer component = base.transform.root.gameObject.GetComponent<EnemyfxIDcontainer>();
			if (component != null)
			{
				viewID = component.myOwnerViewID;
				ownerName = component.titanName;
				myTeam = PhotonView.Find(viewID).gameObject.GetComponent<HERO>().myTeam;
			}
		}
		else
		{
			myTeam = currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.GetComponent<HERO>().myTeam;
		}
		active_me = true;
		count = 0;
	}

	private void FixedUpdate()
	{
		if (count > 1)
		{
			active_me = false;
		}
		else
		{
			count++;
		}
	}

	private bool CheckIfBehind(GameObject titan)
	{
		Transform transform = titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
		Vector3 to = base.transform.position - transform.transform.position;
		if (Vector3.Angle(-transform.transform.forward, to) < 100f)
		{
			return true;
		}
		return false;
	}

	private void OnTriggerStay(Collider other)
	{
		if ((IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && !base.transform.root.gameObject.GetPhotonView().isMine) || !active_me)
		{
			return;
		}
		switch (other.gameObject.tag)
		{
		case "playerHitbox":
		{
			if (!FengGameManagerMKII.Level.PVP)
			{
				break;
			}
			HitBox component2 = other.gameObject.GetComponent<HitBox>();
			if (component2 == null || component2.transform.root == null)
			{
				break;
			}
			HERO component3 = component2.transform.root.GetComponent<HERO>();
			if (!(component3 != null) || component3.myTeam == myTeam || component3.IsInvincible())
			{
				break;
			}
			float b2 = 1f - Vector3.Distance(other.gameObject.transform.position, base.transform.position) * 0.05f;
			b2 = Mathf.Min(1f, b2);
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
			{
				if (!component3.isGrabbed)
				{
					component3.Die((component2.transform.root.transform.position - base.transform.position).normalized * b2 * 1000f + Vector3.up * 50f, isBite: false);
				}
			}
			else if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && !component3.HasDied() && !component3.isGrabbed)
			{
				component3.MarkDead();
				component3.photonView.RPC("netDie", PhotonTargets.All, (component2.transform.root.position - base.transform.position).normalized * b2 * 1000f + Vector3.up * 50f, false, viewID, ownerName, false);
			}
			break;
		}
		case "erenHitbox":
			if (dmg > 0 && !other.gameObject.transform.root.gameObject.GetComponent<TITAN_EREN>().isHit)
			{
				other.gameObject.transform.root.gameObject.GetComponent<TITAN_EREN>().HitByTitan();
			}
			break;
		case "titanneck":
		{
			HitBox component4 = other.gameObject.GetComponent<HitBox>();
			if (component4 == null || !CheckIfBehind(component4.transform.root.gameObject) || currentHits.Contains(component4))
			{
				break;
			}
			component4.hitPosition = (base.transform.position + component4.transform.position) * 0.5f;
			currentHits.Add(component4);
			hitSound.Play();
			int b3 = (int)((currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - component4.transform.root.rigidbody.velocity).magnitude * 10f * scoreMulti);
			b3 = Mathf.Max(10, b3);
			if (b3 < GuardianClient.Properties.LocalMinDamage.Value)
			{
				GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().ShowDamage(b3);
				break;
			}
			TITAN component5 = component4.transform.root.GetComponent<TITAN>();
			if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
			{
				if (!(component5 != null) || component5.hasDie)
				{
					break;
				}
				FengGameManagerMKII component6 = GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>();
				component6.netShowDamage(b3);
				if ((float)b3 > component5.myLevel * 100f)
				{
					component5.Die();
					if (PlayerPrefs.GetInt("EnableSS", 0) == 1)
					{
						currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().StartSnapshot2(component4.transform.position, b3, component4.transform.root.gameObject, 0.02f);
					}
					component6.UpdatePlayerKillInfo(b3);
				}
				break;
			}
			if (component5 != null)
			{
				if (!component5.hasDie && (float)b3 > component5.myLevel * 100f)
				{
					if (PlayerPrefs.GetInt("EnableSS", 0) == 1)
					{
						currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().StartSnapshot2(component4.transform.position, b3, component4.transform.root.gameObject, 0.02f);
					}
					component5.photonView.RPC("titanGetHit", component5.photonView.owner, base.transform.root.gameObject.GetPhotonView().viewID, b3);
				}
			}
			else if ((bool)component4.transform.root.GetComponent<FEMALE_TITAN>())
			{
				FEMALE_TITAN component7 = component4.transform.root.GetComponent<FEMALE_TITAN>();
				if (!component7.hasDie)
				{
					component7.photonView.RPC("titanGetHit", component7.photonView.owner, base.transform.root.gameObject.GetPhotonView().viewID, b3);
				}
			}
			else if ((bool)component4.transform.root.GetComponent<COLOSSAL_TITAN>() && !component4.transform.root.GetComponent<COLOSSAL_TITAN>().hasDie)
			{
				component4.transform.root.GetComponent<COLOSSAL_TITAN>().photonView.RPC("titanGetHit", component4.transform.root.GetComponent<COLOSSAL_TITAN>().photonView.owner, base.transform.root.gameObject.GetPhotonView().viewID, b3);
			}
			ShowCriticalHitFX(other.gameObject.transform.position);
			break;
		}
		case "titaneye":
		{
			if (currentHits.Contains(other.gameObject))
			{
				break;
			}
			currentHits.Add(other.gameObject);
			GameObject gameObject2 = other.gameObject.transform.root.gameObject;
			if ((bool)gameObject2.GetComponent<FEMALE_TITAN>())
			{
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
				{
					if (!gameObject2.GetComponent<FEMALE_TITAN>().hasDie)
					{
						gameObject2.GetComponent<FEMALE_TITAN>().hitEye();
					}
					break;
				}
				if (!gameObject2.GetComponent<FEMALE_TITAN>().hasDie)
				{
					gameObject2.GetComponent<FEMALE_TITAN>().photonView.RPC("hitEyeRPC", PhotonTargets.MasterClient, base.transform.root.gameObject.GetPhotonView().viewID);
				}
			}
			else if (gameObject2.GetComponent<TITAN>().abnormalType != TitanClass.Crawler)
			{
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
				{
					if (!gameObject2.GetComponent<TITAN>().hasDie)
					{
						gameObject2.GetComponent<TITAN>().HitEye();
					}
					break;
				}
				if (!gameObject2.GetComponent<TITAN>().hasDie)
				{
					gameObject2.GetComponent<TITAN>().photonView.RPC("hitEyeRPC", PhotonTargets.MasterClient, base.transform.root.gameObject.GetPhotonView().viewID);
				}
			}
			ShowCriticalHitFX(other.gameObject.transform.position);
			break;
		}
		case "titanankle":
		{
			if (currentHits.Contains(other.gameObject))
			{
				break;
			}
			currentHits.Add(other.gameObject);
			GameObject gameObject = other.gameObject.transform.root.gameObject;
			TITAN component = gameObject.GetComponent<TITAN>();
			if (component != null && component.abnormalType != TitanClass.Crawler)
			{
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
				{
					if (!component.hasDie)
					{
						component.HitAnkle();
					}
					break;
				}
				if (!component.hasDie)
				{
					component.photonView.RPC("hitAnkleRPC", PhotonTargets.MasterClient, base.transform.root.gameObject.GetPhotonView().viewID);
				}
			}
			else if ((bool)gameObject.GetComponent<FEMALE_TITAN>())
			{
				int b = (int)((currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().main_object.rigidbody.velocity - gameObject.rigidbody.velocity).magnitude * 10f * scoreMulti);
				b = Mathf.Max(10, b);
				if (b < GuardianClient.Properties.LocalMinDamage.Value)
				{
					GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().ShowDamage(b);
					break;
				}
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
				{
					if (other.gameObject.name == "ankleR")
					{
						if ((bool)gameObject.GetComponent<FEMALE_TITAN>() && !gameObject.GetComponent<FEMALE_TITAN>().hasDie)
						{
							gameObject.GetComponent<FEMALE_TITAN>().hitAnkleR(b);
						}
					}
					else if ((bool)gameObject.GetComponent<FEMALE_TITAN>() && !gameObject.GetComponent<FEMALE_TITAN>().hasDie)
					{
						gameObject.GetComponent<FEMALE_TITAN>().hitAnkleL(b);
					}
					break;
				}
				if (other.gameObject.name == "ankleR")
				{
					if (!gameObject.GetComponent<FEMALE_TITAN>().hasDie)
					{
						gameObject.GetComponent<FEMALE_TITAN>().photonView.RPC("hitAnkleRRPC", PhotonTargets.MasterClient, base.transform.root.gameObject.GetPhotonView().viewID, b);
					}
				}
				else if (!gameObject.GetComponent<FEMALE_TITAN>().hasDie)
				{
					gameObject.GetComponent<FEMALE_TITAN>().photonView.RPC("hitAnkleLRPC", PhotonTargets.MasterClient, base.transform.root.gameObject.GetPhotonView().viewID, b);
				}
			}
			ShowCriticalHitFX(other.gameObject.transform.position);
			break;
		}
		}
	}

	private void ShowCriticalHitFX(Vector3 position)
	{
		currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().StartShake(0.2f, 0.3f);
		((IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer) ? ((GameObject)Object.Instantiate(Resources.Load("redCross1"))) : PhotonNetwork.Instantiate("redCross1", base.transform.position, Quaternion.Euler(270f, 0f, 0f), 0)).transform.position = position;
	}
}
