using System;
using Photon;
using UnityEngine;

public class Cannon : Photon.MonoBehaviour
{
	private Vector3 correctPlayerPos = Vector3.zero;

	private Quaternion correctPlayerRot = Quaternion.identity;

	private Quaternion correctBarrelRot = Quaternion.identity;

	public float SmoothingDelay = 5f;

	public Transform firingPoint;

	public Transform ballPoint;

	public Transform barrel;

	public GameObject myCannonBall;

	public LineRenderer myCannonLine;

	public float currentRot;

	public HERO myHero;

	public string settings;

	public bool isCannonGround;

	public void Awake()
	{
		if (base.photonView == null)
		{
			return;
		}
		base.photonView.observed = this;
		barrel = base.transform.Find("Barrel");
		correctPlayerPos = base.transform.position;
		correctPlayerRot = base.transform.rotation;
		correctBarrelRot = barrel.rotation;
		if (base.photonView.isMine)
		{
			firingPoint = barrel.Find("FiringPoint");
			ballPoint = barrel.Find("BallPoint");
			myCannonLine = ballPoint.GetComponent<LineRenderer>();
			if (base.gameObject.name.Contains("CannonGround"))
			{
				isCannonGround = true;
			}
		}
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		PhotonPlayer owner = base.photonView.owner;
		if (FengGameManagerMKII.Instance.allowedToCannon.ContainsKey(owner.Id))
		{
			settings = FengGameManagerMKII.Instance.allowedToCannon[owner.Id].settings;
			base.photonView.RPC("SetSize", PhotonTargets.All, settings);
			int viewID = FengGameManagerMKII.Instance.allowedToCannon[owner.Id].viewID;
			FengGameManagerMKII.Instance.allowedToCannon.Remove(owner.Id);
			CannonPropRegion component = PhotonView.Find(viewID).gameObject.GetComponent<CannonPropRegion>();
			if (component != null)
			{
				component.disabled = true;
				component.destroyed = true;
				PhotonNetwork.Destroy(component.gameObject);
			}
		}
		else if (!owner.isLocal && !FengGameManagerMKII.Instance.restartingMC)
		{
			FengGameManagerMKII.Instance.KickPlayer(owner, ban: false, "spawning cannon without request.");
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
			stream.SendNext(barrel.rotation);
		}
		else
		{
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
			correctBarrelRot = (Quaternion)stream.ReceiveNext();
		}
	}

	[RPC]
	public void SetSize(string settings, PhotonMessageInfo info)
	{
		if (!info.sender.isMasterClient)
		{
			return;
		}
		string[] array = settings.Split(',');
		if (array.Length <= 15)
		{
			return;
		}
		float a = 1f;
		if (array[2] != "default")
		{
			if (array[2].StartsWith("transparent"))
			{
				if (float.TryParse(array[2].Substring(11), out var result))
				{
					a = result;
				}
				Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in componentsInChildren)
				{
					renderer.material = (Material)FengGameManagerMKII.RCAssets.Load("transparent");
					if (Convert.ToSingle(array[10]) != 1f || Convert.ToSingle(array[11]) != 1f)
					{
						renderer.material.mainTextureScale = new Vector2(renderer.material.mainTextureScale.x * Convert.ToSingle(array[10]), renderer.material.mainTextureScale.y * Convert.ToSingle(array[11]));
					}
				}
			}
			else
			{
				Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer2 in componentsInChildren)
				{
					if (!renderer2.name.Contains("Line Renderer"))
					{
						renderer2.material = (Material)FengGameManagerMKII.RCAssets.Load(array[2]);
						if (Convert.ToSingle(array[10]) != 1f || Convert.ToSingle(array[11]) != 1f)
						{
							renderer2.material.mainTextureScale = new Vector2(renderer2.material.mainTextureScale.x * Convert.ToSingle(array[10]), renderer2.material.mainTextureScale.y * Convert.ToSingle(array[11]));
						}
					}
				}
			}
		}
		float num = base.gameObject.transform.localScale.x * Convert.ToSingle(array[3]);
		num -= 0.001f;
		float y = base.gameObject.transform.localScale.y * Convert.ToSingle(array[4]);
		float z = base.gameObject.transform.localScale.z * Convert.ToSingle(array[5]);
		base.gameObject.transform.localScale = new Vector3(num, y, z);
		if (!(array[6] != "0"))
		{
			return;
		}
		Color color = new Color(Convert.ToSingle(array[7]), Convert.ToSingle(array[8]), Convert.ToSingle(array[9]), a);
		MeshFilter[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<MeshFilter>();
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			Mesh mesh = componentsInChildren2[i].mesh;
			Color[] array2 = new Color[mesh.vertexCount];
			for (int j = 0; j < mesh.vertexCount; j++)
			{
				array2[j] = color;
			}
			mesh.colors = array2;
		}
	}

	public void Update()
	{
		if (!base.photonView.isMine)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, correctPlayerPos, Time.deltaTime * SmoothingDelay);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, correctPlayerRot, Time.deltaTime * SmoothingDelay);
			barrel.rotation = Quaternion.Lerp(barrel.rotation, correctBarrelRot, Time.deltaTime * SmoothingDelay);
			return;
		}
		Vector3 vector = new Vector3(0f, -30f, 0f);
		Vector3 position = ballPoint.position;
		Vector3 vector2 = ballPoint.forward * 300f;
		float num = 40f / vector2.magnitude;
		myCannonLine.SetWidth(0.5f, 40f);
		myCannonLine.SetVertexCount(100);
		for (int i = 0; i < 100; i++)
		{
			myCannonLine.SetPosition(i, position);
			position += vector2 * num + 0.5f * vector * num * num;
			vector2 += vector * num;
		}
		float num2 = 30f;
		if (FengGameManagerMKII.InputRC.isInputCannon(InputCodeRC.CannonSlow))
		{
			num2 = 5f;
		}
		if (isCannonGround)
		{
			if (FengGameManagerMKII.InputRC.isInputCannon(InputCodeRC.CannonForward))
			{
				if (currentRot <= 32f)
				{
					currentRot += Time.deltaTime * num2;
					barrel.Rotate(new Vector3(0f, 0f, Time.deltaTime * num2));
				}
			}
			else if (FengGameManagerMKII.InputRC.isInputCannon(InputCodeRC.CannonBack) && currentRot >= -18f)
			{
				currentRot += Time.deltaTime * (0f - num2);
				barrel.Rotate(new Vector3(0f, 0f, Time.deltaTime * (0f - num2)));
			}
			if (FengGameManagerMKII.InputRC.isInputCannon(InputCodeRC.CannonLeft))
			{
				base.transform.Rotate(new Vector3(0f, Time.deltaTime * (0f - num2), 0f));
			}
			else if (FengGameManagerMKII.InputRC.isInputCannon(InputCodeRC.CannonRight))
			{
				base.transform.Rotate(new Vector3(0f, Time.deltaTime * num2, 0f));
			}
		}
		else
		{
			if (FengGameManagerMKII.InputRC.isInputCannon(InputCodeRC.CannonForward))
			{
				if (currentRot >= -50f)
				{
					currentRot += Time.deltaTime * (0f - num2);
					barrel.Rotate(new Vector3(Time.deltaTime * (0f - num2), 0f, 0f));
				}
			}
			else if (FengGameManagerMKII.InputRC.isInputCannon(InputCodeRC.CannonBack) && currentRot <= 40f)
			{
				currentRot += Time.deltaTime * num2;
				barrel.Rotate(new Vector3(Time.deltaTime * num2, 0f, 0f));
			}
			if (FengGameManagerMKII.InputRC.isInputCannon(InputCodeRC.CannonLeft))
			{
				base.transform.Rotate(new Vector3(0f, Time.deltaTime * (0f - num2), 0f));
			}
			else if (FengGameManagerMKII.InputRC.isInputCannon(InputCodeRC.CannonRight))
			{
				base.transform.Rotate(new Vector3(0f, Time.deltaTime * num2, 0f));
			}
		}
		if (FengGameManagerMKII.InputRC.isInputCannon(InputCodeRC.CannonFire))
		{
			Fire();
		}
		else if (FengGameManagerMKII.InputRC.isInputCannonDown(InputCodeRC.CannonMount))
		{
			if (myHero != null)
			{
				myHero.isCannon = false;
				myHero.myCannonRegion = null;
				Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetMainObject(myHero.gameObject);
				myHero.baseRigidBody.velocity = Vector3.zero;
				myHero.photonView.RPC("ReturnFromCannon", PhotonTargets.Others);
				myHero.skillCDLast = myHero.skillCDLastCannon;
				myHero.skillCDDuration = myHero.skillCDLast;
			}
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	public void Fire()
	{
		if (myHero.skillCDDuration <= 0f)
		{
			EnemyCheckCollider[] componentsInChildren = PhotonNetwork.Instantiate("FX/boom2", firingPoint.position, firingPoint.rotation, 0).GetComponentsInChildren<EnemyCheckCollider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].dmg = 0;
			}
			myCannonBall = PhotonNetwork.Instantiate("RCAsset/CannonBallObject", ballPoint.position, firingPoint.rotation, 0);
			myCannonBall.rigidbody.velocity = firingPoint.forward * 300f;
			myCannonBall.GetComponent<CannonBall>().myHero = myHero;
			myHero.skillCDDuration = 3.5f;
		}
	}

	public void OnDestroy()
	{
		if (!PhotonNetwork.isMasterClient || FengGameManagerMKII.Instance.isRestarting)
		{
			return;
		}
		string[] array = settings.Split(',');
		if (array[0] == "photon")
		{
			if (array.Length > 15)
			{
				GameObject obj = PhotonNetwork.Instantiate("RCAsset/" + array[1] + "Prop", new Vector3(Convert.ToSingle(array[12]), Convert.ToSingle(array[13]), Convert.ToSingle(array[14])), new Quaternion(Convert.ToSingle(array[15]), Convert.ToSingle(array[16]), Convert.ToSingle(array[17]), Convert.ToSingle(array[18])), 0);
				obj.GetComponent<CannonPropRegion>().settings = settings;
				obj.GetPhotonView().RPC("SetSize", PhotonTargets.AllBuffered, settings);
			}
			else
			{
				PhotonNetwork.Instantiate("RCAsset/" + array[1] + "Prop", new Vector3(Convert.ToSingle(array[2]), Convert.ToSingle(array[3]), Convert.ToSingle(array[4])), new Quaternion(Convert.ToSingle(array[5]), Convert.ToSingle(array[6]), Convert.ToSingle(array[7]), Convert.ToSingle(array[8])), 0).GetComponent<CannonPropRegion>().settings = settings;
			}
		}
	}
}
