using Photon;
using UnityEngine;

public class RockThrow : Photon.MonoBehaviour
{
	private bool launched;

	private Vector3 oldP;

	private Vector3 v;

	private Vector3 r;

	private void Start()
	{
		r = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
	}

	private void Update()
	{
		if (!launched)
		{
			return;
		}
		base.transform.Rotate(r);
		v -= 20f * Vector3.up * Time.deltaTime;
		base.transform.position += v * Time.deltaTime;
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && !PhotonNetwork.isMasterClient)
		{
			return;
		}
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Ground");
		LayerMask layerMask2 = 1 << LayerMask.NameToLayer("Players");
		LayerMask layerMask3 = 1 << LayerMask.NameToLayer("EnemyAABB");
		LayerMask layerMask4 = (int)layerMask2 | (int)layerMask | (int)layerMask3;
		RaycastHit[] array = Physics.SphereCastAll(base.transform.position, 2.5f * base.transform.lossyScale.x, base.transform.position - oldP, Vector3.Distance(base.transform.position, oldP), layerMask4);
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit raycastHit = array[i];
			switch (LayerMask.LayerToName(raycastHit.collider.gameObject.layer))
			{
			case "EnemyAABB":
			{
				TITAN component4 = raycastHit.collider.gameObject.transform.root.gameObject.GetComponent<TITAN>();
				if (component4 != null && !component4.hasDie)
				{
					component4.HitAnkle();
					if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
					{
						component4.photonView.RPC("hitAnkleRPC", PhotonTargets.Others, component4.photonView.ownerId);
					}
				}
				explode();
				break;
			}
			case "Players":
			{
				GameObject gameObject = raycastHit.collider.gameObject.transform.root.gameObject;
				TITAN_EREN component = gameObject.GetComponent<TITAN_EREN>();
				if (component != null)
				{
					if (!component.isHit)
					{
						component.HitByTitan();
					}
					break;
				}
				HERO component2 = gameObject.GetComponent<HERO>();
				if (!(component2 != null) || component2.HasDied() || component2.IsInvincible() || component2.isGrabbed)
				{
					break;
				}
				if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
				{
					component2.Die(v.normalized * 1000f + Vector3.up * 50f, isBite: false);
				}
				else if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer)
				{
					component2.MarkDead();
					int num = -1;
					string text = "Rock";
					EnemyfxIDcontainer component3 = base.transform.root.gameObject.GetComponent<EnemyfxIDcontainer>();
					if (component3 != null)
					{
						num = component3.myOwnerViewID;
						text = component3.titanName;
					}
					component2.photonView.RPC("netDie", PhotonTargets.All, v.normalized * 1000f + Vector3.up * 50f, false, num, text, true);
				}
				break;
			}
			case "Ground":
				explode();
				break;
			}
		}
		oldP = base.transform.position;
	}

	private void explode()
	{
		GameObject gameObject;
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
		{
			gameObject = PhotonNetwork.Instantiate("FX/boom6", base.transform.position, base.transform.rotation, 0);
			if (base.transform.root.gameObject.GetComponent<EnemyfxIDcontainer>() != null)
			{
				gameObject.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = base.transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().myOwnerViewID;
				gameObject.GetComponent<EnemyfxIDcontainer>().titanName = base.transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().titanName;
			}
		}
		else
		{
			gameObject = (GameObject)Object.Instantiate(Resources.Load("FX/boom6"), base.transform.position, base.transform.rotation);
		}
		gameObject.transform.localScale = base.transform.localScale;
		float b = 1f - Vector3.Distance(GameObject.Find("MainCamera").transform.position, gameObject.transform.position) * 0.05f;
		b = Mathf.Min(1f, b);
		GameObject.Find("MainCamera").GetComponent<IN_GAME_MAIN_CAMERA>().StartShake(b, b);
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			PhotonNetwork.Destroy(base.photonView);
		}
	}

	public void launch(Vector3 v1)
	{
		launched = true;
		oldP = base.transform.position;
		v = v1;
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Multiplayer && PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("launchRPC", PhotonTargets.Others, v, oldP);
		}
	}

	[RPC]
	private void launchRPC(Vector3 v, Vector3 p)
	{
		launched = true;
		base.transform.position = p;
		oldP = p;
		base.transform.parent = null;
		launch(v);
	}

	[RPC]
	private void initRPC(int viewID, Vector3 scale, Vector3 pos, float level)
	{
		GameObject gameObject = PhotonView.Find(viewID).gameObject;
		Transform parent = gameObject.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
		base.transform.localScale = gameObject.transform.localScale;
		base.transform.parent = parent;
		base.transform.localPosition = pos;
	}
}
